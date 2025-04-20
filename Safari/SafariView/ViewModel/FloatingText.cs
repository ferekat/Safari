using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SafariView.ViewModel
{
    public class FloatingText : ViewModelBase
    {
        private Thickness margin;
        private string text;

        public Thickness Margin { get { return margin; } set { margin = value; OnPropertyChanged(); } }
        public string Text { get { return text; } set { text = value; OnPropertyChanged(); } }

        public FloatingText(string text, int x, int y)
        {
            this.text = text;
            Margin = new Thickness(x, y, 0, 0);
        }
    }
}