using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : PieceLogic
{
   public override PieceType Type => PieceType.Pawn;
   public override bool HasMoved { get; set; }
 
   public Pawn(PieceColor color, Coords position) : base(color, position)
   {
      HasMoved = !(color == PieceColor.White && position.Rank == 1) &&
                 !(color == PieceColor.Black && position.Rank == 6);
   }

   public override void GetPossibleMoves(Board board)
   {
      PossibleMoves.Clear();
      PossibleAttacks.Clear();

      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      int direction = (IsColor(PieceColor.White)) ? 1 : -1;

      Coords oneSquareMove = Position + (direction, 0);
      Coords twoSquareMove = Position + (2 * direction, 0);

      if (!board.IsOccupied(oneSquareMove))
      {
         //if (enpassant)
         PossibleMoves.Add(oneSquareMove);


         if (!HasMoved && !board.IsOccupied(twoSquareMove))
         {
            PossibleMoves.Add(twoSquareMove);
         }
      }

      var attackDirections = new List<int>() { -1, 1 };

      foreach (int attackDelta in attackDirections)
      {
         Coords possibleAttack = Position + (direction, attackDelta);

         if (!Utilities.IsWithinBounds(possibleAttack)) { continue; }

         if (board.IsOccupied(possibleAttack) &&
             board.GetPieceAtSquare(possibleAttack).IsColor(oppositeColor))
         {
            PossibleMoves.Add(possibleAttack);
            PossibleAttacks.Add(possibleAttack);
         }
      }


      if (PossibleMoves.Any(move => move.Rank == 0 || move.Rank == 7))
      {
         //handle promotion
      }
   }

   public override void Move(Coords positionTo, bool isClone = false)
   {
      base.Move(positionTo, isClone);
      HasMoved = true;
   }
}
