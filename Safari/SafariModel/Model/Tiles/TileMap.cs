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
        private Tile entrance;
        private Tile exit;

        public Tile[,] Map { get { return map; } }
        public Tile Entrance { get { return entrance; } }
        public Tile Exit { get { return exit; } }


        public TileMap(Tile[,] map,Tile entrance,Tile exit)
        {
            this.map = map;
            this.entrance = entrance;
            this.exit = exit;
            entrance.VisitedRoad = new VisitedRoad(false);
            exit.VisitedRoad = new VisitedRoad(false);
        }

        public static TileMap CreateMapTmp()
        {
            Tile[,] map = new Tile[100, 100];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    map[i, j] = new Tile(i, j, null);
                }
            }
            for (int i = 1; i < 30; i++)
            {
                for (int j = 1;j < 30; j++)
                {
                    map[i,j].SetType(TileType.GROUND);
                }
            }

            map[0, 5].SetType(TileType.ENTRANCE);
            map[5,0].SetType(TileType.EXIT);


            return new TileMap(map, map[0,5], map[5,0]);
        }
    }
}
