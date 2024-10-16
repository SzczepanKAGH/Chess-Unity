public class Bishop : PieceLogic
{
   public override PieceType Type => PieceType.Bishop;

   public Bishop(PieceColor color, Coords position) : base(color, position)
   {
      Directions = new() { (-1, -1), (1, -1), (1, 1), (-1, 1) };
   }

}