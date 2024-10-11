
public class Board
{
   private GraphicalBoard graphicalBoard;
   public PieceLogic[,] logicalBoard;
   public King whiteKing;
   public King blackKing;

   public Board(GraphicalBoard graphicalBoard)
   {
      graphicalBoard.CreateGraphicalBoard();
      logicalBoard = InitializeBoard();
   }

   public PieceLogic[,] InitializeBoard() => FenReader.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

   public bool IsOccupied(int rankIndex, int fileIndex) => logicalBoard[rankIndex, fileIndex] != null;

   public PieceLogic GetPieceAtSquare(int rankIndex, int fileIndex) => logicalBoard[rankIndex, fileIndex];

   public bool IsUnderAttackBy(int rankIndex, int fileIndex, PieceColor color) { return true; }

   //public MovePiece





}
