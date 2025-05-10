using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class TileMap
    {

        public static readonly int MAPSIZE = 100;
        public static readonly int MAPCHUNKSIZE = 10;

        private Tile[,] map;
        private Tile[,][,]chunks;
        private PathTile entrance;
        private PathTile exit;

        public Tile[,] Map { get { return map; } }
        public PathTile Entrance { get { return entrance; } set { entrance = value; map[entrance.I, entrance.J] = entrance; } }
        public PathTile Exit { get { return exit; } set { exit = value; map[exit.I, exit.J] = exit; } }
        public Tile[,][,] Chunks { get { return chunks; } }

        public TileMap(Tile[,] map)
        {
            chunks = new Tile[MAPCHUNKSIZE,MAPCHUNKSIZE][,];
            for (int i = 0; i < MAPCHUNKSIZE; i++)
            {
                for (int j = 0; j < MAPCHUNKSIZE; j++)
                {
                    chunks[i,j] = new Tile[MAPCHUNKSIZE, MAPCHUNKSIZE];
                }
            }
            this.map = map;
          
            for (int chunkX = 0; chunkX < MAPCHUNKSIZE; chunkX++) 
            {
                for (int chunkY = 0; chunkY < MAPCHUNKSIZE; chunkY++)
                {
                    for (int i = 0; i < MAPCHUNKSIZE; i++) 
                    {
                        for (int j = 0; j < MAPCHUNKSIZE; j++) 
                        {
                           
                            int mapX = chunkX * MAPCHUNKSIZE + i;
                            int mapY = chunkY * MAPCHUNKSIZE + j;
                            chunks[chunkX,chunkY][i, j] = map[mapX, mapY];
                           
                        }
                    }
                }
            }
        }
        
        public static bool IsTileCoordInBounds(int i, int j)
        {
            return i >= 0 &&
                   i < MAPSIZE &&
                   j >= 0 &&
                   j < MAPSIZE;
        }
        public List<Tile> GetNeighbourTiles(Tile tile)
        {
            List<Tile> ret = new List<Tile>();

            if (IsTileCoordInBounds(tile.I, tile.J - 1))
            {
                ret.Add(map[tile.I, tile.J - 1]);
            }
            if (IsTileCoordInBounds(tile.I, tile.J + 1))
            {
                ret.Add(map[tile.I, tile.J + 1]);
            }
            if (IsTileCoordInBounds(tile.I - 1, tile.J))
            {
                ret.Add(map[tile.I - 1, tile.J]);
            }
            if (IsTileCoordInBounds(tile.I + 1, tile.J))
            {
                ret.Add(map[tile.I + 1, tile.J]);
            }

            return ret;
        }
        public int GetNeighbourTilesCount(Tile tile)
        {
            List<Tile> l = GetNeighbourTiles(tile);
            return l.Count;
        }

        //DEEP_WATER,
        //SHALLOW_WATER,
        //GROUND,
        //GROUND_SMALL,
        //SMALL_HILL,
        //SMALL_MEDIUM,
        //MEDIUM_HILL,
        //MEDIUM_HIGH,
        //HIGH_HILL,
        public static Dictionary<char, TileType> dict = new()
        {
            {'A',TileType.DEEP_WATER },
            {'B',TileType.SHALLOW_WATER },
            {'C',TileType.GROUND},
            {'D',TileType.GROUND_SMALL },
            {'E',TileType.SMALL_HILL},
            {'F',TileType.SMALL_MEDIUM},
            {'G',TileType.MEDIUM_HILL},
            {'H',TileType.MEDIUM_HIGH},
            {'I',TileType.HIGH_HILL}
        };
     
        
    }
}
