using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SafariView.ViewModel
{
    public class EntityRender : ViewModelBase
    {
        private int x;
        private int y;
        private Brush col;

        public int I { get { return x; } private set { x = value; OnPropertyChanged(); } }
        public int J { get { return y; } private set { y = value; OnPropertyChanged(); } }
        public Brush Col { get { return col; } private set { col = value; OnPropertyChanged(); } }
    }
}
