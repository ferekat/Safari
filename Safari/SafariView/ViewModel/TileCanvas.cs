using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace SafariView.ViewModel
{
    public class TileCanvas : Canvas
    {
        
        public List<RenderObject>? tiles;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (tiles == null) return;
            foreach (RenderObject tr in tiles!)
            {
                dc.DrawRectangle(tr.Texture, null, tr.Rectangle);

            //    dc.DrawText(new FormattedText(tr.H.ToString(),CultureInfo.CurrentCulture,FlowDirection.RightToLeft, new Typeface("Verdana"), 20,new SolidColorBrush(Color.FromRgb(255,255,255))), new Point(tr.RX+40, tr.RY));
            }
        }
    }
}
