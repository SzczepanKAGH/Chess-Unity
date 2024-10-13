using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : PieceLogic
{
   public override PieceType Type => PieceType.Pawn;
   private bool HasNotMoved { get; set; }

   public Pawn(PieceColor color, GameObject representation, Coords position, Board board)
      : base(color, representation, position, board)
   {
      HasNotMoved = (color == PieceColor.White && position.Rank == 1) ||
                    (color == PieceColor.Black && position.Rank == 6);
   }


   public override void GetPossibleMoves()
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();
      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      int direction = (IsColor(PieceColor.White)) ? 1 : -1;

      //Coords moveForward = Position + (direction, 0);

      int oneNextRank = Position.Rank + direction;
      int twoNextRank = Position.Rank + 2 * direction;

      int file = Position.File;
      var attackDirections = new List<int>() { -1, 1 };

      if (!Board.IsOccupied(oneNextRank, file))
      {
         //if (enpassant)

         possibleMoves.Add((oneNextRank, file));

         if (HasNotMoved && !Board.IsOccupied(twoNextRank, file))
         {
            possibleMoves.Add((twoNextRank, file));
         }
      }

      foreach (int attackDelta in attackDirections)
      {
         int attackFile = file + attackDelta;

         if (!Utilities.IsWithinBounds(oneNextRank, attackFile)) { continue; }

         if (Board.IsOccupied(oneNextRank, attackFile))
         {
            if (Board.GetPieceAtSquare(oneNextRank, attackFile).IsColor(oppositeColor))
            {
               Debug.Log($"{oneNextRank}, {attackFile}");
               possibleMoves.Add((oneNextRank, attackFile));
               possibleAttacks.Add((oneNextRank, attackFile));
            }
         }
      }

      if (possibleMoves.Any(move => move.Item1 == 0 || move.Item1 == 7))
      {
         //handle promotion
      }

      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }

   public override void Move(Coords positionTo)
   {
      Position = positionTo;
      HasNotMoved = false;
      Renderer.UpdateVisualPosition(positionTo);
   }
}
