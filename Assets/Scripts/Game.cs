using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }

public class Game : MonoBehaviour
{
   public TextMeshProUGUI gameStateIndicator;
   public GraphicalBoard graphicalBoard;
   public SoundManager soundManager;
   public GameState gameState;
   public Board board;
   public bool textDisplayed;

   void Start()
   {
      soundManager = FindAnyObjectByType<SoundManager>();
      board = new Board(graphicalBoard, soundManager);
      board.CalculateAllMoves();
      textDisplayed = false;
      gameState = GameState.Playing;
   }

   void Update()
   {
      if (gameState != GameState.Playing)
      {
         //end game

         if (!textDisplayed) DisplayEndText();

         if (Input.GetKeyDown(KeyCode.Space)) RestartGame();
      }

      if (board.MoveHasBeenMade) StartCoroutine(HandleNewTurn()); 

   }

   public IEnumerator HandleNewTurn()
   {
      board.MoveHasBeenMade = false;
      
      board.CheckForPromotion();

      yield return new WaitUntil(() => board.PromotionPieceChosen == true);

      board.CalculateAllMoves();
      board.ChangeActivePlayer();

      PlayMoveSound();

      VerifyGameState();

      board.GameData.ResetEnPassantSquare();
   }

   public void RestartGame()
   {
      // Za³aduj ponownie bie¿¹c¹ scenê
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   private void DisplayEndText()
   {
      string finalText = "Game is over, ";

      switch (gameState)
      {
         case GameState.WhiteWon:
            finalText += "white won by checkmate";
            break;

         case GameState.BlackWon:
            finalText += "black won by checkmate";
            break;

         case GameState.Stalemate:
            finalText += "it's a draw by stalemate";
            break;

         case GameState.FiftyMoveRule:
            finalText += "it's a draw by no progress in position for 50 moves";
            break;

         case GameState.Repetition:
            finalText += "it's a draw by threefold repetition";
            break;

         case GameState.InsufficientMaterial:
            finalText += "it's a draw because of insufficient material for checkmate";
            break;
      }
      finalText += ", press space to restart.";

      soundManager.PlayGameEndSound();
      gameStateIndicator.SetText(finalText);
      textDisplayed = true;
   }

   private void VerifyGameState()
   {
      board.GameData.MoveNo += 1;

      if (board.CheckForHalfmove())
         gameState = GameState.FiftyMoveRule;

      else if (board.CheckForRepetition())
         gameState = GameState.Repetition;

      else if (board.DetectCheckmate())
         gameState = board.GameData.ActivePlayer == PieceColor.White ? GameState.BlackWon : GameState.WhiteWon;

      else if (board.DetectStalemate())
         gameState = GameState.Stalemate;

      else
         gameState = GameState.Playing;
   }

   private void PlayMoveSound()
   {
      MoveType moveType = board.GameData.LastMoveType;
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
      }
   }
}
