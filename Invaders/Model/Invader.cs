using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Invaders.Model {

    class Invader : Ship {

        public static readonly Size InvaderSize = new Size(15, 15);
        public const int HorizontalInterval = 15;
        public const int VerticalInterval = 15;

        public bool isKamikaze = false;
        public bool readyForMission = true;

        public double TargetX { get; set; }
        public double TargetY { get; set; }

        public InvaderType InvaderType { get; private set; }
        public int Score { get; private set; }

        public Invader(InvaderType invaderType, Point location, int score) : base(location, InvaderSize)
        {
            InvaderType = invaderType;
            Score = score;
        }

        public override void Move(Direction invaderDirection)
        {
            switch (invaderDirection)
            {
                case Direction.Right:
                    Location = new Point(Location.X + HorizontalInterval, Location.Y);
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - HorizontalInterval, Location.Y);
                    break;
                default:   // down:
                    Location = new Point(Location.X, Location.Y + VerticalInterval);
                    break;
            }
        }

        public static explicit operator Invader(double v) {
            throw new NotImplementedException();
        }
    }
}
