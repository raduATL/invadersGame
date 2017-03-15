using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using DispetcherTimer = System.Windows.Threading.DispatcherTimer;
using Invaders.Model;
using Invaders.View;
using System.ComponentModel;

namespace Invaders.ViewModel {

      class InvadersViewModel : INotifyPropertyChanged {

        private ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
        public INotifyCollectionChanged Sprites { get { return _sprites; } }
        
        public int Lives { get; set; }
        public int Score { get; set; }

        private bool _started = false;
        public bool Started {
            get { return _started; }
            set { _started = value; }
        }
        public bool Paused { get; set; }
        private bool _lastPaused = true;
        private readonly List<Point> _fadedStars = new List<Point>();
        private DispetcherTimer _timer = new DispetcherTimer();
        private DispetcherTimer _timerMotherShip = new DispetcherTimer();
      
        private InvadersModel _model = new InvadersModel();
        private FrameworkElement _playerControl = null;
        private bool _playerFlashing = false;

        private readonly Dictionary<Invader, FrameworkElement> _invaders = new Dictionary<Invader, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders = new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<Shot, FrameworkElement> _shots = new Dictionary<Shot, FrameworkElement>();
        private readonly Dictionary<Point, FrameworkElement> _stars = new Dictionary<Point, FrameworkElement>();
        private readonly Dictionary<Mothership, FrameworkElement> _motherships = new Dictionary<Mothership, FrameworkElement>();
        private readonly List<FrameworkElement> _scanLines = new List<FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _kamikazeInvaders = new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<FrameworkElement, DateTime> _kamInvEndMission = new Dictionary<FrameworkElement, DateTime>();

        public Size PlayAreaSize{
            get {
                 return InvadersModel.PlayAreaSize; }
            set {
                _model.UpdateAllShipsAndStars();
            }
        }

        public void StartGame() {
            if (!Started) {
                Paused = false;
                foreach (var invader in _invaders.Values) _sprites.Remove(invader);
                foreach (var shot in _shots.Values) _sprites.Remove(shot);
                foreach (var mothership in _motherships.Values) _sprites.Remove(mothership);
                _model.StartGame();
                OnPropertyChanged("GameOver");
                //RecreateScanLines();
                _timer.Start();
                _timerMotherShip.Start();
                Started = true;
            }
        }

        internal void MoveLeft() {
            if(_timer.IsEnabled)
               _model.MovePlayer(Direction.Left);
        }

        internal void MoveRigh() {
            if(_timer.IsEnabled)
               _model.MovePlayer(Direction.Right);
        }

        internal void FireShot() {
            if(_timer.IsEnabled)
               _model.FireShot();
        }

        internal void PauseGame() {
            _timer.IsEnabled = false;
            Paused = true;
            OnPropertyChanged("Paused");
        }

        internal void ResumeGame() {
            _timer.IsEnabled = true;
            Paused = false;
            OnPropertyChanged("Paused");

        }

        private void RecreateScanLines() {
           foreach(FrameworkElement scanLine in _scanLines) {
                if (_sprites.Contains(scanLine)) {
                    _sprites.Remove(scanLine);
                }
                _scanLines.Clear();
            }
           for(int y=0; y<600; y++) {
                FrameworkElement scanLine = InvadersHelper.ScanLineFactory(y, 700);
                _scanLines.Add(scanLine);
                _sprites.Add(scanLine);
            }
        }

        public bool GameOver { get { return _model.GameOver; } }


        public InvadersViewModel()
        {
            _model.ShipChanged += ShipChangedHandler;
            _model.ShotMoved += ShotMovedHandler;
            _model.StarChanged += StarChangedHandler;

            _timer.Tick += timerTickEventHandler;
            _timer.Interval = TimeSpan.FromMilliseconds(100); // updates the View 10x a second

            _timerMotherShip.Tick += timerMotherShip_TickHandler;
            _timerMotherShip.Interval = TimeSpan.FromSeconds(25);
        }
 

        private void timerMotherShip_TickHandler(object sender, EventArgs e) {
            if (!Paused) {
                _model.AddMothership();
            }
        }

