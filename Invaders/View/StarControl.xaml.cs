using System.Windows.Controls;
using System.Windows.Media;


namespace Invaders.View {
    /// <summary>
    /// Interaction logic for StarControl.xaml
    /// </summary>
    public partial class StarControl : UserControl {
        public StarControl() {
            InitializeComponent();
        }

        public void SetFill(SolidColorBrush solidColorBrush) {
            polygon.Fill = solidColorBrush;
        }
    }
}
