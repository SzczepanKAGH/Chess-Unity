
public class Utilities
{
   public static bool IsLightSquare(int rankIndex, int fileIndex) => ((fileIndex + rankIndex) % 2 != 0);
  
   public static bool IsWithinBounds(int rankIndex, int fileIndex)
   {
      return rankIndex >= 0 && rankIndex < 8 && fileIndex >= 0 && fileIndex < 8;
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


   public override string ToString() => $"(R{Rank}-F{File})";

   public int GetBoardIndex() => SquareIndex;
}