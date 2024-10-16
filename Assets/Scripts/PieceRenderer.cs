using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King };
public enum PieceColor { White, Black };

public interface IPieceRenderer
{
   private static Dictionary<string, Sprite> PieceSprites;
}

public class PieceRenderer : MonoBehaviour, IPieceRenderer
{
   public static Dictionary<string, Sprite> PieceSprites = new Dictionary<string, Sprite>();

   public event EventHandler<PieceClickedEventArgs> OnPieceClicked;

   public PieceLogic pieceLogic;

   public static void LoadSprites()
   {
      for (int piece = (int)PieceType.Pawn; piece <= (int)PieceType.King; piece++)
      {
         foreach (var color in Enum.GetValues(typeof(PieceColor)))
         {
            Sprite pieceSprite = Resources.Load<Sprite>($"Sprites/Piece={(PieceType)piece}, Side={color}");

            PieceSprites.Add($"{color}_{(PieceType)piece}", pieceSprite);
         }
      }
   }

   public void SubscribeToPieceMoved() => pieceLogic.PieceMoved += HandlePieceMoved;

   private void HandlePieceMoved(object sender, PieceMovedEventArgs e) => UpdateVisualPosition(e.Position);

   public void UpdateVisualPosition(Coords newPosition)
   {
      float x = newPosition.File - 3.5f;
      float y = newPosition.Rank - 3.5f;

      transform.position = new Vector2(x, y);
   }

   private void OnMouseDown()
   {
      if (pieceLogic != null)
      {
         PieceClickedEventArgs args = new PieceClickedEventArgs(pieceLogic);
         OnPieceClicked?.Invoke(this, args);
      }
   }
}
public class PieceClickedEventArgs : EventArgs
{
   public PieceLogic Piece;

   public PieceClickedEventArgs(PieceLogic piece)
   {
      Piece = piece;
   }
}