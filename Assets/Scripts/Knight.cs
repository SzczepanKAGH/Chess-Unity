using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PieceLogic
{
   public override PieceType Type => PieceType.Knight;

   public Knight(PieceColor color, GameObject representation, Coords position, Board board, bool hasMoved = false)
      : base(color, representation, position, board)
   {
      Directions = new() { (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2), (1, 2) };
   }

   public override void GetPossibleMoves()
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         int rank = Position.Rank + rankOffset;
         int file = Position.File + fileOffset;
         PieceColor oppositeColor = Utilities.GetOppositeColor(this.Color);

         if (Utilities.IsWithinBounds(rank, file))
         {
            if (Board.IsOccupied(rank, file))
            {
               //Debug.Log($"Square {rank}, {file} is occupied");
               if (Board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
               {
                  possibleAttacks.Add((rank, file));
                  possibleMoves.Add((rank, file));
                  //Debug.Log($"Added Attack {rank}, {file}");
                  continue;
               }
            }
            else
            {
               possibleMoves.Add((rank, file));
            }
         }
      }
      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
}
