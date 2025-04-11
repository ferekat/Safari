using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SafariModel.Model.Utils
{
    public class RoadNetworkHandler
    {
       
        private Tile entrance;
        private Tile exit;

        private List<Tile> shortestPathBetweenGates;

        private Tile[,] tileMap;
        public RoadNetworkHandler(Tile[,] tileMap)
        {
            this.tileMap = tileMap;
        }
        private bool IsTileWithinBounds(int i, int j)
        {
            return i <= 0 &&
                j <= 0 &&
                i < TileMap.MAPSIZE &&
                j < TileMap.MAPSIZE;
        }
        private List<Tile> GetTileNeighbours(Tile tile)
        {
            List<Tile> ret = new List<Tile>();


            if (IsTileWithinBounds(tile.I, tile.J - 1) && tileMap[tile.I, tile.J - 1].IsInRoadNetwork()) ret.Add(tileMap[tile.I, tile.J - 1]);
            if (IsTileWithinBounds(tile.I, tile.J + 1) && tileMap[tile.I, tile.J + 1].IsInRoadNetwork()) ret.Add(tileMap[tile.I, tile.J + 1]);
            if (IsTileWithinBounds(tile.I + 1, tile.J) && tileMap[tile.I + 1, tile.J].IsInRoadNetwork()) ret.Add(tileMap[tile.I + 1, tile.J]);
            if (IsTileWithinBounds(tile.I - 1, tile.J) && tileMap[tile.I - 1, tile.J].IsInRoadNetwork()) ret.Add(tileMap[tile.I - 1, tile.J]);
            return ret;
        }
        private int Fn(Tile tile, int gn)
        {
            //manhattan táv + költség
            return Math.Abs(tile.I - exit.I) + Math.Abs(tile.J - exit.J) + gn;
        }

        private void ShortestPathAStar()
        {
            shortestPathBetweenGates.Clear();
            TileNode entranceNode = new TileNode(entrance, null);
            PriorityQueue<(Tile, int), int> endPoints = new(); // pq <(végpont,végpont g költsége),f összköltség>    A*: f = h + g
            endPoints.Enqueue((entrance, 0), Fn(entrance, 0));
            

            while (endPoints.Count > 0)
            {
                (Tile, int) tileData = endPoints.Dequeue();
                Tile tile = tileData.Item1;
                int tileGn = tileData.Item2;
                //TileNode currentTileNode = new TileNode(tile,)
                List<Tile> neighbourTiles = GetTileNeighbours(tile);

                int neighbourGn = tileGn++;
                for (int i = 0; i < neighbourTiles.Count; i++)
                {
                    Tile neighbour = neighbourTiles[i];
                    endPoints.Enqueue((neighbour, neighbourGn), Fn(neighbour, neighbourGn));
                    
                }


            }
        }


    }
}
