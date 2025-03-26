using SafariView.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafariView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public event EventHandler<Point>? CanvasClick;

        public MainWindow(List<TileRender> renderedTiles)
        {
            InitializeComponent();
            tileCanvas.tiles = renderedTiles;
        }

        public void ShowRender()
        {
            tileCanvas.InvalidateVisual();
        }

        private void Canvas_MouseDown(object? sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(tileCanvas);
            CanvasClick?.Invoke(this,p);
        }
    }
}