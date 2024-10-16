using System;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }

public class Game : MonoBehaviour
{
   public GraphicalBoard graphicalBoard;
   public GameState gameState;
   public Board board;
   
   void Start()
   {
      board = new Board(graphicalBoard);
      board.CalculateAllMoves();
      gameState = GameState.Playing;
   }

   void Update()
   {
      if (gameState != GameState.Playing)
      {
         //end game
      }

      if (board.MoveHasBeenMade) HandleNewTurn(); 

   }

   public void HandleNewTurn()
   {
      //Debug.Log("NewTurn");
      board.MoveHasBeenMade = false;
      
      board.CalculateAllMoves();
      board.ChangeActivePlayer();

      board.GameData.MoveNo += 1;
      board.CheckForHalfmove();
      board.CheckForRepetition();
   }
}
