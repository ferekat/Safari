using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SafariModel.Model.Utils
{
    public class RoadNetworkHandler
    {

        private Tile entrance;
        private Tile exit;

        private List<Tile> roadNetwork = new();
        private List<Tile> shortestPathBetweenGates = new();

        private TileMap tileMap;
        public RoadNetworkHandler(TileMap tileMap)
        {
            this.tileMap = tileMap;
            entrance = tileMap.Map[1,5];
            exit = tileMap.Exit;
            AddToRoadNetwork(entrance,null);
        }
        private void AddToRoadNetwork(Tile tile,TileNode? parent)
        {
            tile.NetworkNode = parent;
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


            if (IsTileWithinBounds(tile.I, tile.J - 1) && tileMap.Map[tile.I, tile.J - 1].IsInRoadNetwork()) ret.Add(tileMap.Map[tile.I, tile.J - 1]);
            if (IsTileWithinBounds(tile.I, tile.J + 1) && tileMap.Map[tile.I, tile.J + 1].IsInRoadNetwork()) ret.Add(tileMap.Map[tile.I, tile.J + 1]);
            if (IsTileWithinBounds(tile.I + 1, tile.J) && tileMap.Map[tile.I + 1, tile.J].IsInRoadNetwork()) ret.Add(tileMap.Map[tile.I + 1, tile.J]);
            if (IsTileWithinBounds(tile.I - 1, tile.J) && tileMap.Map[tile.I - 1, tile.J].IsInRoadNetwork()) ret.Add(tileMap.Map[tile.I - 1, tile.J]);
            return ret;
        }
        private int TotalCost(Tile tile, int distance)
        {
            //manhattan táv (h) + költség (g)       A*: f = h + g
            return Math.Abs(tile.I - exit.I) + Math.Abs(tile.J - exit.J) + distance;
        }

        public void ShortestPathAStar()
        {
            shortestPathBetweenGates.Clear();

            PriorityQueue<(Tile, int), int> endPoints = new(); // pq <(végpont,végpont g távolsága),f összköltség>    A*: f = h + g
            endPoints.Enqueue((entrance, 0), TotalCost(entrance, 0));


            

            int it = 0;
            bool foundExit = false;
            while (endPoints.Count > 0)
            {

                Debug.WriteLine($"{it}: {endPoints.Count}");
                  
                (Tile, int) parentTileData = endPoints.Dequeue();
                Debug.WriteLine($"{it}: {endPoints.Count}");

                Tile parentTile = parentTileData.Item1;
            //    Debug.WriteLine($"parent: {parentTile.I} {parentTile.J}");
                TileNode parentNode = parentTile.NetworkNode!;
                int parentTileDistance = parentTileData.Item2;

                List<Tile> neighbourTiles = GetTileNeighbours(parentTile);

                int neighBourTileDistance = parentTileDistance++;
                Debug.WriteLine($"n: {neighbourTiles.Count}");
                for (int i = 0; i < neighbourTiles.Count; i++)
                {
                    Tile neighbour = neighbourTiles[i];
                    endPoints.Enqueue((neighbour, neighBourTileDistance), TotalCost(neighbour, neighBourTileDistance));
                    AddToRoadNetwork(neighbour,parentTile.NetworkNode);
                    if (neighbour == exit)
                    {
                        foundExit = true;
                        break;
                    }
                }

                if (foundExit)
                {
                    Debug.WriteLine("FE");
                    BackTrackShortestPath(exit);
                    return;
                }
                it++;
                Debug.WriteLine($"{it}end: {endPoints.Count}");

            }
        }
        private void BackTrackShortestPath(Tile tile)
        {
            
            shortestPathBetweenGates.Add(tile);
            tile.SetPlaceable( TilePlaceable.S);
            Tile next;
            if (tile.NetworkNode!.parent != null)
            {
                next = tile.NetworkNode!.parent.tile;
                BackTrackShortestPath(next);
            }
            else
            {
                return;
            }
            
        }


    }
}
