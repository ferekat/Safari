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
        private PathTile entrance;
        private PathTile exit;

        public Tile[,] Map { get { return map; } }
        public PathTile Entrance { get { return entrance; } }
        public PathTile Exit { get { return exit; } }


        public TileMap(Tile[,] map, PathTile entrance, PathTile exit)
        {
            this.map = map;
            this.entrance = entrance;
            this.exit = exit;

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
        public static TileMap CreateMapTmp()
        {
            using (StreamReader sr = new StreamReader("TestMap.txt"))
            {

                Tile[,] map = new Tile[100, 100];
                for (int i = 0; i < 100; i++)
                {
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            char c = line[j];
                            map[i, j] = new Tile(i, j, 0, dict[c]);
                        }
                    }
                }
                for (int i = 0; i < 100; i++)
                {
                    map[i, 0].SetType(TileType.FENCE);
                    map[i,99].SetType(TileType.FENCE);
                }
                    for (int j = 0;j < 100; j++)
                    {
                    map[0,j].SetType(TileType.FENCE);
                    map[99, j].SetType(TileType.FENCE);
                    }
                map[0, 5].SetType(TileType.ENTRANCE);
                map[5, 0].SetType(TileType.EXIT);

                map[0, 5] = new PathTile(map[0, 5], PathTileType.ROAD, new PathIntersectionNode(0, 5));
                map[5, 0] = new PathTile(map[5, 0], PathTileType.ROAD, new PathIntersectionNode(5, 0));




            return new TileMap(map, (PathTile)map[0, 5], (PathTile)map[5, 0]);
            }
        }
    }
}
