using Invaders.Model;
using Invaders.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Invaders.View {
    /// <summary>
    /// Interaction logic for Invaders.xaml
    /// </summary>
    public partial class InvadersPage : Window {

        InvadersViewModel viewModel;

        public InvadersPage()
        {
            InitializeComponent();
            viewModel = FindResource("viewmodel") as InvadersViewModel;
            viewModel.shipBlasted += ViewModel_shipBlasted;
        }

        private void ViewModel_shipBlasted(object sender, Model.ShipChangedEventArgs e) {
            if (e.ShipUpdated is Player)
                blastedPlayerMedia.Play();
            else if (e.ShipUpdated is Invader)
                blastedInvaderMedia.Play();
        }

        private void startButton_Click(object sender, RoutedEventArgs e) {
            viewModel.StartGame();
            backmedia.Volume = 0.8;
            backmedia.Play();
        }

        private void Element_MediaEnded(object sender, EventArgs e) {
            media.Stop();
        }

        private void Element_BackMediaEnded(object sender, EventArgs e) {
            backmedia.Play();
        }

        private void Element_blastedPlayerMediaEnded(object sender, EventArgs e) {
            blastedPlayerMedia.Stop();
        }

        private void Element_blastedInvaderMediaEnded(object sender, EventArgs e) {
            blastedInvaderMedia.Stop();
        }


        private void window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Left) {
                viewModel.MoveLeft();
            }
            if (e.Key == Key.Right) {
                viewModel.MoveRigh();
            }
            if (e.Key == Key.Space) {
                viewModel.FireShot();
                media.Volume = 0.20; 
                media.Play();
 
            }
            if (e.Key == Key.P ) {
                viewModel.PauseGame();
            }
            if (e.Key == Key.R) {
                viewModel.ResumeGame();
            }
           
        }

    }
}
