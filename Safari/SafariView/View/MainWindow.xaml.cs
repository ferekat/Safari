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
        public event EventHandler<(int, int)>? CameraChange;

        public MainWindow(List<TileRender> renderedTiles)
        {
            InitializeComponent();
            tileCanvas.tiles = renderedTiles;
        }

        public void ShowTileMapRender()
        {
            tileCanvas.InvalidateVisual();
        }

        public void ViewModel_CameraChangeRequest(object? sender, (int,int) changerange)
        {
            int x = 0;
            int y = 0;
            Point p = Mouse.GetPosition(tileCanvas);
            if (p.X >= 0 && p.X < changerange.Item1) x = -1;
            if (p.Y >= 0 && p.Y < changerange.Item2) y = -1;
            if (p.X >= tileCanvas.ActualWidth - changerange.Item1 && p.X < tileCanvas.ActualWidth) x = 1;
            if (p.Y >= tileCanvas.ActualHeight - changerange.Item2 && p.Y < tileCanvas.ActualHeight) y = 1;

            OnCameraChange(x, y);
        }

        private void OnCameraChange(int x, int y)
        {
            CameraChange?.Invoke(this, (x, y));
        }

        private void Canvas_MouseDown(object? sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(tileCanvas);
            CanvasClick?.Invoke(this,p);
        }
    }
}