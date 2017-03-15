using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Invaders.ViewModel;

namespace Invaders.Model {

    class InvadersModel {
        public readonly static Size PlayAreaSize = new Size(700, 500);
        public const int MaximumPlayerShots = 10;
        public const int InitialStarCount = 50;
        public const int MaximumMotherships = 1;

        private readonly Random _random = new Random();

        public int Score { get; private set; }
        public int Wave { get; private set; }
        public int Lives { get; private set; }

        public bool GameOver { get; private set; }

        public DateTime? _playerDied = null; 
        public bool PlayerDying { get { return _playerDied.HasValue; } }

        private Player _player;

        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();
        private readonly List<Mothership> _motherships = new List<Mothership>();
        private readonly List<Invader> _kamikaze = new List<Invader>();

        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown = false;

        private DateTime _lastUpdated = DateTime.MinValue; // used to update movement of Invaders

        private DateTime _lastPlayerShot = DateTime.MinValue;

        private DispatcherTimer _timerKamikaze = new DispatcherTimer();

        public InvadersModel() {
            _player = new Player();
        }

  
        public void StartGame()
        {
            _timerKamikaze.Tick += _timerKamikaze_Tick;
            _timerKamikaze.Interval = TimeSpan.FromSeconds(15);
            _timerKamikaze.Start();

            GameOver = false;
            ClearMotherships();
            ClearInvaders();
            ClearShots(); // clear both Player's and Invader's shots, firing ShotMoved events
            foreach (Point star in _stars)
                OnStarChanged(star, true);
            _stars.Clear();

            _player = new Player();
            OnShipChanged(_player, false);
            Lives = 3;
            Wave = 0;
            AddStars();
            NextWave();
        }

        private void _timerKamikaze_Tick(object sender, EventArgs e) {
            // get a random invader to be Kamikaze
            if (_invaders.Count > 0 & _kamikaze.Count <= 1 ) {
                int rnd = _random.Next(_invaders.Count());
                _invaders[rnd].isKamikaze = true;
                _kamikaze.Add(_invaders[rnd]);
                OnShipChanged(_invaders[rnd], false);
            }
            if (_kamikaze.Count == 1)
                _kamikaze.Clear();
        }


        private void NextWave()
        {
            Wave++;
            _invaders.Clear();
            for(int row = 0; row <= 5; row ++ ) {
                for( int column = 0; column < 11; column++) {
                    Point location = new Point(column * Invader.InvaderSize.Width * 2.4, row * Invader.InvaderSize.Height * 2.4);
                    Invader invader;
                    switch (row) {
                        case 0:
                            invader = new Invader(InvaderType.Spaceship, location, 50);
                            break;
                        case 1:
                            invader = new Invader(InvaderType.Bug, location, 40);
                            break;
                        case 2:
                            invader = new Invader(InvaderType.Saucer, location, 30);
                            break;
                        case 3:
                            invader = new Invader(InvaderType.Satellite, location, 20);
                            break;
                        case 4:
                            invader = new Invader(InvaderType.Watchit, location, 15);
                            break;
                        default:
                            invader = new Invader(InvaderType.Star, location, 10);
                            break;
                    }
                    _invaders.Add(invader);
                    OnShipChanged(invader, false);
                }
            }
        }

        private void ClearShots()
        {
            if(_invaderShots.Count > 0 )
                foreach(Shot shot in _invaderShots.ToList()) {
                    OnShotMoved(shot, true);
                    _invaderShots.Remove(shot);
                }

            if (_playerShots.Count > 0 )
                foreach(Shot shot in _playerShots.ToList()) {
                    OnShotMoved(shot, true);
                    _playerShots.Remove(shot);
                }
        }

