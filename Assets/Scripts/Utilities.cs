
using System.Collections.Generic;

public class Utilities
{
   public static bool IsLightSquare(int rankIndex, int fileIndex) => ((fileIndex + rankIndex) % 2 != 0);
  
   public static bool IsWithinBounds(Coords moveTo)
   {
      return moveTo.Rank >= 0 && moveTo.Rank < 8 && moveTo.File >= 0 && moveTo.File < 8;
   }

   public static PieceColor GetOppositeColor(PieceColor color)
   {
      return (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
   }
}

public struct Coords
{
   public int Rank { get; }
   public int File { get; }
   private readonly int SquareIndex;

   public Coords(int rank, int file)
   {
      Rank = rank;
      File = file;
      SquareIndex = rank * 8 + file;
   }
   public Coords(int squareIndex)
   {
      Rank = squareIndex / 8;
      File = squareIndex % 8;
      SquareIndex = squareIndex;
   }

  

   public static Coords operator +(Coords coords, (int, int) delta) => new(coords.Rank + delta.Item1, coords.File + delta.Item2);

   public override string ToString() => $"(R{Rank}-F{File})";

   public (int, int) AsTuple() => (Rank, File);
}

public struct ChessGameData
{
   public int MoveNo;
   public int HalfmoveRule;
   public PieceColor ActivePlayer { get; set; }
   private Dictionary<PieceColor, Coords> EnPassantSquares { get; }

   public ChessGameData(PieceColor activePlayer, int moveNo, int halfmoveRule)
   {
      EnPassantSquares = new Dictionary<PieceColor, Coords>();
      ActivePlayer = activePlayer;
      HalfmoveRule = halfmoveRule;
      MoveNo = moveNo;
   }

   public void SetEnPassanSquares(PieceColor enPassant, Coords position)
   {
      EnPassantSquares[enPassant] = position;
   }
}