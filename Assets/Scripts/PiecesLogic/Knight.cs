
public class Knight : PieceLogic
{
   public override PieceType Type => PieceType.Knight;

   public Knight(PieceColor color, Coords position) : base(color, position)
   {
      Directions = new() { (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2), (1, 2) };
   }

   public override void GetPossibleMoves(Board board)
   {
      PossibleMoves.Clear();
      PossibleAttacks.Clear();

      foreach (var (rankOffset, fileOffset) in Directions)
      {
         Coords possibleMove = Position + (rankOffset, fileOffset);   
         PieceColor oppositeColor = Utilities.GetOppositeColor(this.Color);

         if (Utilities.IsWithinBounds(possibleMove))
         {
            if (board.IsOccupied(possibleMove))
            {
               //Debug.Log($"Square {rank}, {file} is occupied");
               if (board.GetPieceAtSquare(possibleMove).IsColor(oppositeColor))
               {
                  PossibleAttacks.Add(possibleMove);
                  PossibleMoves.Add(possibleMove);
                  //Debug.Log($"Added Attack {rank}, {file}");
                  continue;
               }
            }
            else
            {
               PossibleMoves.Add(possibleMove);
            }
         }
      }
   }
}
