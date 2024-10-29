using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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
      board.AcknowledgeNewMove();      
      board.CheckForPromotion();

      yield return new WaitUntil(() => board.PromotionPieceChosen == true);

      board.CalculateAllMoves();
      board.ChangeActivePlayer();

      PlayMoveSound();

      VerifyGameState();

      board.GameData.ResetEnPassantSquare();
   }

   private void DisplayEndText()
   {
      string finalText = gameState switch
      {
         GameState.Repetition => "Draw by repetition",
         GameState.Stalemate => "Draw because of stalemate",
         GameState.InsufficientMaterial => "Draw because of insufficient material",
         _ => "Checkmate"
      };

      soundManager.PlayGameEndSound();
      gameStateIndicator.SetText(finalText);
      textDisplayed = true;
   }

   private void VerifyGameState()
   {

      if (board.CheckForHalfmove())
         gameState = GameState.FiftyMoveRule;

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

   public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

}
