using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King };
public enum PieceColor { White, Black };

public interface IPieceRenderer
{
   private static Dictionary<string, Sprite> PieceSprites;
   public GameObject SpawnPiece(Coords pieceCoords, PieceType type, PieceColor color);
}
public class PieceRenderer : MonoBehaviour, IPieceRenderer
{
   public static Dictionary<string, Sprite> PieceSprites = new Dictionary<string, Sprite>();

   public GameObject SpawnPiece(Coords pieceCoords, PieceType pieceType, PieceColor pieceColor)
   {
      var newPiece = new GameObject(name = $"{pieceType}-{pieceColor}-{pieceCoords}");
      newPiece.AddComponent<SpriteRenderer>();

      newPiece.GetComponent<SpriteRenderer>().sprite = PieceSprites[$"{pieceColor}_{pieceType}"];
      newPiece.transform.position = GraphicalBoard.squaresList[pieceCoords.GetBoardIndex()].transform.position;

      return newPiece;
   }

   public static void LoadSprites()
   {
      for (int piece = (int)PieceType.Pawn; piece <= (int)PieceType.King; piece++)
      {
         foreach (var color in Enum.GetValues(typeof(PieceColor)))
         {
            Sprite pieceSprite = Resources.Load<Sprite>($"Sprites/Piece={(PieceType)piece}, Side={color}");

            Debug.Log($"{color}_{(PieceType)piece}");
            PieceSprites.Add($"{color}_{(PieceType)piece}", pieceSprite);
         }
      }
   }
}
