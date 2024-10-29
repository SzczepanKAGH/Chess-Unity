using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class GraphicalBoard : MonoBehaviour // Class responsible for graphic representation of the board
{
   public GameObject tilePrefab;
   public GameObject highlightPrefab;
   public GameObject promotionTilePrefab;
   public Material whiteMaterial;
   public Material blackMaterial;

   public GameObject[,] squaresList = new GameObject[8, 8];
   public List<GameObject> highlitedSquares = new();
   public List<GameObject> graphicalPiecesList = new();
   public Dictionary<int ,string> figuresToPromote = new Dictionary<int ,string>() { {0, "Queen" }, { 1, "Knight"},
                                                                                       {2, "Rook" }, {3, "Bishop"}};

   public void CreateGraphicalBoard()
   {
      for (int rankIdx = 0; rankIdx < 8; rankIdx++)
      {
         for (int fileIdx = 0; fileIdx < 8; fileIdx++)
         {
            var position = new Vector2(fileIdx - 3.5f, rankIdx - 3.5f); // In Unity X goes first, then Y
            bool isLight = Utilities.IsLightSquare(rankIdx, fileIdx);
            var newSquare = CreateSquare(isLight, position);

            newSquare.GetComponent<SquareScript>().SquarePosition = new Coords(rankIdx, fileIdx);

            squaresList[rankIdx, fileIdx] = (newSquare);
         }
      }
   }

   public GameObject[] CreatePromotionMenu(PieceColor promotingSide, Coords promotionCoords)
   {
      GameObject[] promotionMenu = new GameObject[4]; // In chess you can promote pawn to 4 types of pieces

      // That's the reason menu takes 4 squares, on each of four sqaures there is displayed another piece
      for (int tileOffset = 0; tileOffset < 4; tileOffset++)               // graphic for player to choose
      {
         GameObject promotionAlternative = CreatePromotionTile(tileOffset, promotionCoords, promotingSide);
         promotionAlternative.GetComponent<PromotionScript>().SetPiece(tileOffset, promotionCoords, promotingSide);
         promotionMenu[tileOffset] = promotionAlternative;
      }
      return promotionMenu;
   }

   public void DestroyPieceGraphic(Coords piecePosition, EventHandler<PieceClickedEventArgs> pieceClickedEventHandler)
   {
      GameObject pieceForRemoval = graphicalPiecesList.FirstOrDefault(piece =>
         piece.GetComponent<PieceRenderer>().pieceLogic.Position == piecePosition);

      pieceForRemoval.GetComponent<PieceRenderer>().OnPieceClicked -= pieceClickedEventHandler;

      graphicalPiecesList.Remove(pieceForRemoval);
      Destroy(pieceForRemoval);
   }

   public void DestroyPromotionMenu(EventHandler<PieceToPromoteChosen> pieceToPromoteChosenEventHandler)
   {
      GameObject[] promotionTiles = GameObject.FindGameObjectsWithTag("PromotionTile");

      foreach (GameObject promotionTile in promotionTiles)
      {
         var promotionScript = promotionTile.GetComponent<PromotionScript>();
         promotionScript.OnPromotionChosen -= pieceToPromoteChosenEventHandler;

         Destroy(promotionTile);
      }
   }

   private GameObject CreatePromotionTile(int tileOffset, Coords promotionCoords, PieceColor promotingSide)
   {
      // This value makes sure that promotion menu is created towards the center of the board regardless of promoting side
      int offsetDirection = (promotingSide == PieceColor.White) ? -1 : 1; 

      float tileXpos = promotionCoords.File - 3.5f;
      float tileYpos = promotionCoords.Rank - 3.5f + tileOffset * offsetDirection;

      // Z value is assigned in order to make sure, that tile is displayed over pieces and board squares
      var tilePosition = new Vector3(tileXpos, tileYpos, -2); 

      GameObject promotionTile = Instantiate(promotionTilePrefab, tilePosition, Quaternion.identity);
      promotionTile.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("PromotionLayer");

      string figureToPromoteID = $"Piece={figuresToPromote[tileOffset]}, Side={promotingSide}";
      SetupPromotionPieceGraphic(promotionTile ,figureToPromoteID);

      return promotionTile;
   }

   private void SetupPromotionPieceGraphic(GameObject promotionTile, string figureID)
   {
      GameObject figureToPromoteObject = new GameObject($"figureToPromote-{figureID}");
      figureToPromoteObject.transform.parent = promotionTile.transform;
      figureToPromoteObject.transform.position = promotionTile.transform.position;

      SpriteRenderer spriteRendererFigure = figureToPromoteObject.AddComponent<SpriteRenderer>();

      spriteRendererFigure.sprite = Resources.Load<Sprite>($"Sprites/{figureID}");
      spriteRendererFigure.sortingLayerID = SortingLayer.NameToID("PromotionLayer");
      spriteRendererFigure.sortingOrder = 1;
   }

   private GameObject CreateSquare(bool isLight, Vector2 position)
   {
      GameObject newSquare = Instantiate(tilePrefab, position, Quaternion.identity);
      Renderer renderer = newSquare.GetComponent<Renderer>();

      renderer.material = (isLight) ? whiteMaterial : blackMaterial;

      return newSquare;
   }

   public void HighlightSquares(List<Coords> moveCoords, HashSet<Coords> attackCoords)
   {
      List<Coords> onlyMoves = moveCoords.Except(attackCoords).ToList();

      foreach (var squareToMove in onlyMoves) HighlightSquare(squareToMove);
      foreach (var squareToAttack in attackCoords) HighlightSquare(squareToAttack, isAttack : true);
   }

   private void HighlightSquare(Coords squarePosition, bool isAttack = false)
   {
      int rankIndex = squarePosition.Rank;
      int fileIndex = squarePosition.File;

      Vector2 squarePositionAsVector = new Vector2(fileIndex - 3.5f, rankIndex - 3.5f);
      GameObject newHighlight = Instantiate(highlightPrefab, squarePositionAsVector, Quaternion.identity);

      if (isAttack) newHighlight.transform.localScale = new Vector2(0.8f, 0.8f);
 
      highlitedSquares.Add(newHighlight);
   }

   public void ClearHighlitedSquares()
   {
      foreach (GameObject highlitedSquare in highlitedSquares) Destroy(highlitedSquare);
   }
}