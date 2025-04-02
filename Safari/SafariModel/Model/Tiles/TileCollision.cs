using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class TileCollision
    {
        private Tile[,] tileMap;

        public TileCollision(Tile[,] tileMap)
        {
            this.tileMap = tileMap;
        }

        public bool IsPassable(int x, int y)
        {
            return tileMap[x, y].Type != TileType.WATER;
        }
    }
}