        private void ClearInvaders()
        {
            if (_invaders.Count > 0)
                foreach (Invader invader in _invaders.ToList()) {
                    OnShipChanged(invader, true);
                    _invaders.Remove(invader);
                }

            if(_kamikaze.Count > 0) {
                foreach(Invader invader in _kamikaze.ToList()) {
                    OnShipChanged(invader, true);
                    _kamikaze.Remove(invader);
                }
            }
        }

        private void ClearMotherships() {
            if (_motherships.Count > 0)
                foreach (Mothership mothership in _motherships.ToList()) {
                    OnShipChanged(mothership, true);
                    _motherships.Remove(mothership);
                }
        }

        private void AddStars()
        {
           for(int i = 0; i < InitialStarCount; i++) {
                AddAStar();
            }
        }

        public void AddMothership() {
            if (GameOver)
                return;

            if (_motherships.Count >= MaximumMotherships)
                return;

            Mothership mothership = new Mothership(_random);
            _motherships.Add(mothership);
            OnShipChanged(mothership, false);
        }
      
        public void FireShot()  // makes the Player fire a shot
        {
            if (GameOver)
                return;

            if (_playerShots.Count <= MaximumPlayerShots)  // 3
               {
                Point shotLocation = new Point(_player.Location.X + _player.Size.Width / 2, _player.Location.Y);
                Shot shot = new Shot(shotLocation, Direction.Up);
                _playerShots.Add(shot);
                // play shot sound !

                OnShotMoved(shot, false);
            }
        }

        public void MovePlayer(Direction direction)
        {
            if (_playerDied.HasValue)
                 return;

            _player.Move(direction);
            OnShipChanged(_player, false);
        }

        public void Twinkle()
        {
            // There are always fewer than 50% more and greater than 15% fewer than the
            // initial number of stars. In our case 50.

            // greater than 15% fewer
            if (_random.Next(2) == 1 && _stars.Count < InitialStarCount * .75) 
                AddAStar();
            else if (_stars.Count > (InitialStarCount * 1.5)) // fewer than 50% more. No less than 25 stars on the sky
                RemoveAStar();                                // at any given moment
        }

        private void RemoveAStar()
        {
            if (_stars.Count <= 0)
                return;

            Point starToRemove = _stars[_random.Next(_stars.Count)];
            _stars.Remove(starToRemove);
            OnStarChanged(starToRemove, true); // fire the event before removing the star
        }

        private void AddAStar()
        {
            Point newStar = new Point(_random.Next((int)PlayAreaSize.Width),
                                           _random.Next(20, (int)PlayAreaSize.Height) - 20);
            if (!_stars.Contains(newStar))
            {
                _stars.Add(newStar);
                OnStarChanged(newStar, false); // star appears
            }
        }

        public void Update(bool paused)
        {
            if (!paused)
            {
                if (_invaders.Count <= 0)
                    NextWave();
                if (!_playerDied.HasValue)
                {
                    MoveInvaders();
                    MovePlayerShots(); //  Player's shots
                    MoveInvaderShots();
                    MoveMothership();
                    ReturnFire();
                    CheckForInvaderCollision();
                    CheckForPlayerCollision();
                    CheckForShotsCollision();
                    CheckForMothershipCollision(); // Mothership collides only with Player or out of bounds
                    CheckForKamikazeCollision();
                } 
                else if (_playerDied.HasValue && DateTime.Now > _playerDied + TimeSpan.FromSeconds(2.5))
                {
                    _playerDied = null; // reset player life
                    OnShipChanged(_player, false);
                }
            }
            Twinkle();
        }

