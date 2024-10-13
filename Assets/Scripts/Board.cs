
using UnityEngine;

public class Board
{
   public GraphicalBoard GraphicalBoard;
   public PieceLogic[,] LogicalBoard;
   public PieceLogic SelcectedPiece;
   public King WhiteKing;
   public King BlackKing;
   public bool MoveHasBeenMade;

   public Board(GraphicalBoard graphicalBoard)
   {
      this.GraphicalBoard = graphicalBoard;
      GraphicalBoard.CreateGraphicalBoard();
      MoveHasBeenMade= false;

      SubscribeToSquareClickedEvents();
   }

   public static PieceLogic[,] InitializeBoard(Board board) => FenReader.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board);

   public bool IsOccupied(int rankIndex, int fileIndex) => LogicalBoard[rankIndex, fileIndex] != null;

   public PieceLogic GetPieceAtSquare(int rankIndex, int fileIndex) => LogicalBoard[rankIndex, fileIndex];

   public PieceLogic GetPieceAtSquare(Coords pieceCoords) => LogicalBoard[pieceCoords.Rank, pieceCoords.File];

   public bool IsUnderAttackBy(int rankIndex, int fileIndex, PieceColor color) { return true; }

   public bool IsMoveLegal(PieceLogic piece, Coords move) => (piece.PossibleMoves.Contains((move.Rank, move.File)));

   public void CalculateAllMoves()
   {
      for (int i = 0; i < 8; ++i)
      {
         for (int j = 0; j < 8; j++)
         {
            PieceLogic piece = GetPieceAtSquare(i, j);
            piece?.GetPossibleMoves();
         }
      }
   }

   public void MovePiece(Coords positionFrom, Coords positionTo)
   {
      PieceLogic pieceToMove = GetPieceAtSquare(positionFrom);

      if (IsMoveLegal(pieceToMove, positionTo))
      {
         GraphicalBoard.ClearHighlitedSquares();

         LogicalBoard[positionFrom.Rank, positionFrom.File] = null;
         LogicalBoard[positionTo.Rank, positionTo.File] = pieceToMove;

         pieceToMove.Move(positionTo);
      }
   }

   public void SubscribeToSquareClickedEvents()
   {
      foreach (var square in GraphicalBoard.squaresList) 
      {
         var squareScript = square.GetComponent<SquareScript>();
         squareScript.SquareClicked += HandleSquareClicked;
      }
   }

   private void HandleSquareClicked(object sender , SquareClickedEventArgs e)
   {
      if (SelcectedPiece != null) 
      {
         Coords positionTo = e.SquarePosition;
         MovePiece(SelcectedPiece.Position, positionTo);

         SelcectedPiece = null;
         MoveHasBeenMade = true;

      }
   }
}
