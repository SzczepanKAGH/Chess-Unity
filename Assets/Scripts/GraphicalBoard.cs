using System.Collections.Generic;
using System;
using UnityEngine;


public class GraphicalBoard : MonoBehaviour
{
   public GameObject tilePrefab;
   public GameObject highlightPrefab;
   public Material whiteMaterial;
   public Material blackMaterial;
   public static GameObject[,] squaresList = new GameObject[8, 8];
   public List<GameObject> highlitedSquares = new();

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

   public void HighlightSquares(List<(int, int)> squareIndices)
   {
      foreach (var (rankIndex, fileIndex) in squareIndices)
      {
         Coords squarePosition = new(rankIndex, fileIndex);
         HighlightSquare(squarePosition);
      }
   }

   private void HighlightSquare(Coords squarePosition)
   {
      int rankIndex = squarePosition.Rank;
      int fileIndex = squarePosition.File;

      Vector2 squarePositionAsVector = new Vector2(fileIndex - 3.5f, rankIndex - 3.5f);
      GameObject newHighlight = Instantiate(highlightPrefab, squarePositionAsVector, Quaternion.identity);

      highlitedSquares.Add(newHighlight);
   }

   public void ClearHighlitedSquares()
   {
      foreach (GameObject highlitedSquare in highlitedSquares)
      {
         Destroy(highlitedSquare);
      }
   }
}