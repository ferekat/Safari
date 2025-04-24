using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class TileCollision
    {
        private TileMap tileMap;

        public TileMap TileMap { get { return tileMap; } }
        public TileCollision(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        public bool IsPassable(int x, int y)
        {
            if (x < 0 || x >= TileMap.MAPSIZE || y < 0 || y >= TileMap.MAPSIZE) return false;
            if(tileMap.Map[x, y] is PathTile) return true;
            if (tileMap.Map[x, y].Type == TileType.FENCE) return false;
            return tileMap.Map[x, y] is PathTile p && p.PathType == PathTileType.ROAD;

           
        }

        public int GetTileWeight(int x, int y)
        {
            //mennyire "nehéz" átkelni egy adott tile-on (pl. hilleken lassabban lehet közlekedni), a pathfindinghoz
            //Nincs még implementálva!!!
            return 1;
        }
    }
}