        private void timerTickEventHandler(object sender, EventArgs e)
        {
            if (_lastPaused != Paused) {
                OnPropertyChanged("Paused");
                _lastPaused = Paused;
            }

            if (!Paused) {
                _model.Update(false);
            }

            if (Score != _model.Score) {
                Score = _model.Score;
                OnPropertyChanged("Score");
            }

            if (Lives != _model.Lives) {
                Lives = _model.Lives;
                OnPropertyChanged("Lives");
            }

            if (_model.GameOver) {
                OnPropertyChanged("GameOver");
                _timer.Stop();
                _timerMotherShip.Stop();
                Started = false;
            }
        }


        private void ShipChangedHandler(object sender, ShipChangedEventArgs e) {
            if (!e.Killed) {
                if (e.ShipUpdated is Invader) {
                    if (!_invaders.ContainsKey((Invader)e.ShipUpdated)) {
                        Invader invader = (Invader)e.ShipUpdated;
                        FrameworkElement shipControl = InvadersHelper.InvaderControlFactory(invader);
                        _invaders[invader] = shipControl;
                        _sprites.Add(shipControl);
                    }
                    else {
                        Invader invader = (Invader)e.ShipUpdated;
                        FrameworkElement shipControl = _invaders[invader];
                     
                        if (invader.isKamikaze) {
                            if(_playerControl != null) {
                                if (invader.readyForMission) {
                                    // move towards the player
                                    Point playerLocation = InvadersHelper.GetCanvasLocation(_playerControl);
                                    InvadersHelper.MoveElementOnCanvasKamikaze(shipControl, playerLocation.X, playerLocation.Y);
                                    // memorize player location
                                    invader.TargetX = playerLocation.X;
                                    invader.TargetY = playerLocation.Y;
                                    _kamikazeInvaders.Add(shipControl, DateTime.Now);
                                    _kamInvEndMission.Add(shipControl, DateTime.Now);
                                    invader.readyForMission = false;
                                }
                                // check 3 seconds time span for kamikaze mission
                                FrameworkElement[] array = new FrameworkElement[_kamikazeInvaders.Count];
                                _kamikazeInvaders.Keys.CopyTo(array, 0);
                                foreach (FrameworkElement control in array) {
                                    DateTime elapsed = _kamikazeInvaders[control];
                                    if (DateTime.Now - elapsed > TimeSpan.FromSeconds(3)) {
                                        if (invader.isKamikaze) {
                                            invader.isKamikaze = false; // if time expired convert kamikaze back to invader
                                            _kamikazeInvaders.Remove(control);
                                        }
                                    }
                                }

                                // if kamikaze time has not expired keep moving toward locked in player location
                                InvadersHelper.MoveElementOnCanvasKamikaze(shipControl, invader.TargetX, invader.TargetY);
                            }
                        }
                        else {
                            if (!invader.readyForMission) {
                                // now the invader is on its way back to formation Kamikaze speed
                                InvadersHelper.MoveElementOnCanvasKamikaze(shipControl, invader.Location.X, invader.Location.Y);

                                FrameworkElement[] array = new FrameworkElement[_kamInvEndMission.Count];
                                _kamInvEndMission.Keys.CopyTo(array, 0);
                                foreach (FrameworkElement control in array) {
                                    DateTime elapsed = _kamInvEndMission[control];
                                    if (DateTime.Now - elapsed > TimeSpan.FromSeconds(5)) {
                                        if (invader.readyForMission == false) {
                                            invader.readyForMission = true; // if time expired convert kamikaze back to 
                                                                            // invader status
                                                                            // ready for new Mission
                                            _kamInvEndMission.Remove(control);
                                        }
                                    }
                                }

                            }
                            else  // Move invader on canvas on normal formation speed
                                InvadersHelper.MoveElementOnCanvas(shipControl, invader.Location.X, invader.Location.Y);
                        }
                    }
                }
                else if (e.ShipUpdated is Player) {

                    // play sound
                    if (_playerFlashing) {
                        _playerFlashing = false;
                        AnimatedImage control = _playerControl as AnimatedImage;
                        if (control != null)
                            control.StopFlashing();
                    }

                    Player player = e.ShipUpdated as Player;
                    if (_playerControl == null) {
                        _playerControl = InvadersHelper.PlayerControlFactory(player);
                        _sprites.Add(_playerControl);
                    }
                    else {
                        // move player control
                        InvadersHelper.MoveElementOnCanvas(_playerControl, player.Location.X, player.Location.Y);
                    }
                }
                else if(e.ShipUpdated is Mothership) {
                    if (!_motherships.ContainsKey((Mothership)e.ShipUpdated)) {
                        Mothership moship = (Mothership)e.ShipUpdated;
                        FrameworkElement moshipControl = InvadersHelper.MothershipControlFactory(moship);
                        _motherships[moship] = moshipControl;
                        _sprites.Add(moshipControl);
                    }
                    else {
                        //AnimatedImage beeControl = _bees[e.BeeThatMoved];
                        //BeeStarHelper.MoveElementOnCanvas(beeControl, e.X, e.Y);
                        Mothership moship = (Mothership)e.ShipUpdated;
                        FrameworkElement moshipControl = _motherships[moship];
                        InvadersHelper.MoveElementOnCanvas(moshipControl, moship.Location.X, moship.Location.Y);
                    }
                }
            }
            else {
                // if invader or Player is killed
                OnShipBlasted(e.ShipUpdated, true); // invoke event to play blasting sound

                if (e.ShipUpdated is Invader) {
                    Invader invader = e.ShipUpdated as Invader;
                    if (!_invaders.ContainsKey(invader)) return;

                    AnimatedImage invaderControl = _invaders[invader] as AnimatedImage;
                    invaderControl.InvaderShot(); // calls the invader storyboard of fading out invader
                    _shotInvaders[invaderControl] = DateTime.Now;
                    _invaders.Remove(invader);
                }
                else if (e.ShipUpdated is Player) {
                    AnimatedImage control = _playerControl as AnimatedImage;
                    if (control != null)
                        control.StartFlashing();
                    _playerFlashing = true;
                }
                else if (e.ShipUpdated is Mothership) {
                    Mothership moship = e.ShipUpdated as Mothership;
                    if (!_motherships.ContainsKey(moship)) return;

                    AnimatedImage moshipControl = _motherships[moship] as AnimatedImage;
                    _motherships.Remove(moship);
                    if (_sprites.Contains(moshipControl))
                        _sprites.Remove(moshipControl);
                }
            }
        }


