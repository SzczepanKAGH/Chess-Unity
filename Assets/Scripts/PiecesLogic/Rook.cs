public class Rook : PieceLogic
{
   public override PieceType Type => PieceType.Rook;
   public override bool HasMoved { get; set; }

   public Rook(PieceColor color, Coords position, bool hasMoved = true) : base(color, position)
   {
      Directions = new() { (-1, 0), (0, -1), (1, 0), (0, 1) };
      HasMoved = hasMoved;
   }

   public override void Move(Coords positionTo, bool isClone = false)
   {
      base.Move(positionTo, isClone);
      HasMoved = true;
   }
}