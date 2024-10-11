
using System;
using System.Collections;
using System.Collections.Generic;

public class FenReader
{
   private static Dictionary<char, (PieceColor, PieceType)> fenNotationTranslator = new()
{
   {'k', (PieceColor.Black, PieceType.King)}, {'K', (PieceColor.White, PieceType.King)},
   {'p', (PieceColor.Black, PieceType.Pawn)}, {'P', (PieceColor.White, PieceType.Pawn)},
   {'r', (PieceColor.Black, PieceType.Rook)}, {'R', (PieceColor.White, PieceType.Rook)},
   {'q', (PieceColor.Black, PieceType.Queen)}, {'Q', (PieceColor.White, PieceType.Queen)},
   {'n', (PieceColor.Black, PieceType.Knight)}, {'N', (PieceColor.White, PieceType.Knight)},
   {'b', (PieceColor.Black, PieceType.Bishop)}, {'B', (PieceColor.White, PieceType.Bishop)},
};

   public static PieceLogic[,] ReadFEN(string Fen)
   {
      var newBoard = new PieceLogic[8, 8];
      int row = 7, col = 0;

      string[] fenFlags = Fen.Split(' ');

      string fenTurn = fenFlags[1];
      string fenCastling = fenFlags[2];
      string fenEnPassant = fenFlags[3];
      string fenHalfmove = fenFlags[4];
      string fenNoMove = fenFlags[5];

      foreach (char ch in fenFlags[0])
      {
         if (ch == '/')
         {
            row--;
            col = 0;
         }
         else if (char.IsDigit(ch))
         {
            col += (int)char.GetNumericValue(ch);
         }
         else if (fenNotationTranslator.ContainsKey(ch))
         {
            var coords = new Coords(row, col);
            var (color, type) = fenNotationTranslator[ch];
            PieceLogic piece = PieceFactory.CreatePiece(coords ,type, color);

            newBoard[row, col] = piece;
            col++;
         }
      }
      return newBoard;

   }
}
