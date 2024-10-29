//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;

//public enum PlayerType { Human, Computer }

//public interface IPlayer
//{
//}

//public abstract class Player
//{
//   protected bool canInteractChessboard;
//   protected PieceColor side;
//   public Board board;
//   public GraphicalBoard boardUI;

//   public Player(PieceColor playingSide, Board gameboard)
//   {
//      this.canInteractChessboard = true;
//      this.side = playingSide;
//      this.board = gameboard;
//      this.boardUI = board.BoardUI;
//   }

//   public virtual void MakeMove() { }
//   public virtual void FindBestMove() { }

//}

//public class HumanPlayer : Player
//{
//   public PieceLogic SelectedPiece;

//   public HumanPlayer(PieceColor playingSide, Board board) : base(playingSide, board)
//   {
//      this.canInteractChessboard = true;

//      SubscribeToPieceClickedEvents();
//      SubscribeToSquareClickedEvents();
//   }

//   public void SubscribeToSquareClickedEvents()
//   {
//      foreach (var square in board.BoardUI.squaresList)
//      {
//         var squareScript = square.GetComponent<SquareScript>();
//         squareScript.SquareClicked += HandleSquareClicked;
//      }
//   }

//   public void SubscribeToPieceClickedEvents()
//   {
//      foreach (var graphicalPiece in board.BoardUI.graphicalPiecesList)
//      {
//         var pieceRenderer = graphicalPiece.GetComponent<PieceRenderer>();
//         if (pieceRenderer != null) pieceRenderer.OnPieceClicked += HandlePieceClicked;
//      }
//   }

//   private void HandlePieceClicked(object sender, PieceClickedEventArgs e)
//   {
//      board.BoardUI.ClearHighlitedSquares();
//      PieceLogic clickedPiece = e.Piece;

//      if (clickedPiece.Color == board.GameData.ActivePlayer && board.PromotionPieceChosen)
//      {
//         SelectedPiece = e.Piece;
//         board.BoardUI.HighlightSquares(SelectedPiece.PossibleMoves, SelectedPiece.PossibleAttacks);
//      }
//      else if (SelectedPiece != null && SelectedPiece.PossibleAttacks.Contains(clickedPiece.Position))
//      {
//         UnsubscribeToPieceClicked(clickedPiece);
//         board.CaptureEnemyPiece(clickedPiece.Position, SelectedPiece);
//         board.GameData.ResetHalfmoveCounter();
//      }
//   }

//   private void HandleSquareClicked(object sender, SquareClickedEventArgs e)
//   {
//      Coords positionTo = e.SquarePosition;

//      if (SelectedPiece != null)
//      {
//         if (SelectedPiece.PossibleMoves.Contains(positionTo))
//         {
//            if (SelectedPiece is not Pawn) board.GameData.IncreaseHalfmoveCounter();
//            else board.GameData.ResetHalfmoveCounter(); HumanPromotePawn(positionTo, SelectedPiece);

//            bool madeEnPassant = false;
//            if (SelectedPiece is Pawn) madeEnPassant = board.HandleEnPassantCapture(positionTo, SelectedPiece);
//            board.MovePiece(positionTo, SelectedPiece);

//            if (madeEnPassant)
//            {
//               board.SetMoveType(MoveType.capture);
//               UnsubscribeToPieceClicked(board.GetPieceAtSquare(positionTo));
//            }
//         }
//         else board.BoardUI.ClearHighlitedSquares();
//      }
//   }

//   private void HumanPromotePawn(Coords moveTo, PieceLogic pawn)
//   {
//      int promotionRank = (side == PieceColor.White) ? 7 : 0;

//      if (moveTo.Rank == promotionRank)
//      {
//         Debug.Log("promotion");

//         HandlePawnPromotion(side, pawn);
//      }
//   }

//   private void HandlePawnPromotion(PieceColor promotingColor, PieceLogic pawnToPromote)
//   {
//      GameObject[] promotionMenu = boardUI.CreatePromotionMenu(promotingColor, pawnToPromote.Position);
//      int PawnRank = pawnToPromote.Position.Rank;
//      int PawnFile = pawnToPromote.Position.File;

//      board.LogicalBoard[PawnRank, PawnFile] = null;
//      boardUI.DestroyPieceGraphic(pawnToPromote.Position);

//      foreach (GameObject promotionCase in promotionMenu)
//      {
//         var promotionScript = promotionCase.GetComponent<PromotionScript>();
//         promotionScript.OnPromotionChosen += HandlePromotionPieceChosen;
//      }
//   }

//   private void HandlePromotionPieceChosen(object sender, PieceToPromoteChosen e)
//   {
//      PieceType pieceChosen = e.PieceChosen;
//      PieceColor pieceColor = e.PromotingSide;
//      Coords pieceCoords = e.PromotionCoords;

//      var newPiece = PieceFactory.CreatePiece(pieceCoords, pieceChosen, pieceColor, boardUI);
//      var newPieceGraphic = boardUI.graphicalPiecesList.Last();
//      //newPieceGraphic.GetComponent<PieceRenderer>().OnPieceClicked += HandlePieceClicked;

//      //UpdatePiecePosition(pieceCoords, newPiece);
//      board.LogicalBoard[pieceCoords.Rank, pieceCoords.File] = newPiece;
//      boardUI.DestroyPromotionMenu(HandlePromotionPieceChosen);
//      //.PromotionPieceChosen = true;
//   }

//   private void UnsubscribeToPieceClicked(PieceLogic pieceToUnsubscribe)
//   {
//      GameObject pieceForRemoval = boardUI.graphicalPiecesList.FirstOrDefault(piece =>
//      piece.GetComponent<PieceRenderer>().pieceLogic == pieceToUnsubscribe);

//      pieceForRemoval.GetComponent<PieceRenderer>().OnPieceClicked -= HandlePieceClicked;
//   }
//}

//public class ComputerPlayer : Player
//{
//   public Engine ComputerEngine;
//   public Move BestMove;

//   public ComputerPlayer(PieceColor playingSide, Board board) : base(playingSide, board)
//   {
//      this.canInteractChessboard = false;
//      this.ComputerEngine = new Engine(playingSide);
//   }

//   public override void MakeMove()
//   {
//      PieceLogic pieceToMove = board.GetPieceAtSquare(BestMove.PositionFrom);
//      bool isCapture = board.GetPieceAtSquare(BestMove.PositionTo) != null;

//      if (isCapture) board.CaptureEnemyPiece(BestMove.PositionTo, pieceToMove);
//      else board.MovePiece(BestMove.PositionTo, pieceToMove);
//   }

//   public override void FindBestMove()
//   {
//      ComputerEngine.GetAllMovesForColor(board);
//      BestMove = ComputerEngine.FindBestMove(board);
//   }

//   private void ComputerPromote()
//   {

//   }
//}