        private void MoveInvaders()
        {
            // this changes according to Wave number
            double milisecondsBetweenMovements = Math.Min(10 - Wave, 1) * (2 * _invaders.Count());

            if (DateTime.Now - _lastUpdated > TimeSpan.FromMilliseconds(milisecondsBetweenMovements))
            {
                _lastUpdated = DateTime.Now;

                var invadersTouchingLeftBoundary = from invader in _invaders
                                                   where invader.Area.Left < Invader.HorizontalInterval
                                                   select invader;

                var invadersTouchingRightBoundary = from invader in _invaders
                                                    where invader.Area.Right > PlayAreaSize.Width - Invader.HorizontalInterval * 2
                                                    select invader;

                if (!_justMovedDown)
                {
                    if (invadersTouchingLeftBoundary.Count() > 0)
                    {
                        foreach (Invader invader in _invaders) {
                            invader.Move(Direction.Down);
                            OnShipChanged(invader, false);
                        }
                        _invaderDirection = Direction.Right;

                    }
                    else if (invadersTouchingRightBoundary.Count() > 0)
                    {
                        foreach (Invader invader in _invaders) {
                            invader.Move(Direction.Down);
                            OnShipChanged(invader, false);
                        }
                        _invaderDirection = Direction.Left;
                    }
                    _justMovedDown = true;
                } 
                else
                {
                    _justMovedDown = false;

                    foreach (Invader invader in _invaders) {
                        invader.Move(_invaderDirection);
                        OnShipChanged(invader, false);
                    }
                    
                }
            } 
        }

