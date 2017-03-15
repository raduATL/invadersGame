using System;
using System.Windows;

namespace Invaders.Model {

    class Shot {

        public const double ShotPixelsPerSecond = 95; // speed up or down by changing this number
        public Point Location { get; private set; }
        public static Size ShotSize = new Size(2, 10);

        public Rect Area { get { return new Rect(Location, ShotSize); } }

        public Direction Direction { get; private set; }

        private DateTime _lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location; Direction = direction;
        }

        public void Move()
        {
            TimeSpan timeSinceLastMoved = DateTime.Now - _lastMoved;
            double distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;

            if (Direction == Direction.Up)
                distance *= -1;
            Location = new Point(Location.X , Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}
