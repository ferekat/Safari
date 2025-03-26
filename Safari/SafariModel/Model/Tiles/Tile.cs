using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class Tile
    {
        public static readonly int TILESIZE = 50;
        private int i;
        private int j;
        private TileType tileType;
        private TileCondition tileCondition;

        public int I { get { return i; } }
        public int J { get { return j; } }

        public TileType Type { get { return tileType; } }


        public Tile(int i, int j)
        {
            this.i = i;
            this.j = j;
            tileType = TileType.GROUND;
            //Csak tesztelésre!!!
            Random r = new Random();
            tileType = r.Next(2) == 0 ? TileType.GROUND : TileType.HILL;
            if (i == 0 || i == Model.MAPSIZE - 1 || j == 0 || j == Model.MAPSIZE - 1) tileType = TileType.FENCE;
            //

            tileCondition = TileCondition.EMPTY;
        }

        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }
        public void SetCondition(TileCondition tileCondition)
        {
            this.tileCondition = tileCondition;
        }
        //public void BuildRoad()
        //{
        //    tileCondition = TileCondition.IS_ROAD;
        //}
        //public void BuildLake()
        //{
        //    SetType(TileType.WATER);
        //}
    }
}
