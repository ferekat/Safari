using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SafariView.ViewModel
{
    public class EntityRender : ViewModelBase
    {
        private Thickness margin;
        private Brush? col;
        private int size;

        public EntityRender(int x, int y, Brush tex, int size)
        {
            Margin = new Thickness(x, y, 0, 0);
            Col = tex;
            Size = size;
        }
        public int Size { get { return size; }  private set { size = value; OnPropertyChanged(); } }
        public Thickness Margin { get { return margin; } private set { margin = value; OnPropertyChanged(); } }
        public Brush Col { get { return col!; } private set { col = value; OnPropertyChanged(); } }
    }
}
