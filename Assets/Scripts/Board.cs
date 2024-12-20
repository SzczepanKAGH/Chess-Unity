
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBoard
{
   void ChangeActivePlayer();
   Board Copy();
   void SetMoveType(MoveType type);
   List<PieceLogic> GetPiecesOfColor(PieceColor color);
   bool CheckForHalfmove();
   bool DetectStalemate();
   bool DetectCheckmate();
   bool CheckForPromotion();
   void CalculateAllMoves(bool isBoardCopy = false);
   bool IsInCheck(PieceColor kingColor);
   bool IsLegal(Coords moveFrom, Coords moveTo);
   bool IsOccupied(Coords position);
   PieceLogic GetPieceAtSquare(Coords pieceCoords);
}

public class Board : IBoard
{
   public ChessGameData GameData;
   public GraphicalBoard BoardUI { get; private set; }
   public PieceLogic[,] LogicalBoard { get; private set; }
   public PieceLogic SelectedPiece { get; private set; }
   public bool MoveHasBeenMade { get; private set; }
   public bool PromotionPieceChosen { get; private set; } = true;


   public Board(GraphicalBoard graphicalBoard, SoundManager soundManager)
   {
      this.BoardUI = graphicalBoard;
      BoardUI.CreateGraphicalBoard();
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
      Board newBoard = new();

      for (int row = 0; row < 8; row++)
      {
         for (int col = 0; col < 8; col++)
         {
            newBoard.LogicalBoard[row, col] = LogicalBoard[row, col]?.DeepCopy();
         }
      }

      newBoard.GameData = GameData.DeepCopy();

      return newBoard;
   }

   public List<PieceLogic> GetPiecesOfColor(PieceColor color)
   {
      var pieces = new List<PieceLogic>();
      
      foreach (PieceLogic piece in LogicalBoard) if (piece?.Color == color) pieces.Add(piece);

      return pieces;
   }

   public bool CheckForHalfmove()
   {
      if (GameData.HalfmoveRule >= 100) return true;
      else return false;
   }

   //public bool CheckForRepetition()
   //{
   //   return false;
   //}

   public bool DetectStalemate()
   {
      if (GetPiecesOfColor(GameData.ActivePlayer).All(piece => !piece.PossibleMoves.Any()) &&
         !IsInCheck(GameData.ActivePlayer)) return true;

      else return false;
   }

   public bool DetectCheckmate()
   {
      if (GetPiecesOfColor(GameData.ActivePlayer).All(piece => !piece.PossibleMoves.Any()) &&
         IsInCheck(GameData.ActivePlayer)) return true;

      else return false;
   }

   private void CaptureEnemyPiece(Coords enemyPiecePosition, PieceLogic alliedPiece)
   {      
      BoardUI.DestroyPieceGraphic(enemyPiecePosition, HandlePieceClicked);
      MovePiece(enemyPiecePosition, alliedPiece);

      SetMoveType(MoveType.capture);
   }

   private void MovePiece(Coords positionTo, PieceLogic pieceToMove)
   {
      Coords positionFrom = pieceToMove.Position;
      MoveType moveType = (GameData.ActivePlayer == PieceColor.White) ? MoveType.whiteMove : MoveType.blackMove;
      SetMoveType (moveType);

      if (pieceToMove is King) HandleCastling(positionTo, pieceToMove);
      if (pieceToMove is Pawn) CheckForEnPassant(positionTo, pieceToMove);

      SelectedPiece = null;
      MoveHasBeenMade = true;

      BoardUI.ClearHighlitedSquares();
      UpdatePiecePosition(positionTo, pieceToMove);
   }

   private void CheckForEnPassant(Coords positionTo, PieceLogic pieceToMove)
   {
      bool isTwoSquaresMove = Math.Abs(pieceToMove.Position.Rank - positionTo.Rank) == 2;
      if (isTwoSquaresMove)
      {
         int enPassantRank = (pieceToMove.Position.Rank + positionTo.Rank) / 2;
         GameData.SetEnPassantSquare(new Coords(enPassantRank, positionTo.File));
      }
   }

   private bool HandleEnPassantCapture(Coords positionForAttack, PieceLogic pieceToMove)
   {
      bool isDiagonalPawnMove = (pieceToMove.Position.File != positionForAttack.File);

      if (isDiagonalPawnMove)
      {
         Coords enemyPawnPosition = new(pieceToMove.Position.Rank, positionForAttack.File);
         LogicalBoard[enemyPawnPosition.Rank, enemyPawnPosition.File] = null;
         BoardUI.DestroyPieceGraphic(enemyPawnPosition, HandlePieceClicked);
         return true;
      }
      return false;
   }

   private void HandleCastling(Coords positionTo, PieceLogic pieceToMove)
   {
      if (pieceToMove.CanCastleQueenSide(this) && positionTo.File == 2)
      {
         Coords rookPosition = new(pieceToMove.Position.Rank, 0);
         PieceLogic rookToCastle = GetPieceAtSquare(rookPosition);

         SetMoveType(MoveType.castling);
         UpdatePiecePosition(rookPosition + (0, 3), rookToCastle);
      } 
      else if (pieceToMove.CanCastleKingSide(this) && positionTo.File == 6)
      {
         Coords rookPosition = new(pieceToMove.Position.Rank, 7);
         PieceLogic rookToCastle = GetPieceAtSquare(rookPosition);

         SetMoveType(MoveType.castling);
         UpdatePiecePosition(rookPosition + (0, -2), rookToCastle);
      } 
   }

