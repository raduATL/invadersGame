using System.Windows;

namespace Invaders.Model {

    class Player : Ship {
        public static readonly Size PlayerSize = new Size(20, 15);
        const double PixelsToMove = 10;

        public Player() 
         : base(new Point(PlayerSize.Width, InvadersModel.PlayAreaSize.Height - PlayerSize.Height * 3), PlayerSize)
        {
          
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (Location.X > PlayerSize.Width)
                        Location = new Point(Location.X - PixelsToMove, Location.Y);
                    break;
                default:   // right
                    if (Location.X < InvadersModel.PlayAreaSize.Width - PlayerSize.Width * 2)
                        Location = new Point(Location.X + PixelsToMove, Location.Y);
                    break;
            }
        }
    }
}
