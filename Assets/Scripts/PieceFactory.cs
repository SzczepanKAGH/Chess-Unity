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

   public static PieceLogic CreatePiece(Coords pieceCoords, PieceType type, PieceColor color, bool hasMoved = false)
   {
      GameObject visualRepresentation = pieceRenderer.SpawnPiece(pieceCoords, type, color);
      switch (type)
      {
         case PieceType.Pawn:
            return new Pawn(color, visualRepresentation, pieceCoords, hasMoved);

         case PieceType.Rook:
            return new Rook(color, visualRepresentation, pieceCoords, hasMoved);

         case PieceType.Knight:
            return new Knight(color, visualRepresentation, pieceCoords);

         case PieceType.Bishop:
            return new Bishop(color, visualRepresentation, pieceCoords);

         case PieceType.Queen:
            return new Queen(color, visualRepresentation, pieceCoords);

         case PieceType.King:
            return new King(color, visualRepresentation, pieceCoords, true, true, hasMoved);

         default:
            return null;
      }

   }
}