        private void MovePlayerShots()
        {
            foreach (Shot shot in _playerShots) {
                shot.Move();
                OnShotMoved(shot, false);
            }

            var shotsOutOfBounds = from shot in _playerShots
                                   where (shot.Location.Y < 10 )
                                   select shot;

            if (shotsOutOfBounds.Count() > 0) {
                foreach(Shot shot in shotsOutOfBounds.ToList()) {
                    _playerShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
            }
        }

        private void MoveInvaderShots() {
            foreach (Shot shot in _invaderShots) {
                shot.Move();
                OnShotMoved(shot, false);
            }

            var shotsOutOfBounds = from shot in _invaderShots
                                   where (shot.Location.Y > PlayAreaSize.Height - 10) 
                                   select shot;

            if (shotsOutOfBounds.Count() > 0) {
                foreach (Shot shot in shotsOutOfBounds.ToList()) {
                    _invaderShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
            }
        }

        private void MoveMothership() {
            foreach(Mothership mothership in _motherships) {
                mothership.Move(Direction.Down);
                OnShipChanged(mothership, false);
            }

            var msOutOfBounds = from ms in _motherships
                                where (ms.Location.Y > PlayAreaSize.Height - 5)
                                select ms;

            if (msOutOfBounds.Count() > 0) {
                foreach(Mothership moship in _motherships.ToList()) {
                    _motherships.Remove(moship);
                    OnShipChanged(moship, true);
                }
            }
        }

        private void ReturnFire()
        {
            if (_invaders.Count() <= 0)
                return;

            if (_invaderShots.Count() > Wave + 1 || _random.Next(10) < 10 - Wave)
                return;

            var result = from invader in _invaders
                        group invader by invader.Area.X into invaderGroup
                        orderby invaderGroup.Key descending
                        select invaderGroup;
            var randomGroup = result.ElementAt(_random.Next(result.ToList().Count()));
            var randomInvader = randomGroup.Last(); // find a random invader at the bottom coloumn

            Point shotLocation = new Point(randomInvader.Area.X + randomInvader.Area.Width / 2, randomInvader.Area.Bottom + 2);
            Shot shot = new Shot(shotLocation, Direction.Down);
            _invaderShots.Add(shot);
            OnShotMoved(shot, false);
        }

        private void CheckForPlayerCollision()
        {
            bool removeAllShots = false;

            // check to see if invaders collided with player
            var result = from invader in _invaders
                         where RectsOverlap(invader.Area, _player.Area)
                         select invader;
            if (result.Count() > 0)
                EndGame();

            // check to see if any invader shots hit the player
            var shotsHit = from shot in _invaderShots
                           where RectsOverlap(_player.Area, shot.Area)
                           select shot;

            if (shotsHit.Count() > 0) // if we have a collision
            {
                  Lives--;
                if (Lives >= 0) {
                    _playerDied = DateTime.Now;
                    OnShipChanged(_player, true);
                    removeAllShots = true;
                }
                else
                    EndGame();
            }

            if (removeAllShots) {
                foreach(Shot shot in _playerShots.ToList()) {  // use .ToList() to copy the list first, 
                    OnShotMoved(shot, true);                   // otherwise u get InvalidOperationException
                    _playerShots.Remove(shot);                 // because you cannot modify a collection inside 
                }                                              // a foreach loop
                foreach(Shot shot in _invaderShots.ToList()) {
                    OnShotMoved(shot, true);
                    _invaderShots.Remove(shot);
                }
            }
        }

        private void CheckForMothershipCollision() {
            foreach (Mothership moship in _motherships.ToList())
                if (RectsOverlap(moship.Area, _player.Area)) {
                    Lives++;
                    _motherships.Remove(moship);
                    OnShipChanged(moship, true);
                }
        }

        public void CheckForKamikazeCollision() {
            foreach(Invader invader in _invaders.ToList())
                if (invader.isKamikaze) {
                    if (RectsOverlap(invader.Area, _player.Area)) {
                        Lives--;
                        _invaders.Remove(invader);
                        OnShipChanged(invader, true);

                        if (Lives >= 0) {
                            _playerDied = DateTime.Now;
                            OnShipChanged(_player, true);
                        }
                        else
                            EndGame();
                    }
                }
        }

        private void CheckForInvaderCollision()
        {
            foreach (Shot shot in _playerShots.ToList())
                foreach (Invader invader in _invaders.ToList())
                    if (RectsOverlap(invader.Area, shot.Area)) {
                        _playerShots.Remove(shot);
                        OnShotMoved(shot, true);
                        Score += invader.Score;
                        _invaders.Remove(invader);
                        OnShipChanged(invader, true);
                    }
        }

        // if player and invader shots collide
       private void CheckForShotsCollision() {
            foreach(Shot shot in _playerShots.ToList())
                foreach(Shot ishot in _invaderShots.ToList())
                    if(RectsOverlap(shot.Area, ishot.Area)) {
                        _playerShots.Remove(shot);
                        OnShotMoved(shot, true);
                        _invaderShots.Remove(ishot);
                        OnShotMoved(ishot, true);
                    }
        }

        public static bool RectsOverlap(Rect r1, Rect r2)
        {
            r1.Intersect(r2); // finds the intersection of the current Rect and the specified Rect and stores the result as current Rect (r1)
            if (r1.Width > 0 || r1.Height > 0) // that means we have an intersection
                return true;

            return false;
        }

      
        public void EndGame() {
            GameOver = true;
            _timerKamikaze.Stop();
        }


        internal void UpdateAllShipsAndStars() {
            foreach (Shot shot in _playerShots)
                OnShotMoved(shot, false);
            foreach (Invader ship in _invaders)
                OnShipChanged(ship, false);
            OnShipChanged(_player, false);
            foreach (Point star in _stars)
                OnStarChanged(star, false);
        }

        //
        // the events and methods that fire the events
        //
        public event EventHandler<ShotMovedEventArgs> ShotMoved;
        public void OnShotMoved(Shot shotThatMoved, bool dissapeared)
        {
            ShotMoved?.Invoke(this, new ShotMovedEventArgs(shotThatMoved, dissapeared));
        }

        public event EventHandler<StarChangedEventArgs> StarChanged;
        public void OnStarChanged(Point point, bool disappeared)
        {
            StarChanged?.Invoke(this, new StarChangedEventArgs(point, disappeared));
        }

        public event EventHandler<ShipChangedEventArgs> ShipChanged;
        public void OnShipChanged(Ship shipUpdated, bool killed)
        {
            ShipChanged?.Invoke(this, new ShipChangedEventArgs(shipUpdated, killed));
        }

    }
}
