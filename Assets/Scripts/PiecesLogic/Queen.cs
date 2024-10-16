
public class Queen : PieceLogic
{
   public override PieceType Type => PieceType.Queen;

   public Queen(PieceColor color, Coords position, bool hasMoved = false) : base(color, position)
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (1, -1), (1, 1), (-1, 1) };
   }
}
