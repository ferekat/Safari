using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class Tile
    {
        private static int TILESIZE = 50;
        private int i;
        private int j;
        private TileType tileType;
        private TileCondition tileCondition;
       

        public Tile(int i, int j)
        {
            this.i = i;
            this.j = j;
            tileType = TileType.EMPTY;
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
