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
   [SerializeField] private float moveDuration = 0.1f;

   public static Dictionary<string, Sprite> PieceSprites = new Dictionary<string, Sprite>();

   public event EventHandler<PieceClickedEventArgs> OnPieceClicked;

   public PieceLogic pieceLogic;

   public static void LoadSprites()
   {
      PieceSprites.Clear();

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
      Vector3 targetPos = new Vector3(x, y, transform.position.z);

      StartCoroutine(AnimateMove(transform.position, targetPos));
   }


   private IEnumerator AnimateMove(Vector3 startPosition, Vector3 newPosition)
   {

      float distance = Vector3.Distance(startPosition, newPosition);

      float elapsedTime = 0;
      while (elapsedTime < moveDuration)
      {
         transform.position = Vector3.Lerp(startPosition, newPosition, elapsedTime / moveDuration);
         elapsedTime += Time.deltaTime;

         yield return null;
      }
      transform.position = newPosition;
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