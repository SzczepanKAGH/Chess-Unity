using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class GraphicalBoard : MonoBehaviour
{
   public GameObject tilePrefab;
   public GameObject highlightPrefab;
   public Material whiteMaterial;
   public Material blackMaterial;
   public GameObject[,] squaresList = new GameObject[8, 8];
   public List<GameObject> highlitedSquares = new();
   public List<GameObject> graphicalPiecesList = new();

   public void CreateGraphicalBoard()
   {
      for (int rankIdx = 0; rankIdx < 8; rankIdx++)
      {
         for (int fileIdx = 0; fileIdx < 8; fileIdx++)
         {
            var position = new Vector2(fileIdx - 3.5f, rankIdx - 3.5f); // In Unity X goes first, then Y
            bool isLight = Utilities.IsLightSquare(rankIdx, fileIdx);
            var newSquare = CreateSquare(isLight, position);

            newSquare.GetComponent<SquareScript>().SquarePosition = new Coords(rankIdx, fileIdx);


            squaresList[rankIdx, fileIdx] = (newSquare);
         }
      }
   }

   private GameObject CreateSquare(bool isLight, Vector2 position)
   {
      GameObject newSquare = Instantiate(tilePrefab, position, Quaternion.identity);
      Renderer renderer = newSquare.GetComponent<Renderer>();

      renderer.material = (isLight) ? whiteMaterial : blackMaterial;

      newSquare.AddComponent<SquareScript>();
      return newSquare;
   }

   public void HighlightSquares(List<Coords> moveCoords, HashSet<Coords> attackCoords)
   {
      List<Coords> onlyMoves = moveCoords.Except(attackCoords).ToList();

      foreach (var squareToMove in onlyMoves) HighlightSquare(squareToMove);
      foreach (var squareToAttack in attackCoords) HighlightSquare(squareToAttack, true);
   }

   private void HighlightSquare(Coords squarePosition, bool isAttack = false)
   {
      int rankIndex = squarePosition.Rank;
      int fileIndex = squarePosition.File;

      Vector2 squarePositionAsVector = new Vector2(fileIndex - 3.5f, rankIndex - 3.5f);
      GameObject newHighlight = Instantiate(highlightPrefab, squarePositionAsVector, Quaternion.identity);

      if (isAttack) newHighlight.transform.localScale = new Vector2(0.8f, 0.8f);
 
      highlitedSquares.Add(newHighlight);
   }

   public void ClearHighlitedSquares()
   {
      foreach (GameObject highlitedSquare in highlitedSquares) Destroy(highlitedSquare);
   }
}