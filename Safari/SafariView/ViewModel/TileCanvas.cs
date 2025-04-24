using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

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
            }
        }
    }
}
