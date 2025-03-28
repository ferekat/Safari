using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class TileZ
    {
        private int z;
        public int Z { get { return z; } set { z = value; } }

        public TileZ(int z)
        {
            this.z = z;
        }
    }
}
