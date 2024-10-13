using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceFactory : MonoBehaviour
{
   public static PieceRenderer pieceRenderer;

   private void Awake()
   {
      pieceRenderer = gameObject.AddComponent<PieceRenderer>();
   }

   public static GameObject SpawnPiece(Coords pieceCoords, PieceType pieceType, PieceColor pieceColor)
   {
      var newPiece = new GameObject();
      newPiece.name = $"{pieceType}-{pieceColor}-{pieceCoords}";

      var pieceColider = newPiece.AddComponent<BoxCollider2D>();
      pieceColider.isTrigger = true;

      newPiece.AddComponent<SpriteRenderer>();
      newPiece.AddComponent<PieceRenderer>();

      newPiece.GetComponent<SpriteRenderer>().sprite = PieceRenderer.PieceSprites[$"{pieceColor}_{pieceType}"];
      newPiece.GetComponent<SpriteRenderer>().sortingOrder = 1;
      newPiece.transform.position = GraphicalBoard.squaresList[pieceCoords.Rank, pieceCoords.File].transform.position;

      return newPiece;
   }

   public static PieceLogic CreatePiece(Coords pieceCoords, PieceType type, PieceColor color, Board board, bool hasMoved = false)
   {
      GameObject visualRepresentation = SpawnPiece(pieceCoords, type, color);

      switch (type)
      {
         case PieceType.Pawn:
            return new Pawn(color, visualRepresentation, pieceCoords, board);

         case PieceType.Rook:
            return new Rook(color, visualRepresentation, pieceCoords, board, hasMoved);

         case PieceType.Knight:
            return new Knight(color, visualRepresentation, pieceCoords, board);

         case PieceType.Bishop:
            return new Bishop(color, visualRepresentation, pieceCoords, board);

         case PieceType.Queen:
            return new Queen(color, visualRepresentation, pieceCoords, board);

         case PieceType.King:
            return new King(color, visualRepresentation, pieceCoords, board, true, true, hasMoved);

         default:
            return null;
      }

   }
}
