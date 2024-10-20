using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceFactory : MonoBehaviour
{
   public static void SpawnGraphicalPiece(
         GraphicalBoard graphicalBoard,
         PieceLogic pieceLogic,
         Coords pieceCoords,
         PieceType pieceType,
         PieceColor pieceColor)
   {
      var newPiece = new GameObject();
      newPiece.name = $"{pieceType}-{pieceColor}-{pieceCoords}";

      var pieceColider = newPiece.AddComponent<BoxCollider2D>();
      pieceColider.isTrigger = true;

      newPiece.AddComponent<SpriteRenderer>();
      PieceRenderer newPieceRenderer = newPiece.AddComponent<PieceRenderer>();

      newPieceRenderer.pieceLogic = pieceLogic;
      newPieceRenderer.SubscribeToPieceMoved();

      newPiece.GetComponent<SpriteRenderer>().sprite = PieceRenderer.PieceSprites[$"{pieceColor}_{pieceType}"];
      newPiece.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Pieces");
      

      newPiece.transform.position = graphicalBoard.squaresList[pieceCoords.Rank, pieceCoords.File].transform.position;
      
      newPiece.transform.position = new Vector3(newPiece.transform.position.x,
                                                newPiece.transform.position.y,
                                                newPiece.transform.position.z - 1);


      graphicalBoard.graphicalPiecesList.Add(newPiece);
   }

   public static PieceLogic CreatePiece(Coords pieceCoords,
                                        PieceType type,
                                        PieceColor color,
                                        GraphicalBoard graphicalBoard)
   {
      PieceLogic piece = type switch
      {
         PieceType.Pawn => new Pawn(color, pieceCoords),
         PieceType.Queen => new Queen(color, pieceCoords),
         PieceType.Knight => new Knight(color, pieceCoords),
         PieceType.Bishop => new Bishop(color, pieceCoords),
         PieceType.Rook => new Rook(color, pieceCoords),
         PieceType.King => new King(color, pieceCoords),
         _ => null,
      };

      SpawnGraphicalPiece(graphicalBoard, piece, pieceCoords, type, color);

      return piece;
   }
}
