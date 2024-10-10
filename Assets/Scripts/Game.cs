using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
   public GraphicalBoard graphicalBoard;
   public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial}
   
   void Start()
   {
      PieceRenderer.LoadSprites();
      var board = new Board(graphicalBoard);
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space)) {
         List<int> list = new List<int>() {0};
         graphicalBoard.HighlightSquares(list);
      }
   }
}
