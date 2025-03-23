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
        private Brush col;

        public EntityRender(int x, int y, Brush tex)
        {
            Margin = new Thickness(x, y, 0, 0);
            Col = tex;
        }

        public Thickness Margin { get { return margin; } private set { margin = value; OnPropertyChanged(); } }
        public Brush Col { get { return col; } private set { col = value; OnPropertyChanged(); } }
    }
}