        private void ShotMovedHandler(object sender, ShotMovedEventArgs e) {
            if (!e.Dissapeared) {
                if (!_shots.ContainsKey(e.Shot)) {
                    FrameworkElement shotControl = InvadersHelper.ShotControlFactory(e.Shot);
                    _shots[e.Shot] = shotControl;
                    _sprites.Add(shotControl);
                }
                else {
                    FrameworkElement shotControl = _shots[e.Shot];
                    InvadersHelper.MoveElementOnCanvas(shotControl, e.Shot.Location.X, e.Shot.Location.Y);
                }
            }
            else {
                // the shot dissapeared, so check _shots to see if it's there
                if (_shots.ContainsKey(e.Shot)) {
                    FrameworkElement shotControl = _shots[e.Shot];
                    _shots.Remove(e.Shot);
                    _sprites.Remove(shotControl);
                }
            }
        }

        private void StarChangedHandler(object sender, StarChangedEventArgs e)
        {
            if(e.Disappeared && _stars.ContainsKey(e.Point)) {
                if (_sprites.Contains(_stars[e.Point])) {
                    _sprites.Remove(_stars[e.Point]);
                    _stars.Remove(e.Point);
                }
            }
            else {
                if (!_stars.ContainsKey(e.Point)) {
                    FrameworkElement starControl = InvadersHelper.StarControlFactory(e.Point);
                    _stars[e.Point] = starControl;
                    _sprites.Add(starControl);
                }
                else {
                    FrameworkElement starControl = _stars[e.Point];
                    InvadersHelper.MoveElementOnCanvas(starControl, e.Point.X, e.Point.Y);
                }
            }
        }

        public event EventHandler<ShipChangedEventArgs> shipBlasted;
        public void OnShipBlasted(Ship blastedShip, bool killed) {
            shipBlasted?.Invoke(this, new ShipChangedEventArgs(blastedShip, killed));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    } // end class InvadersViewModel

} // end namespace Invaders
