using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Tile
    {
        private static int TILESIZE = 50;
        private int i;
        private int j;
        private TileType tileType;
        private bool isRoad;
        private bool isOccupied;

        public Tile(int i, int j)
        {
            this.i = i;
            this.j = j;
            tileType = TileType.EMPTY;
            isRoad = false;
            isOccupied = false;
        }

        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }
        public void BuildRoad()
        {
            isRoad = true;
        }
        public void BuildLake()
        {
            SetType(TileType.WATER);
        }
        public void Occupy()
        {
            this.isOccupied = true;
        }
        public void Release()
        {
            this.isOccupied = false;
        }


    }
}
