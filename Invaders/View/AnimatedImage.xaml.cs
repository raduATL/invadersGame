using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Invaders.View {
    /// <summary>
    /// Interaction logic for AnimatedImage.xaml
    /// </summary>
    public partial class AnimatedImage : UserControl {

        private Storyboard invaderShotStoryboard;
        private Storyboard flashStoryboard;

        public AnimatedImage()
        {
            InitializeComponent();
            invaderShotStoryboard = FindResource("invaderShotStoryboard") as Storyboard;
            flashStoryboard = FindResource("flashStoryboard") as Storyboard;
        }

        public AnimatedImage(IEnumerable<string> imageNames, TimeSpan interval) : this()
        {
            StartAnimation(imageNames, interval);
        }

        public void StartAnimation(IEnumerable<string> imageNames, TimeSpan interval)
        {
            Storyboard storyboard = new Storyboard();
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, image);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.SourceProperty));
            TimeSpan currentInterval = TimeSpan.FromMilliseconds(0);
            foreach(string imageName in imageNames)
            {
                ObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
                keyFrame.Value = CreateImageFromAssets(imageName);
                keyFrame.KeyTime = currentInterval;
                animation.KeyFrames.Add(keyFrame);
                currentInterval = currentInterval.Add(interval);
            }
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private static BitmapImage CreateImageFromAssets(string imageFileName)
        {
            try {
                Uri uri = new Uri(imageFileName, UriKind.RelativeOrAbsolute);
                return new BitmapImage(uri);
            } catch (System.IO.IOException)
            {
                return new BitmapImage();
            }
        }

        public void InvaderShot() {
            invaderShotStoryboard.Begin();
        }

        public void StartFlashing() {
            flashStoryboard.Begin();
        }

        public void StopFlashing() {
            flashStoryboard.Stop();
        }
    }
}
