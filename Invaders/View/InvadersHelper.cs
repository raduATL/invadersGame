using Invaders.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Invaders.View {

    static class InvadersHelper {

        private static readonly Random _random = new Random();

        public static IEnumerable<string> CreateImageList(InvaderType shipType) {
            string filename;
            switch (shipType) {
                case InvaderType.Bug:
                    filename = "bug";
                    break;
                case InvaderType.Spaceship:
                    filename = "spaceship";
                    break;
                case InvaderType.Star:
                    filename = "star";
                    break;
                case InvaderType.Saucer:
                    filename = "flyingsaucer";
                    break;
                case InvaderType.Watchit:
                    filename = "watchit";
                    break;
                case InvaderType.Satellite:
                default:
                    filename = "satellite";
                    break;
            }
            List<string> imageList = new List<string>();
            for (int i = 1; i <= 4; i++)
                imageList.Add(filename + i + ".png");
            return imageList;
        }


        // this is where the Logic is combined with Canvas control, and the control
        // on the canvas with its new position is returned to UI.
        // AnimatedImage is of type FrameworkElement, so it's ok 
        internal static FrameworkElement InvaderControlFactory(Invader invader)
        {
            IEnumerable<string> imageNames = CreateImageList(invader.InvaderType);
            AnimatedImage control = new AnimatedImage(imageNames, TimeSpan.FromMilliseconds(.75));
            control.Width = invader.Area.Width;
            control.Height = invader.Area.Height; 
            SetCanvasLocation(control, invader.Location.X, invader.Location.Y);
            return control;
        }

        internal static FrameworkElement PlayerControlFactory(Player player) {
            List<string> playerImages = new List<string> { "player.png", "player.png" };
            AnimatedImage control = new AnimatedImage(playerImages, TimeSpan.FromSeconds(1));
            control.Width = player.Area.Width;
            control.Height = player.Area.Height;
            SetCanvasLocation(control, player.Location.X, player.Location.Y);
            return control;
        }

        internal static FrameworkElement MothershipControlFactory(Mothership mothership) {
            List<string> mothershipImages = new List<string> { "player.png", "player.png" };
            AnimatedImage control = new AnimatedImage(mothershipImages, TimeSpan.FromSeconds(1));
            control.Width = mothership.Area.Width;
            control.Height = mothership.Area.Height;
            SetCanvasLocation(control, mothership.Location.X, mothership.Location.Y);
            return control;
        }

        internal static FrameworkElement ShotControlFactory(Shot shot) {
            Rectangle rect = new Rectangle();
            rect.Width = shot.Area.Width;
            rect.Height = shot.Area.Height;
            if (shot.Direction == Direction.Down)
                rect.Fill = new SolidColorBrush(Colors.DeepPink);
            else
                rect.Fill = new SolidColorBrush(Colors.Yellow);

            SetCanvasLocation(rect, shot.Location.X, shot.Location.Y);
            return rect;
        }

        internal static FrameworkElement ScanLineFactory(int y, int width) {
            Rectangle rect = new Rectangle();
            rect.Width = width;
            rect.Height = 2;
            rect.Opacity = .025;
            rect.Fill = new SolidColorBrush(Colors.White);
            SetCanvasLocation(rect, 0, y);
            return rect;
        }

        internal static FrameworkElement StarControlFactory(Point point) {
            FrameworkElement star;
            switch (_random.Next(3)) {
                case 0:
                    star = new Rectangle();
                    ((Rectangle)star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                case 1:
                    star = new Ellipse();
                    ((Ellipse)star).Fill = new SolidColorBrush(RandomStarColor());
                    star.Width = 2;
                    star.Height = 2;
                    break;
                default:
                    star = new StarControl();
                    ((StarControl)star).SetFill(new SolidColorBrush(RandomStarColor()));
                    break;
            }
            SetCanvasLocation(star, point.X, point.Y);
            Panel.SetZIndex(star, -1000);
            return star;
        }

        public static void SetCanvasLocation(FrameworkElement control, double x, double y)
        {
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
        }

        public static Point GetCanvasLocation(FrameworkElement control) 
        {
            return new Point(Canvas.GetLeft(control),Canvas.GetTop(control));
        }

        public static void MoveElementOnCanvas(FrameworkElement uiElement, double toX, double toY)
        {
            double fromX = Canvas.GetLeft(uiElement); // canvas will get the x position of the uiElement
            double fromY = Canvas.GetTop(uiElement); // canvas will get the y position of the uiElement

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationX = CreateDoubleAnimation(uiElement, fromX, toX, new PropertyPath(Canvas.LeftProperty));
            DoubleAnimation animationY = CreateDoubleAnimation(uiElement, fromY, toY, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        public static void MoveElementOnCanvasKamikaze(FrameworkElement uiElement, double toX, double toY) {
            double fromX = Canvas.GetLeft(uiElement); // canvas will get the x position of the uiElement
            double fromY = Canvas.GetTop(uiElement); // canvas will get the y position of the uiElement

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationX = CreateDoubleAnimationKamikaze(uiElement, fromX, toX, new PropertyPath(Canvas.LeftProperty));
            DoubleAnimation animationY = CreateDoubleAnimationKamikaze(uiElement, fromY, toY, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        private static DoubleAnimation CreateDoubleAnimation(FrameworkElement uiElement, double from, double to, 
                                                                           PropertyPath propertyToAnimate)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, uiElement);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromMilliseconds(25);
            return animation;
        }

        private static DoubleAnimation CreateDoubleAnimationKamikaze(FrameworkElement uiElement, double from, double to,
                                                                     PropertyPath propertyToAnimate) {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, uiElement);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromSeconds(2);
            return animation;
        }

        public static void SendToBack(FrameworkElement newStar)
        {
            Panel.SetZIndex(newStar, -1000); // alias to Canvas.SetZIndex(newStar, - 1000);
        }

        private static Color RandomStarColor() {
            switch (_random.Next(6)) {
                case 0:
                    return Colors.White;
                case 1:
                    return Colors.LightBlue;
                case 2:
                    return Colors.MediumPurple;
                case 3:
                    return Colors.PaleVioletRed;
                case 4:
                    return Colors.Yellow;
                default:
                    return Colors.LightSlateGray;
            }
        }
    }
}
