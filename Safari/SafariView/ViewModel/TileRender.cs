using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SafariView.ViewModel
{
    public class TileRender : ViewModelBase
    {
        private Rect rect;
        private Brush texture;

        public Rect Rectangle {  get { return rect; } }
        public Brush Texture { get { return texture; } }

        public TileRender(double x, double y, double size, Brush tex)
        {
            rect = new Rect(x, y, size, size);
            texture = tex;
        }
    }
}
