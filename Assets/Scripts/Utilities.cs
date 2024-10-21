
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MoveType { whiteMove, blackMove, castling, check, capture, promotion }

public enum GameState { Playing, WhiteWon, BlackWon, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }

public class Utilities
{
   public static bool IsLightSquare(int rankIndex, int fileIndex) => ((fileIndex + rankIndex) % 2 != 0);
  
   public static bool IsWithinBounds(Coords moveTo)
   {
      return moveTo.Rank >= 0 && moveTo.Rank < 8 && moveTo.File >= 0 && moveTo.File < 8;
   }

   public static PieceColor GetOppositeColor(PieceColor color)
   {
      return (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
   }
}

public struct Coords
{
   public int Rank { get; }
   public int File { get; }

   public Coords(int rank, int file)
   {
      Rank = rank;
      File = file;
   }
   public Coords(int squareIndex)
   {
      Rank = squareIndex / 8;
      File = squareIndex % 8;
   }

   public static bool operator !=(Coords left, Coords right) => !left.Equals(right);

   public static bool operator ==(Coords left, Coords right) => left.Equals(right);

   public static Coords operator +(Coords coords, (int, int) delta)
   {
      return new(coords.Rank + delta.Item1, coords.File + delta.Item2);
   }

   public override bool Equals(object obj)
   {
      if (obj is Coords other)
      {
         return Rank == other.Rank && File == other.File;
      }
      return false;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(Rank, File);
   }

   public override string ToString() => $"(R{Rank}-F{File})";
}

public struct ChessGameData
{
   public int MoveNo;
   public int HalfmoveRule;
   public PieceColor ActivePlayer { get; set; }
   
   public MoveType LastMoveType { get; set; }

   public Coords EnPassantTargetSquare { get; set; }

   private static Coords noEnPassantSquare = new Coords(-1, -1);

   public ChessGameData(PieceColor activePlayer, int moveNo, int halfmoveRule)
   {
      EnPassantTargetSquare = new Coords(-1, -1);
      ActivePlayer = activePlayer;
      HalfmoveRule = halfmoveRule;
      MoveNo = moveNo;
      LastMoveType = MoveType.whiteMove;
   }

   public ChessGameData DeepCopy()
   {
      var newGameData = new ChessGameData
      {
         ActivePlayer = this.ActivePlayer,
         MoveNo = this.MoveNo,
         EnPassantTargetSquare = noEnPassantSquare,
      };

      return newGameData;
   }

   public void IncreaseHalfmoveCounter() => HalfmoveRule += 1;

   public void ResetHalfmoveCounter() => HalfmoveRule = 0;

   public void SetEnPassantSquare(Coords position) => EnPassantTargetSquare = position;

   public void ResetEnPassantSquare() => EnPassantTargetSquare = noEnPassantSquare;

   public Coords GetEnPassantSquare() => EnPassantTargetSquare;
}