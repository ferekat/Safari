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
    public class RenderObject : ViewModelBase
    {
        private Rect rect;
        private Brush texture;

        private int h;
        private double x;
        private double y;
        public int H { get { return h; } }
        public double RX { get { return x; } }
        public double RY { get { return y; } } 


        public Rect Rectangle {  get { return rect; } }
        public Brush Texture { get { return texture; } }

        public RenderObject(double x, double y, double size, Brush tex,int h)
        {
            rect = new Rect(x, y, size, size);
            texture = tex;
            
            this.h = h;
            this.y = y;
            this.x = x;
        }
    }
}
