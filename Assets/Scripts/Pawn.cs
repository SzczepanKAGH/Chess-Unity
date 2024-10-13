using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : PieceLogic
{
   public override PieceType Type => PieceType.Pawn;
   public bool HasMoved { get; set; }

   public Pawn(PieceColor color, GameObject representation, Coords position, Board board, bool hasMoved = false)
      : base(color, representation, position, board)
   {
      this.HasMoved = hasMoved;
   }

   public override void GetPossibleMoves()
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();
      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      int direction = (IsColor(PieceColor.White)) ? 1 : -1;

      int oneNextRank = Position.Rank + direction;
      int twoNextRank = Position.Rank + 2 * direction;

      int file = Position.File;
      var attackDirections = new List<int>() { -1, 1 };

      if (!Board.IsOccupied(oneNextRank, file) && Utilities.IsWithinBounds(oneNextRank, file))
      {
         //if (enpassant)

         possibleMoves.Add((oneNextRank, file));

         foreach (int attackFile in attackDirections)
         {
            if (!Utilities.IsWithinBounds(oneNextRank, attackFile)){ continue; }

            if (Board.IsOccupied(oneNextRank, attackFile))
            {
               if (Board.GetPieceAtSquare(oneNextRank, attackFile).IsColor(Color))
               {
                  possibleMoves.Add((oneNextRank, attackFile));
                  possibleAttacks.Add((oneNextRank, attackFile));
               }
            }
         }

         if (!HasMoved && !Board.IsOccupied(twoNextRank, file))
         {
            possibleMoves.Add((twoNextRank, file));
         }
      }

      if (possibleMoves.Any(move => move.Item1 == 0 || move.Item1 == 7))
      {
         //handle promotion
      }

      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
}
