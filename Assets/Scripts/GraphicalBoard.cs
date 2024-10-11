using System.Collections.Generic;
using UnityEngine;


public class GraphicalBoard : MonoBehaviour
{
   public GameObject tilePrefab;
   public Material whiteMaterial;
   public Material blackMaterial;
   public static List<GameObject> squaresList = new List<GameObject>();

   public void CreateGraphicalBoard()
   {
      for (int fileIdx = 7; fileIdx >= 0; fileIdx--)
      {
         for (int rankIdx = 7; rankIdx >= 0; rankIdx--)
         {
            var position = new Vector2(rankIdx - 3.5f, fileIdx - 3.5f);
            bool isLight = Utilities.IsLightSquare(fileIdx, rankIdx);
            CreateSquare(isLight, position);

         }
      }
      squaresList.Reverse();
   }
   public void CreateSquare(bool isLight, Vector2 position)
   {
      GameObject newSquare = Instantiate(tilePrefab, position, Quaternion.identity);
      Renderer renderer = newSquare.GetComponent<Renderer>();

      renderer.material = (isLight) ? whiteMaterial : blackMaterial;
      squaresList.Add(newSquare);
   }

   public void HighlightSquares(List<int> squareIndices)
   {
      foreach (int squareIndex in squareIndices)
      {
         squaresList[squareIndex].GetComponent<Renderer>().material.color = Color.green;
      }
   }
}