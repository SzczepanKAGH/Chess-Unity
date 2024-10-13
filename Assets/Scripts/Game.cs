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
      board = SetupGame();
      board.CalculateAllMoves();
      gameState = GameState.Playing;
   }

   void Update()
   {
      
   }

   public Board SetupGame()
   {
      PieceRenderer.LoadSprites();
      var board = new Board(graphicalBoard);
      board.LogicalBoard = Board.InitializeBoard(board);

      return board;
   }
}
