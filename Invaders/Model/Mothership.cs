using System;
using System.Windows;

namespace Invaders.Model {

    class Mothership : Ship {
        public static readonly Size MothershipSize = new Size(20, 15);
        public const double PixelsToMove = 85;
        private DateTime _lastMoved;

        public Mothership(Random random)
            : base(new Point(random.Next(5, (int)InvadersModel.PlayAreaSize.Width), MothershipSize.Height),
                MothershipSize) {
        }

        public override void Move(Direction direction) {
            TimeSpan timeSinceLastMoved = DateTime.Now - _lastMoved;
            double distance = timeSinceLastMoved.Milliseconds * PixelsToMove / 1000;

            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}
