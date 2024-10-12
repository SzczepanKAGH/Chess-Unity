using JetBrains.Annotations;
using System.Linq;
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
   public void GetPossibleMoves();
   public bool IsColor(PieceColor color);
}

public abstract class PieceLogic : IPieceLogic
{
   public Coords Position { get; }
   public abstract PieceType Type { get; }
   public PieceColor Color { get; }
   public PieceRenderer Renderer { get; }
   public GameObject GraphicalRepresentation { get; }
   public List<(int, int)> PossibleMoves { get; set; }
   public List<(int, int)> PossibleAttacks { get; set; }

   protected List<(int, int)> Directions = new List<(int, int)>();

   protected Board Board { get; }

   public PieceLogic(PieceColor color, GameObject representation, Coords position, Board board)
   {
      this.GraphicalRepresentation = representation;
      this.Renderer = GraphicalRepresentation.GetComponent<PieceRenderer>();
      this.Color = color;
      this.Position = position;
      this.Board = board;
   }

   public virtual void GetPossibleMoves()
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
            if (Board.IsOccupied(rank, file))
            {
               Debug.Log($"Square {rank}, {file} is occupied");
               if (Board.GetPieceAtSquare(rank, file).IsColor(oppositeColor))
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

public class Rook : PieceLogic
{
   public override PieceType Type => PieceType.Rook;
   public bool HasMoved { get; set; }

   public Rook(PieceColor color, GameObject representation, Coords position, Board board, bool hasMoved = false)
      : base(color, representation, position, board)
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1) };
      this.HasMoved = hasMoved;
   }
}

public class Bishop : PieceLogic
{
   public override PieceType Type => PieceType.Bishop;

   public Bishop(PieceColor color, GameObject representation, Coords position, Board board, bool hasMoved = false)
      : base(color, representation, position, board) 
   {
      Directions = new() { (-1, -1), (1, -1), (1, 1), (1, -1) };
   }

}

public class Queen : PieceLogic
{
   public override PieceType Type => PieceType.Queen;

   public Queen(PieceColor color, GameObject representation, Coords position, Board board, bool hasMoved = false)
      : base(color, representation, position, board) 
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (1, -1), (1, 1), (1, -1) };
   }

}


