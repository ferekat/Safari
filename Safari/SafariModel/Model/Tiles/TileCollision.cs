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
            if (x < 0 || x >= tileMap.GetLength(0) || y < 0 || y >= tileMap.GetLength(1)) return false;
            if(tileMap[x, y].Placeable == TilePlaceable.IS_LARGE_BRIDGE) return true;
            return tileMap[x, y].Type != TileType.WATER && tileMap[x, y].Type != TileType.FENCE;
        }

        public int GetTileWeight(int x, int y)
        {
            //mennyire "nehéz" átkelni egy adott tile-on (pl. hilleken lassabban lehet közlekedni), a pathfindinghoz
            //Nincs még implementálva!!!
            return 1;
        }
    }
}
