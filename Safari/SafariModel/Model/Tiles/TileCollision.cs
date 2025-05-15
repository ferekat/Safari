using SafariModel.Model.AbstractEntity;
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
                if (tile is PathTile)
                {
                    return 1;
                }
            }
            else
            {
                weight = tile.H * 10;
            }
            return weight;
            //mennyire "nehéz" átkelni egy adott tile-on (pl. hilleken lassabban lehet közlekedni), a pathfindinghoz

            return 1;
        }
        public float GetTileSpeed(int x, int y)
        {
            if (TileMap.IsTileCoordInBounds(x, y))
            {
                Tile tile = tileMap.Map[x, y];
                float speed;
                float baseSpeed = 5.5f;
                float minSpeed = 0.1f;
                int minHeight = 1;
                int maxHeight = 600;
                if (tile.H == 0)
                {
                    return baseSpeed;
                }
                if (tile.IsWater())
                {
                    speed = 2f;
                    if (tile is PathTile)
                    {
                        return baseSpeed;
                    }
                }
                else
                {
                    speed = baseSpeed+ ((tile.H -minHeight)*(minSpeed-baseSpeed))/(maxHeight-minHeight);
                }
                return speed;
            }
            return 1f;
        }
    }
}
