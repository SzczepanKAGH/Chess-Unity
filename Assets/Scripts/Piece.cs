using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;


public interface IPieceLogic
{ 
   public PieceType Type { get; }
   public PieceColor Color { get; }
   public Coords Position { get; }
   public GameObject GraphicalRepresentation { get; }
   public void GetPossibleMoves(Board board);

}
public abstract class PieceLogic : IPieceLogic
{
   public Coords Position { get; }
   public abstract PieceType Type { get; }
   public PieceColor Color { get; }
   public GameObject GraphicalRepresentation { get; }
   public List<int> PossibleMoves { get; set; }

   public PieceLogic(PieceColor color, GameObject representation, Coords position)
   {
      this.GraphicalRepresentation = representation;
      this.Color = color;
      this.Position = position;
   }
   public abstract void GetPossibleMoves(Board board);

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
      var possibleMoves = new List<int>();
      PossibleMoves = possibleMoves;
   }

}

public class Rook : PieceLogic
{
   public override PieceType Type => PieceType.Rook;
   public bool HasMoved { get; set; }

   public List<(int, int)> Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1) };

   public Rook(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : base(color, representation, position)
   {
      this.HasMoved = hasMoved;
   }
   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<int>();

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         int rank = Position.Rank;
         int file = Position.File;

         while (Utilities.IsWithinBounds(rank, file))
         {
            rank += rankOffset;
            file += fileOffset;

            if (board.IsOccupied(rank, file))
            {

            }
         }
      }
      PossibleMoves = possibleMoves;
   }

}

/* == KNIGHT ==*/
public class Knight : PieceLogic
{
   public override PieceType Type => PieceType.Knight;

   public Knight(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : 
      base(color, representation, position) { }

   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<int>();
      PossibleMoves = possibleMoves;
   }
}
public class Bishop : PieceLogic
{
   public override PieceType Type => PieceType.Bishop;
   public List<(int, int)> Directions = new() { (-1, -1), (1, -1), (1, 1), (1, -1) };

   public Bishop(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : 
      base(color, representation, position) { }

   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<int>();
      PossibleMoves = possibleMoves;
   }
}
public class Queen : PieceLogic
{
   public override PieceType Type => PieceType.Queen;
   public List<(int, int)> Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (1, -1), (1, 1), (1, -1) };

   public Queen(PieceColor color, GameObject representation, Coords position, bool hasMoved = false) : 
      base(color, representation, position) { }

   public override void GetPossibleMoves(Board board)
   {
      var possibleMoves = new List<int>();
      PossibleMoves = possibleMoves;
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
      var possibleMoves = new List<int>();
      PossibleMoves = possibleMoves;
   }
}