   public bool CheckForPromotion()
   {
      bool isWhitePromoting = CheckForPromotionForColor(7, PieceColor.White);
      bool isBlackPromoting = CheckForPromotionForColor(0, PieceColor.Black);

      return isWhitePromoting || isBlackPromoting;
   }

   private bool CheckForPromotionForColor(int rankIndex, PieceColor color)
   {
      for (int fileIdx = 0; fileIdx < 8; fileIdx++)
      {
         Coords promotingSquare = new(rankIndex, fileIdx);
         PieceLogic supposedPawn = GetPieceAtSquare(promotingSquare);
         if (supposedPawn is Pawn)
         {
            PromotionPieceChosen = false;
            HandlePawnPromotion(color, supposedPawn);

            return true;
         }
      }
      return false;
   }

   private void HandlePawnPromotion(PieceColor promotingColor, PieceLogic pawnToPromote)
   {
      GameObject[] promotionMenu = BoardUI.CreatePromotionMenu(promotingColor, pawnToPromote.Position);
      int PawnRank = pawnToPromote.Position.Rank;
      int PawnFile = pawnToPromote.Position.File;

      LogicalBoard[PawnRank, PawnFile] = null;
      BoardUI.DestroyPieceGraphic(pawnToPromote.Position, HandlePieceClicked);

      foreach (GameObject promotionCase in promotionMenu)
      {
         var promotionScript = promotionCase.GetComponent<PromotionScript>();
         promotionScript.OnPromotionChosen += HandlePromotionPieceChosen;
      }
   }

   private void HandlePromotionPieceChosen(object sender, PieceToPromoteChosen e)
   {
      PieceType pieceChosen = e.PieceChosen;
      PieceColor pieceColor = e.PromotingSide;
      Coords pieceCoords = e.PromotionCoords;

      var newPiece = PieceFactory.CreatePiece(pieceCoords, pieceChosen, pieceColor, BoardUI);
      var newPieceGraphic = BoardUI.graphicalPiecesList.Last();
      newPieceGraphic.GetComponent<PieceRenderer>().OnPieceClicked += HandlePieceClicked;

      //UpdatePiecePosition(pieceCoords, newPiece);
      LogicalBoard[pieceCoords.Rank, pieceCoords.File] = newPiece;
      BoardUI.DestroyPromotionMenu(HandlePromotionPieceChosen);
      PromotionPieceChosen = true;
   }

   private void UpdatePiecePosition(Coords positionTo, PieceLogic pieceToMove, bool isPieceCopy = false)
   {
      isPieceCopy = (BoardUI == null); // Virtual board copies don't have graphicalboard attached

      LogicalBoard[pieceToMove.Position.Rank, pieceToMove.Position.File] = null;
      LogicalBoard[positionTo.Rank, positionTo.File] = pieceToMove;

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
      foreach (var square in BoardUI.squaresList)
      {
         var squareScript = square.GetComponent<SquareScript>();
         squareScript.SquareClicked += HandleSquareClicked;
      }
   }

   public void SubscribeToPieceClickedEvents()
   {
      foreach (var graphicalPiece in BoardUI.graphicalPiecesList)
      {
         var pieceRenderer = graphicalPiece.GetComponent<PieceRenderer>();
         if (pieceRenderer != null) pieceRenderer.OnPieceClicked += HandlePieceClicked;
      }
   }

   private void HandlePieceClicked(object sender, PieceClickedEventArgs e)
   {
      BoardUI.ClearHighlitedSquares();
      PieceLogic clickedPiece = e.Piece;

      if (clickedPiece.Color == GameData.ActivePlayer && PromotionPieceChosen)
      {
         SelectedPiece = e.Piece;
         BoardUI.HighlightSquares(SelectedPiece.PossibleMoves, SelectedPiece.PossibleAttacks);
      }
      else if (SelectedPiece != null && SelectedPiece.PossibleAttacks.Contains(clickedPiece.Position))
      {
         CaptureEnemyPiece(clickedPiece.Position, SelectedPiece);
         GameData.ResetHalfmoveCounter();
      }
   }

   private void HandleSquareClicked(object sender, SquareClickedEventArgs e)
   {
      Coords positionTo = e.SquarePosition;

      if (SelectedPiece != null)
      {
         if (SelectedPiece.PossibleMoves.Contains(positionTo))
         {
            if (SelectedPiece is not Pawn) GameData.IncreaseHalfmoveCounter();
            else GameData.ResetHalfmoveCounter();

            bool madeEnPassant = false;
            if (SelectedPiece is Pawn) madeEnPassant = HandleEnPassantCapture(positionTo, SelectedPiece);
            MovePiece(positionTo, SelectedPiece);

            if (madeEnPassant) SetMoveType(MoveType.capture);
         }
         else BoardUI.ClearHighlitedSquares();
      }
   }

   public void AcknowledgeNewMove()
   {
      MoveHasBeenMade = false;
      GameData.MoveNo += 1;
   }

   private static string defaultStartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

   public static PieceLogic[,] InitializeBoard(Board board) => FenReader.ReadFEN(defaultStartingPosition, board);

   public void SetMoveType(MoveType type) => GameData.LastMoveType = type;

   public bool IsOccupied(Coords position) => LogicalBoard[position.Rank, position.File] != null;

   public PieceLogic GetPieceAtSquare(Coords pieceCoords) => LogicalBoard[pieceCoords.Rank, pieceCoords.File];
}
