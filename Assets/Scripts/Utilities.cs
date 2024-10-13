
using System.Collections.Generic;

public class Utilities
{
   public static bool IsLightSquare(int rankIndex, int fileIndex) => ((fileIndex + rankIndex) % 2 != 0);
  
   public static bool IsWithinBounds(int rankIndex, int fileIndex)
   {
      return rankIndex >= 0 && rankIndex < 8 && fileIndex >= 0 && fileIndex < 8;
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

   public int GetBoardIndex() => SquareIndex;

   public (int, int) AsTuple() => (Rank, File);
}

public struct GameData
{
   public int MoveNo;
   public int HalfmoveRule;
   public PieceColor ActivePlayer { get; set; }
   private Dictionary<PieceColor, Coords> EnPassantSquares { get; }

   public GameData(PieceColor activePlayer, int moveNo, int halfmoveRule)
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