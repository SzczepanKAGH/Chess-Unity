using System.Collections;
using UnityEngine;


public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }

public class Game : MonoBehaviour
{
   public GraphicalBoard graphicalBoard;
   public SoundManager soundManager;
   public GameState gameState;
   public Board board;
   
   void Start()
   {
      soundManager = FindAnyObjectByType<SoundManager>();
      board = new Board(graphicalBoard, soundManager);
      board.CalculateAllMoves();
      gameState = GameState.Playing;
   }

   void Update()
   {
      if (gameState != GameState.Playing)
      {
         //end game
      }

      if (board.MoveHasBeenMade) StartCoroutine(HandleNewTurn()); 

   }

   public IEnumerator HandleNewTurn()
   {
      //Debug.Log("NewTurn");
      board.MoveHasBeenMade = false;
      
      board.CheckForPromotion();

      yield return new WaitUntil(() => board.PromotionPieceChosen == true);

      board.CalculateAllMoves();
      board.ChangeActivePlayer();
      PlayMoveSound(board);


      board.GameData.MoveNo += 1;
      board.CheckForHalfmove();
      board.CheckForRepetition();
      Debug.Log(board.GameData.EnPassantTargetSquare);
      board.GameData.ResetEnPassantSquare();
   }

   private void PlayMoveSound(Board board)
   {
      MoveType moveType = board.GameData.LastMoveType;
      PieceColor activePlayer = board.GameData.ActivePlayer;
      if (board.IsInCheck(board.GameData.ActivePlayer)) moveType = MoveType.check;

      switch (moveType)
      {
         case MoveType.whiteMove:
            soundManager.PlayWhiteMoveSound();
            break;

         case MoveType.blackMove:
            soundManager.PlayBlackMoveSound();
            break;

         case MoveType.castling:
            soundManager.PlayCastlingSound();
            break;

         case MoveType.capture:
            soundManager.PlayCaptureSound();
            break;

         case MoveType.check:
            soundManager.PlayCheckSound();
            break;

         //case MoveType.promotion:
         //   soundManager.Play
      }
   }
}
