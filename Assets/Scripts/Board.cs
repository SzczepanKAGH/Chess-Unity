
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Board
{
   public GraphicalBoard GraphicalBoard;
   public PieceLogic[,] LogicalBoard;
   public PieceLogic SelcectedPiece;
   public ChessGameData GameData;
   public King WhiteKing;
   public King BlackKing;
   public bool MoveHasBeenMade;

   public Board(GraphicalBoard graphicalBoard)
   {
      this.GraphicalBoard = graphicalBoard;
      GraphicalBoard.CreateGraphicalBoard();
      PieceRenderer.LoadSprites();
      LogicalBoard = InitializeBoard(this);
      MoveHasBeenMade = false;

      SubscribeToSquareClickedEvents();
      SubscribeToPieceClickedEvents();
   }

   public Board() { LogicalBoard = new PieceLogic[8, 8]; }

   public void ChangeActivePlayer()
   {
      GameData.ActivePlayer = Utilities.GetOppositeColor(GameData.ActivePlayer);
   }

   public Board Copy()
   {
      Board newBoard = new Board();

      for (int row = 0; row < 8; row++)
      {
         for (int col = 0; col < 8; col++)
         {
            newBoard.LogicalBoard[row, col] = LogicalBoard[row, col]?.DeepCopy();
         }
      }

      return newBoard;
   }

   public List<PieceLogic> GetPiecesOfColor(PieceColor color)
   {
      var pieces = new List<PieceLogic>();
      
      foreach (PieceLogic piece in LogicalBoard) if (piece?.Color == color) pieces.Add(piece);

      return pieces;
   }

   public void CheckForHalfmove()
   {
      //TODO
   }

   public void CheckForRepetition()
   {
      //TODO
   }

   private void MovePiece(Coords positionTo, PieceLogic pieceToMove)
   {
      Coords positionFrom = pieceToMove.Position;

      SelcectedPiece = null;
      MoveHasBeenMade = true;

      GraphicalBoard.ClearHighlitedSquares();
      UpdatePiecePosition(positionTo, pieceToMove);
   }

   private void HandleCastling(Coords positionTo, PieceLogic pieceToMove)
   {
      //if 
   }

   private void UpdatePiecePosition(Coords positionTo, PieceLogic pieceToMove, bool isPieceCopy = false)
   {
      isPieceCopy = (GraphicalBoard == null); // Virtual board copies don't have graphicalboard attached

      LogicalBoard[pieceToMove.Position.Rank, pieceToMove.Position.File] = null;
      LogicalBoard[positionTo.Rank, positionTo.File] = pieceToMove;

      //Debug.Log($"Piece moved from {pieceToMove.Position} to {positionTo}");
      pieceToMove.Move(positionTo, isPieceCopy);
   }

   public bool IsLegal(Coords moveFrom, Coords moveTo)
   {
      Board boardCopy = Copy();

      PieceLogic pieceCopy = boardCopy.GetPieceAtSquare(moveFrom);
      PieceColor playerColor = pieceCopy.Color;

      boardCopy.UpdatePiecePosition(moveTo, pieceCopy, isPieceCopy: true);
      boardCopy.CalculateAllMoves(isBoardCopy: true);

      return !boardCopy.IsInCheck(playerColor);
   }

   public void CalculateAllMoves(bool isBoardCopy = false)
   {
      if (!isBoardCopy) foreach (var piece in LogicalBoard) piece?.GetLegalMoves(this);

      else foreach (var piece in LogicalBoard) piece?.GetPossibleMoves(this);
   }

   public bool IsInCheck(PieceColor kingColor)
   {
      List<PieceLogic> enemyPieces = GetPiecesOfColor(Utilities.GetOppositeColor(kingColor));

      return enemyPieces.Any(piece => { return piece.CanCaptureEnemyKing(this); });
   }

   public void SubscribeToSquareClickedEvents()
   {
      foreach (var square in GraphicalBoard.squaresList)
      {
         var squareScript = square.GetComponent<SquareScript>();
         squareScript.SquareClicked += HandleSquareClicked;
      }
   }

   public void SubscribeToPieceClickedEvents()
   {
      foreach (var graphicalPiece in GraphicalBoard.graphicalPiecesList)
      {
         var pieceRenderer = graphicalPiece.GetComponent<PieceRenderer>();
         if (pieceRenderer != null) pieceRenderer.OnPieceClicked += HandlePieceClicked;
      }
   }

   private void HandlePieceClicked(object sender, PieceClickedEventArgs e)
   {
      //Debug.Log(e.Piece.Type);
      GraphicalBoard.ClearHighlitedSquares();
      PieceLogic clickedPiece = e.Piece;

      if (clickedPiece.Color == GameData.ActivePlayer)
      {
         SelcectedPiece = e.Piece;
         GraphicalBoard.HighlightSquares(SelcectedPiece.PossibleMoves, SelcectedPiece.PossibleAttacks);
      }
   }

   private void HandleSquareClicked(object sender, SquareClickedEventArgs e)
   {
      Coords positionTo = e.SquarePosition;

      if (SelcectedPiece != null)
      {
         if (SelcectedPiece.PossibleMoves.Contains(positionTo)) MovePiece(positionTo, SelcectedPiece);

         else GraphicalBoard.ClearHighlitedSquares();
      }
   }

   public static PieceLogic[,] InitializeBoard(Board board) => FenReader.ReadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board);

   public bool IsOccupied(Coords position) => LogicalBoard[position.Rank, position.File] != null;

   public PieceLogic GetPieceAtSquare(Coords pieceCoords) => LogicalBoard[pieceCoords.Rank, pieceCoords.File];

   //public bool IsMoveLegal(PieceLogic piece, Coords move) => (piece.PossibleMoves.Contains(move));
}
