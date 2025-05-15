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

        public bool IsPassable(int x, int y, TileType[] imPassableTiles)
        {
            if (x < 0 || x >= TileMap.MAPSIZE || y < 0 || y >= TileMap.MAPSIZE) return false;
            return !imPassableTiles.ToList().Contains(TileMap.Map[x, y].TileType);
        }
        
        public int GetTileWeight(int x, int y)
        {
            Tile tile = tileMap.Map[x, y];
            int weight; 
            if (tile.IsWater())
            {
                weight = Tile.waterHeightMap[tile.TileType];
            }
            else
            {
                weight = tile.H/2;
            }
            return weight;
            //mennyire "nehéz" átkelni egy adott tile-on (pl. hilleken lassabban lehet közlekedni), a pathfindinghoz
            //Nincs még implementálva!!!
            return 1;
        }
    }
}
