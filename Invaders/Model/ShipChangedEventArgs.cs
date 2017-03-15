using System;

namespace Invaders.Model {

    class ShipChangedEventArgs : EventArgs {

        public Ship ShipUpdated { get; private set; }
        public bool Killed { get; private set; }

        public ShipChangedEventArgs(Ship shipupdated, bool killed)
        {
            ShipUpdated = shipupdated;
            Killed = killed;
        }
    }
}
