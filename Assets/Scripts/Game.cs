using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
   public GraphicalBoard graphicalBoard;
   public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }
   
   void Start()
   {
      PieceRenderer.LoadSprites();
      var board = new Board(graphicalBoard); 
      //board.GetPieceAtSquare(4, 3).GetPossibleMoves(board);
      //foreach ((int, int) move in board.GetPieceAtSquare(4, 3).PossibleMoves)
      //{
      //   Debug.Log($"{move.Item1} {move.Item2}");
      //}
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space)) {
         Coords newcord = new Coords(4, 3);
         List<int> list = new List<int>() {newcord.GetBoardIndex()};
         graphicalBoard.HighlightSquares(list);
      }
   }
}
