using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SquareScript : MonoBehaviour
{
   public event EventHandler<SquareClickedEventArgs> SquareClicked;
   public Coords SquarePosition { get; set; }

   private void OnMouseDown()
   {
      Debug.Log($"Aww Square clicked {SquarePosition}");

      if (SquareClicked != null)
      {
         var eventArgs = new SquareClickedEventArgs(SquarePosition);
         SquareClicked?.Invoke(this, eventArgs);
      }
   }
}

public class SquareClickedEventArgs : EventArgs
{
   public Coords SquarePosition;

   public SquareClickedEventArgs(Coords squarePosition)
   {
      SquarePosition = squarePosition;
   }
}

