using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : PieceLogic
{
   public override PieceType Type => PieceType.King;
   public bool CanCastleQueenside { get; set; }
   public bool CanCastleKingside { get; set; }
   public bool HasMoved { get; set; }

   public King(PieceColor color,
               GameObject representation,
               Coords position,
               Board board,
               bool canCastleQueenside = true,
               bool canCastleKingside = true,
               bool hasMoved = false)
               : base(color, representation, position, board)
   {
      this.CanCastleKingside = canCastleKingside;
      this.CanCastleQueenside = canCastleQueenside;
      this.HasMoved = hasMoved;
   }

   public override void GetPossibleMoves()
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();
      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      Directions = new() { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         int rank = Position.Rank + rankOffset;
         int file = Position.File + fileOffset;

         if (Utilities.IsWithinBounds(rank, file))
         {
            if (Board.IsOccupied(rank, file))
            {
               if (!Board.IsUnderAttackBy(rank, file, oppositeColor) && Board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
               {
                  possibleMoves.Add((rank, file));
                  possibleAttacks.Add((rank, file));
               }
            }
            else if (!Board.IsUnderAttackBy(rank, file, oppositeColor))
            {
               possibleMoves.Add((rank, file));
            }
         }
      }
      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
}
