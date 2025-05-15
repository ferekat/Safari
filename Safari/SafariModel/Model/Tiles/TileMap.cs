using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public TileMap()
        {

        }
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
        public bool IsTraversablePath(int x0, int y0, int x1, int y1,bool excludeGates)
        {
            Tile t0 = map[x0, y0];
            Tile t1 = map[x1, y1];
             //   Debug.WriteLine($"tr{t0.TileType},{t1.TileType},{x0},{y0}");
            if (excludeGates && (t0.IsGate() || t1.IsGate()))
            {
                return true;
            }
            if (Tile.OrderedTraversableHeights.Contains(t0.TileType) && Tile.OrderedTraversableHeights.Contains(t1.TileType))
            {
                int t0Type = Tile.OrderedTraversableHeights.IndexOf(t0.TileType);
                int t1Type = Tile.OrderedTraversableHeights.IndexOf(t1.TileType);
                return Math.Abs(t0Type - t1Type) <= 1;
            }
            return false;
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
     
    }
}
