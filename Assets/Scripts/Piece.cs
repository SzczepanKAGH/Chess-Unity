using System.Linq;
using System.Collections.Generic;
using System;



public interface IPieceLogic
{ 
   public PieceType Type { get; }
   public PieceColor Color { get; }
   public Coords Position { get; }
   public List<Coords> PossibleMoves { get; set; }
   public HashSet<Coords> PossibleAttacks { get; set; }
   public void GetPossibleMoves(Board board);
   public bool IsColor(PieceColor color);
}

public abstract class PieceLogic : IPieceLogic
{
   public event EventHandler<PieceMovedEventArgs> PieceMoved;
   public Coords Position { get; set; }
   public abstract PieceType Type { get; }
   public PieceColor Color { get; }
   public List<Coords> PossibleMoves { get; set; }
   public HashSet<Coords> PossibleAttacks { get; set; }
   public virtual bool HasMoved { get; set; } = false;
   
   protected List<(int, int)> Directions = new List<(int, int)>();

   public PieceLogic(PieceColor color, Coords position)
   {
      this.Color = color;
      this.Position = position;
      this.PossibleMoves = new List<Coords>();
      this.PossibleAttacks = new HashSet<Coords>();  
   }

   public PieceLogic ShallowCopy()
   {
      return (PieceLogic)MemberwiseClone();
   }

   public PieceLogic DeepCopy()
   {
      PieceLogic copy = (PieceLogic)MemberwiseClone();

      copy.PossibleMoves = new List<Coords>(PossibleMoves);
      copy.PossibleAttacks = new HashSet<Coords>(PossibleAttacks);

      return copy;
   }

   public void GetLegalMoves(Board board)
   {
      GetPossibleMoves(board);
      DiscardIllegalMoves(board);
   }

   public virtual void GetPossibleMoves(Board board)
   {
      PossibleMoves.Clear();
      PossibleAttacks.Clear();

      PieceColor oppositeColor = Utilities.GetOppositeColor(Color);

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         Coords possibleMove = Position + (rankOffset, fileOffset);

         while (Utilities.IsWithinBounds(possibleMove))
         {
            if (board.IsOccupied(possibleMove))
            {
               if (board.GetPieceAtSquare(possibleMove).Color == oppositeColor)
               {
                  PossibleMoves.Add(possibleMove);
                  PossibleAttacks.Add(possibleMove);
               }
               break;
            }
            else
            {
               PossibleMoves.Add(possibleMove);
            }
            possibleMove += (rankOffset, fileOffset);
         }
      }
   }

   protected virtual void DiscardIllegalMoves(Board board) 
   {
      PossibleMoves.RemoveAll(move => !board.IsLegal(Position, move));
      PossibleAttacks = PossibleAttacks.Intersect(PossibleMoves).ToHashSet<Coords>();
   }

   public virtual bool CanCaptureEnemyKing(Board board)
   {
      foreach (Coords attackMove in PossibleAttacks)
      {
         if (board.GetPieceAtSquare(attackMove).Type == PieceType.King) return true;
      }
      return false;
   }

   public virtual void Move(Coords positionTo, bool isClone = false)
   {
      Position = positionTo;
      if (!isClone) RaiseMovedEvent(new PieceMovedEventArgs(Position));
   }

   protected void RaiseMovedEvent(PieceMovedEventArgs eventArgs) => PieceMoved?.Invoke(this, eventArgs);

   public bool IsColor(PieceColor color) => Color == color;

   public virtual bool CanCastleQueenSide(Board board) { throw new NotImplementedException($"Piece {Type} can't castle"); }
   public virtual bool CanCastleKingSide(Board board) { throw new NotImplementedException($"Piece {Type} can't castle"); }
}

public class PieceMovedEventArgs : EventArgs
{
   public Coords Position;

   public PieceMovedEventArgs(Coords position)
   {
      Position = position;
   }
}

