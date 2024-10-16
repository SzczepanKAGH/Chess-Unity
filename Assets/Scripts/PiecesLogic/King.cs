using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : PieceLogic
{
   public override PieceType Type => PieceType.King;
   public override bool HasMoved { get; set; }

   public King(PieceColor color, Coords position, bool hasMoved = false) : base(color, position)
   {
      HasMoved = hasMoved;
   }

   public override void GetPossibleMoves(Board board)
   {
      PossibleMoves.Clear();
      PossibleAttacks.Clear();

      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      Directions = new() { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         Coords possibleMove = Position + (rankOffset, fileOffset);

         if (Utilities.IsWithinBounds(possibleMove))
         {
            if (board.IsOccupied(possibleMove))
            {
               if (board.GetPieceAtSquare(possibleMove).IsColor(oppositeColor))
               {
                  PossibleMoves.Add(possibleMove);
                  PossibleAttacks.Add(possibleMove);
               }
            }
            else 
            {
               PossibleMoves.Add(possibleMove);
            }
         }
      }

      Coords kingSideCastleSquare = new(Position.Rank, 6);
      Coords queenSideCastleSquare = new(Position.Rank, 2);

      if (CanCastleKingSide(board)) PossibleMoves.Add(kingSideCastleSquare);
      if (CanCastleQueenSide(board)) PossibleMoves.Add(queenSideCastleSquare);
   }

   public override bool CanCastleQueenSide(Board board)
   {
      if (HasMoved) return false;

      Coords rookCoords = new(Position.Rank, 0);
      PieceLogic rookForCastling = board.GetPieceAtSquare(rookCoords);

      if (rookForCastling?.Type is PieceType.Rook && rookForCastling.HasMoved is false)
      {
         PieceColor opponentColor = Utilities.GetOppositeColor(Color);
         List<PieceLogic> enemyPieces = board.GetPiecesOfColor(opponentColor);

         for (int squareFile = 1; squareFile < Position.File; squareFile++)
         {
            Coords squareBetween = new(Position.Rank, squareFile);

            if (board.IsOccupied(squareBetween) || 
            enemyPieces.Any(piece => piece.PossibleMoves.Contains(squareBetween))) return false;
         }
         return true;
      }
      else
      {
         return false;
      }
   }

   public override bool CanCastleKingSide(Board board) 
   {
      if (HasMoved) return false;

      Coords rookCoords = new(Position.Rank, 7);
      PieceLogic rookForCastling = board.GetPieceAtSquare(rookCoords);

      if (rookForCastling?.Type is PieceType.Rook && rookForCastling.HasMoved is false)
      {
         PieceColor opponentColor = Utilities.GetOppositeColor(Color);
         List<PieceLogic> enemyPieces = board.GetPiecesOfColor(opponentColor);

         for (int squareFile = 6; squareFile > Position.File; squareFile--)
         {
            Coords squareBetween = new(Position.Rank, squareFile);

            if (board.IsOccupied(squareBetween) ||
            enemyPieces.Any(piece => piece.PossibleMoves.Contains(squareBetween))) return false;
         }
         return true;
      }
      else
      {
         return false;
      }
   }

   public override void Move(Coords positionTo, bool isClone = false)
   {
      base.Move(positionTo, isClone);
      HasMoved = true;
   }
}
