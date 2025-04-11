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
        private static List<Tile> shortestPathEntranceToExit = new();

        public static List<Tile> ShortestPathEntranceToExit { get { return shortestPathEntranceToExit; } }
        private TileMap tileMap;
        public RoadNetworkHandler(TileMap tileMap)
        {
            this.tileMap = tileMap;
            entrance = tileMap.Entrance;
            exit = tileMap.Exit;
            AddPathNode(entrance,null);
            ShortestPathAStar();
        }
        public void AddToRoadNetwork(Tile tile)
        {
            roadNetwork.Add(tile);
        }
        private void AddPathNode(Tile tile,TileNode? parent)
        {
            tile.NetworkNode = new TileNode(tile,parent);
        }
        private bool IsTileCoordInBounds(int i,int j)
        {
            return i >= 0 &&
                   i < TileMap.MAPSIZE &&
                   j >= 0 &&
                   j < TileMap.MAPSIZE;
        }
        private bool IsValidNeighbour(Tile tile)
        {
            return tile.IsInRoadNetwork() &&
                    !tile.VisitedRoad!.IsVisited;
                    
        }
        private List<Tile> GetTileNeighbours(Tile tile)
        {
            List<Tile> ret = new List<Tile>();

            if (IsTileCoordInBounds(tile.I, tile.J-1) && IsValidNeighbour(tileMap.Map[tile.I, tile.J - 1]))
            {
                ret.Add(tileMap.Map[tile.I, tile.J - 1]);
            }
            if (IsTileCoordInBounds(tile.I, tile.J+1) && IsValidNeighbour(tileMap.Map[tile.I, tile.J + 1]))
            {
                ret.Add(tileMap.Map[tile.I, tile.J + 1]);
            }
            if (IsTileCoordInBounds(tile.I-1, tile.J) && IsValidNeighbour(tileMap.Map[tile.I-1, tile.J ]))
            {
                ret.Add(tileMap.Map[tile.I-1, tile.J ]);
            }
            if (IsTileCoordInBounds(tile.I+1, tile.J) && IsValidNeighbour(tileMap.Map[tile.I+1, tile.J ]))
            {
                ret.Add(tileMap.Map[tile.I+1, tile.J ]);
            }
         
            return ret;
        }
        private int TotalCost(Tile tile, int distance)
        {
            //manhattan táv (h) + költség (g)       A*: f = h + g
            return Math.Abs(tile.I - exit.I) + Math.Abs(tile.J - exit.J) + distance;
        }

        public void ShortestPathAStar()
        {
            shortestPathEntranceToExit.Clear();

            PriorityQueue<(Tile, int), int> endPoints = new(); // pq <(végpont,végpont g távolsága),f összköltség>    A*: f = h + g
            endPoints.Enqueue((entrance, 0), TotalCost(entrance, 0));

            int it = 0;
            bool foundExit = false;
            while (endPoints.Count > 0)
            {
                  
                (Tile, int) parentTileData = endPoints.Dequeue();
                Tile parentTile = parentTileData.Item1;
                TileNode parentNode = parentTile.NetworkNode!;
               

                parentTile.VisitedRoad!.IsVisited = true;
               
          
                int parentTileDistance = parentTileData.Item2;

                List<Tile> neighbourTiles = GetTileNeighbours(parentTile);

                int neighBourTileDistance = parentTileDistance++;
               
                for (int i = 0; i < neighbourTiles.Count; i++)
                {
                   
                    Tile neighbour = neighbourTiles[i];
                    endPoints.Enqueue((neighbour, neighBourTileDistance), TotalCost(neighbour, neighBourTileDistance));
                    AddPathNode(neighbour,parentTile.NetworkNode);
                    if (neighbour == exit)
                    {
                        foundExit = true;
                        break;
                    }
                }

                if (foundExit)
                {
                    foreach (Tile road in roadNetwork)
                    {
                        road.SetPlaceable(TilePlaceable.IS_ROAD);
                    }
                    BackTrackShortestPath(exit);
                    return;
                }
                it++;
            }

            foreach (Tile road in roadNetwork)
            {
                road.VisitedRoad!.IsVisited = false;
            }
        }
        private void BackTrackShortestPath(Tile tile)
        {
           
            shortestPathEntranceToExit.Add(tile);
            tile.SetPlaceable(TilePlaceable.S);
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
