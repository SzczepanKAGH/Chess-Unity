using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;


public interface IPieceLogic
{ 
   public PieceType Type { get; }
   public PieceColor Color { get; }
   public Coords Position { get; }
   public GameObject GraphicalRepresentation { get; }
   public List<(int, int)> PossibleMoves { get; set; }
   public List<(int, int)> PossibleAttacks { get; set; }
   public void GetPossibleMoves(Board board);
   public bool IsColor(PieceColor color);
}
public abstract class PieceLogic : IPieceLogic
{
   public Coords Position { get; }
   public abstract PieceType Type { get; }
   public PieceColor Color { get; }
   public GameObject GraphicalRepresentation { get; }
   public List<(int, int)> PossibleMoves { get; set; }
   public List<(int, int)> PossibleAttacks { get; set; }

   protected List<(int, int)> Directions = new List<(int, int)>();

   public PieceLogic(PieceColor color, GameObject representation, Coords position)
   {
      this.GraphicalRepresentation = representation;
      this.Color = color;
      this.Position = position;
   }

   public virtual void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();
      PieceColor oppositeColor = Utilities.GetOppositeColor(this.Color);

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         int rank = Position.Rank + rankOffset;
         int file = Position.File + fileOffset;

         while (Utilities.IsWithinBounds(rank, file))
         {
            if (board.IsOccupied(rank, file))
            {
               Debug.Log($"Square {rank}, {file} is occupied");
               if (board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
               {
                  possibleAttacks.Add((rank, file));
                  Debug.Log($"Added Attack {rank}, {file}");
               }
               break;
            }
            rank += rankOffset;
            file += fileOffset;

            possibleMoves.Add((rank, file));
         }
      }
      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
   public bool IsColor(PieceColor color) => this.Color == color;
}

public class Pawn : PieceLogic
{
   public override PieceType Type => PieceType.Pawn;
   public bool HasMoved { get; set; }

   public Pawn(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : 
      base(color, representation, position) 
   {
      this.HasMoved = hasMoved;
   }
   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<(int, int)>();
      PossibleMoves = possibleMoves;
   }

}

public class Rook : PieceLogic
{
   public override PieceType Type => PieceType.Rook;
   public bool HasMoved { get; set; }

   public Rook(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : base(color, representation, position)
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1) };
      this.HasMoved = hasMoved;
   }
}

/* == KNIGHT ==*/
public class Knight : PieceLogic
{
   public override PieceType Type => PieceType.Knight;

   public Knight(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : base(color, representation, position) 
   {
      Directions = new() { (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2), (1, 2) };
   }

   public override void GetPossibleMoves(Board board)
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
            if (board.IsOccupied(rank, file))
            {
               Debug.Log($"Square {rank}, {file} is occupied");
               if (board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
               {
                  possibleAttacks.Add((rank, file));
                  possibleMoves.Add((rank, file));
                  Debug.Log($"Added Attack {rank}, {file}");
                  continue;
               }
               else
               {
                  continue;
               }
            }
            possibleMoves.Add((rank, file));
         }
      }
      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
}
public class Bishop : PieceLogic
{
   public override PieceType Type => PieceType.Bishop;

   public Bishop(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : base(color, representation, position) 
   {
      Directions = new() { (-1, -1), (1, -1), (1, 1), (1, -1) };
   }

}
public class Queen : PieceLogic
{
   public override PieceType Type => PieceType.Queen;

   public Queen(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : base(color, representation, position) 
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (1, -1), (1, 1), (1, -1) };
   }

}
public class King : PieceLogic
{
   public override PieceType Type => PieceType.King;
   public bool CanCastleQueenside { get; set; }
   public bool CanCastleKingside { get; set; }
   public bool HasMoved { get; set; }

   public King(PieceColor color,
               GameObject representation,
               Coords position,
               bool canCastleQueenside = true,
               bool canCastleKingside = true,
               bool hasMoved = false)
               : base(color, representation, position)
   {
      this.CanCastleKingside = canCastleKingside;
      this.CanCastleQueenside = canCastleQueenside;
      this.HasMoved = hasMoved;
   }
   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<(int, int)>();
      var possibleAttacks = new List<(int, int)>();

      Directions = new() { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1)};

      foreach (var (rankOffset, fileOffset) in possibleMoves)
      {
         int rank = Position.Rank + rankOffset;
         int file = Position.File + fileOffset;
         PieceColor oppositeColor = Utilities.GetOppositeColor(this.Color);

         if (Utilities.IsWithinBounds(rank, file))
         {
            if (board.IsOccupied(rank, file))
            {
               if (!board.IsUnderAttackBy(rank, file, oppositeColor) && board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
               {
                  possibleMoves.Add((rank, file));
                  possibleAttacks.Add((rank, file));
               }
            } else if (!board.IsUnderAttackBy(rank, file, oppositeColor))
            {
               possibleMoves.Add((rank, file));
            }
         }
      }
      PossibleMoves = possibleMoves;
      PossibleAttacks = possibleAttacks;
   }
}
