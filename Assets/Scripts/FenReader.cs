
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

   public static PieceLogic[,] ReadFEN(string Fen, Board board)
   {
      var newLogicalBoard = new PieceLogic[8, 8];
      int row = 7, col = 0;

      string[] fenFlags = Fen.Split(' ');

      char[] fenPieceSetup = fenFlags[0].ToCharArray();
      char fenActivePlayer = Convert.ToChar(fenFlags[1]);
      string fenCastling = fenFlags[2];
      string fenEnPassant = fenFlags[3];
      int fenHalfmove = Int32.Parse(fenFlags[4]);
      int fenNoMove = Int32.Parse(fenFlags[5]);

      foreach (char ch in fenPieceSetup)
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
            PieceLogic piece = PieceFactory.CreatePiece(coords ,type, color, board.BoardUI);

            newLogicalBoard[row, col] = piece;
            col++;
         }
      }

      CheckForCastlingRights(newLogicalBoard, fenCastling);

      PieceColor activePlayer = (fenActivePlayer == 'w') ? PieceColor.White : PieceColor.Black;

      ChessGameData newGameData = new(activePlayer, fenNoMove, fenHalfmove);

      board.GameData = newGameData;

      return newLogicalBoard;
   }

   private static void CheckForEnPassantSquares(string fenEnPassant, ChessGameData gameData)
   {
      if (fenEnPassant != "-") 
      {
         char[] enPassantNotation = fenEnPassant.ToCharArray();

         int enPassantFile = enPassantNotation[0] - 'a' + 1; // Converts letters to number eg.
                               // a -> 1, b -> 2 using default ASCII letter to int conversion.

         int enPassantRank = Convert.ToInt32(enPassantNotation[1]);

         gameData.SetEnPassantSquare(new Coords(enPassantRank, enPassantFile));  
      }
   }

   private static void CheckForCastlingRights(PieceLogic[,] logicalBoard, string castlingString)
   {
      char[] castlingRights = castlingString.ToCharArray();

      // Dictionary consist of starting rook positions and castling rights corresponding to
      // each eg. => 'Q' white queen side rook 
      Dictionary<(int, int), char> basicRookPositions = new Dictionary<(int, int), char>() 
      { 
         { (0, 0), 'Q' }, { (0, 7), 'K' }, { (7, 0), 'q' }, { (7, 7), 'k' }
      };

      foreach (var (rookRank, rookFile) in basicRookPositions.Keys) 
      {
         if (logicalBoard[rookRank, rookFile]?.Type == PieceType.Rook &&
             castlingRights.Contains(basicRookPositions[(rookRank, rookFile)]))
         {
            logicalBoard[rookRank, rookFile].HasMoved = false;
         }
      }
   }
}
