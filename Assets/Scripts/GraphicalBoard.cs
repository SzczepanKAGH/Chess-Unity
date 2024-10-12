using System.Collections.Generic;
using UnityEngine;


public class GraphicalBoard : MonoBehaviour
{
   public GameObject tilePrefab;
   public Material whiteMaterial;
   public Material blackMaterial;
   public static GameObject[,] squaresList = new GameObject[8, 8];

   public void CreateGraphicalBoard()
   {
      for (int rankIdx = 0; rankIdx < 8; rankIdx++)
      {
         for (int fileIdx = 0; fileIdx < 8; fileIdx++)
         {
            var position = new Vector2(fileIdx - 3.5f, rankIdx - 3.5f); // In Unity X goes first, then Y
            bool isLight = Utilities.IsLightSquare(rankIdx, fileIdx);
            var newSquare = CreateSquare(isLight, position);
            squaresList[rankIdx, fileIdx] = (newSquare);
         }
      }
   }

   public GameObject CreateSquare(bool isLight, Vector2 position)
   {
      GameObject newSquare = Instantiate(tilePrefab, position, Quaternion.identity);
      Renderer renderer = newSquare.GetComponent<Renderer>();

      renderer.material = (isLight) ? whiteMaterial : blackMaterial;
      return newSquare;
   }

   public void HighlightSquares(List<(int, int)> squareIndices)
   {
      foreach (var (rankIndex, fileIndex) in squareIndices)
      {
         squaresList[rankIndex, fileIndex].GetComponent<Renderer>().material.color = Color.green;
      }
   }
}