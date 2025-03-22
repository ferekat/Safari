using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{

    public class HillTile : Tile
    {
        private static readonly int MAX_ZHEIGHT = 20;
        private struct ZPoint
        {
            private int ZHeight;
            private int ZWidth;
        }
        private List<ZPoint> zMap = new();

        public HillTile(int i ,int j) : base(i,j)
        {
            
        }

    }
}
