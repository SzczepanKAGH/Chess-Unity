using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PromotionScript : MonoBehaviour
{
   public EventHandler<PieceToPromoteChosen> OnPromotionChosen;

   private const int QueenID = 0;
   private const int KnightID = 1;
   private const int RookID = 2;
   private const int BishopID = 3;

   private int PieceID;
   private Coords PromotionCoords;
   private PieceColor PromotingSide;

   public void SetPiece(int pieceId, Coords promotionCoords, PieceColor promotingSide)
   {
      PieceID = pieceId;
      PromotionCoords = promotionCoords;
      PromotingSide = promotingSide;
   }

   private void OnMouseDown()
   {

      switch (PieceID)
      {
         case QueenID:
            OnPromotionChosen?.Invoke(this, new PieceToPromoteChosen(PieceType.Queen, PromotingSide,
                                                                     PromotionCoords));
            break;

         case KnightID:
            OnPromotionChosen?.Invoke(this, new PieceToPromoteChosen(PieceType.Knight, PromotingSide,
                                                                     PromotionCoords));
            break;

         case RookID:
            OnPromotionChosen?.Invoke(this, new PieceToPromoteChosen(PieceType.Rook, PromotingSide,
                                                                     PromotionCoords));
            break;

         case BishopID:
            OnPromotionChosen?.Invoke(this, new PieceToPromoteChosen(PieceType.Bishop, PromotingSide,
                                                                     PromotionCoords));
            break;

         default:
            Debug.LogError("No pieceID specified");
            break;

      }
   }
}

public class PieceToPromoteChosen : EventArgs
{
   public PieceType PieceChosen;
   public PieceColor PromotingSide;
   public Coords PromotionCoords;

   public PieceToPromoteChosen(PieceType pieceChosen, PieceColor promotingSide, Coords promotionCoords)
   {
      PieceChosen = pieceChosen;
      PromotingSide = promotingSide;
      PromotionCoords = promotionCoords;
   }
}
