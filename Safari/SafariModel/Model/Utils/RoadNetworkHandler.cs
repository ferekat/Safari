using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace SafariModel.Model.Utils
{

    public class RoadNetworkHandler
    {

        private PathTile entrance;
        private PathTile exit;
        private static bool foundShortestPath;


        private List<PathTile> roadNetwork = new();
        private static List<PathTile> shortestPathExitToEntrance = new();
        private static List<PathTile> shortestPathEntranceToExit = new();
        public static List<PathTile> ShortestPathExitToEntrance { get { return shortestPathExitToEntrance; } }
        public static bool FoundShortestPath { get { return foundShortestPath; } }
        public static List<PathTile> ShortestPathEntranceToExit { get { return shortestPathEntranceToExit; } }

        private TileMap tileMap;
        public RoadNetworkHandler(TileMap tileMap)
        {
            this.tileMap = tileMap;
            entrance = tileMap.Entrance;
            exit = tileMap.Exit;
            entrance.IntersectionNode!.Distance = 0;
            foundShortestPath = false;
        }




        public bool ConnectToNetwork(Tile tileToConnect, PathTileType pathToConnect)
        {
            if (tileToConnect is PathTile)
            {
                return false;
            }
            int pathNeighbours = 0;
            List<PathIntersectionNode> neighNodes = new();
            PathTile connectedTile = new PathTile(tileToConnect, pathToConnect, new PathIntersectionNode(tileToConnect.I, tileToConnect.J));
            PathIntersectionNode neighNode = null!;
            PathIntersectionNode connectedTileNode = connectedTile.IntersectionNode!;
            bool succ = false;
            foreach (Tile neigh in tileMap.GetNeighbourTiles(tileToConnect))  //HEGYEK
            {
                if (neigh is PathTile neighPathTile)
                {
                    succ = true;
                   
                    switch (neighPathTile.PathType) //HIDAK
                    {
                        case PathTileType.SMALL_BRIDGE_VERT or PathTileType.SMALL_BRIDGE_HOR or PathTileType.SMALL_BRIDGE_DR or PathTileType.SMALL_BRIDGE_DL or PathTileType.SMALL_BRIDGE_UR or PathTileType.SMALL_BRIDGE_UL:
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_VERT) return false;
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_HOR) return false;
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_DR) return false;
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_DL) return false;
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_UR) return false;
                            if (pathToConnect != PathTileType.SMALL_BRIDGE_UL) return false;
                            break;
                        case PathTileType.LARGE_BRIDGE_VERT or PathTileType.LARGE_BRIDGE_HOR or PathTileType.LARGE_BRIDGE_U or PathTileType.LARGE_BRIDGE_D:
                            if (pathToConnect != PathTileType.LARGE_BRIDGE_VERT) return false;
                            if (pathToConnect != PathTileType.LARGE_BRIDGE_HOR) return false;
                            if (pathToConnect != PathTileType.LARGE_BRIDGE_D) return false;
                            if (pathToConnect != PathTileType.LARGE_BRIDGE_U) return false;
                            break;
                        default:
                            break;
                    }

                    roadNetwork.Add(connectedTile);


                   

                    if (neighPathTile.IntersectionNode == null)
                    {
                      
                        neighPathTile.IntersectionNode = new PathIntersectionNode(neighPathTile.I, neighPathTile.J);
                        int deltaI = Math.Abs(connectedTile.I - neighPathTile.I);
                        int deltaJ = Math.Abs(neighPathTile.J - neighPathTile.J);

                        PathIntersectionNode[] closestNodes = ClosestNodesOnAxis(neighPathTile, deltaI, deltaJ);
                      
                        PathIntersectionNode.ConnectIntersections(neighPathTile.IntersectionNode, closestNodes[0]);
                        PathIntersectionNode.ConnectIntersections(neighPathTile.IntersectionNode, closestNodes[1]);
                        PathIntersectionNode.DisconnectIntersections(closestNodes[0], closestNodes[1]);

                    }

                    neighNode = neighPathTile.IntersectionNode;
                    neighNodes.Add(neighNode);
                    PathIntersectionNode.ConnectIntersections(connectedTileNode, neighNode);








                    pathNeighbours++;
                }
            }
            if (succ)
            {
                tileMap.Map[tileToConnect.I, tileToConnect.J] = connectedTile;
                neighNodes.Add(connectedTileNode);
                foreach(PathIntersectionNode node in neighNodes)
                {
                    SimplifyStraightPath(node);
                }
                if (pathNeighbours >= 2)
                {
                    ShortestPathAStar();
                }



                ///debug
                //foreach (PathTile pt in roadNetwork)
                //{
                //    pt.PathType = PathTileType.EMPTY;
                //}
                
                foreach (PathIntersectionNode node in PathIntersectionNode.inst)
                {
                    
                    if (tileMap.Map[node.PathI, node.PathJ] is PathTile pt)
                    {

                   //     pt.PathType = PathTileType.EMPTY;
                    }
                  
                }
            }
           
           
          
            return pathNeighbours > 0;
        }
        private int CalculateDistance(PathIntersectionNode node)
        {
            if (node.NextIntersections.Count == 0)
            {
                return TileMap.MAPSIZE; 
            }

           
            int minDistance = node.NextIntersections[0].Distance;

        
            foreach (PathIntersectionNode neigh in node.NextIntersections)
            {
                int distance;
            
                if (neigh.PathI != node.PathI)
                {
                    distance = Math.Abs(neigh.PathJ - node.PathJ); 
                }
                else
                {
                    distance = Math.Abs(neigh.PathI - node.PathI); 
                }

               
                if (distance + neigh.Distance < minDistance)
                {
                    minDistance = distance + neigh.Distance;
                }
            }

           
            node.Distance = minDistance;

            return minDistance; 
        }

        private PathIntersectionNode[] ClosestNodesOnAxis(PathTile neighPathTile, int axisI, int axisJ)
        {
            PathIntersectionNode[] ret = new PathIntersectionNode[2];
            if (axisI != 0)
            {
                for (int j = neighPathTile.J + 1; j < TileMap.MAPSIZE; j++)
                {
                    if (tileMap.Map[neighPathTile.I, j] is PathTile pt && pt.IntersectionNode != null)
                    {
                        ret[0] = pt.IntersectionNode;
                        break;
                    }
                }
                for (int j = neighPathTile.J - 1; j >= 0; j--)
                {
                    if (tileMap.Map[neighPathTile.I, j] is PathTile pt && pt.IntersectionNode != null)
                    {
                        ret[1] = pt.IntersectionNode;
                        break;
                    }
                }
            }
            else
            {
                for (int i = neighPathTile.I + 1; i < TileMap.MAPSIZE; i++)
                {
                    if (tileMap.Map[i, neighPathTile.J] is PathTile pt && pt.IntersectionNode != null)
                    {
                        ret[0] = pt.IntersectionNode;
                        break;
                    }
                }
                for (int i = neighPathTile.I - 1; i >= 0; i--)
                {
                    if (tileMap.Map[i, neighPathTile.J] is PathTile pt && pt.IntersectionNode != null)
                    {
                        ret[1] = pt.IntersectionNode;
                        break;
                    }
                }
            }

            return ret;
        }
        private void SimplifyStraightPath(PathIntersectionNode node) //node szomszédai egyenes utat alkotnak e 
        {


            List<PathIntersectionNode> neighs = node.NextIntersections;

            if (neighs.Count != 2)
            {
                return;
            }



            if (neighs[0].PathI == neighs[1].PathI || neighs[0].PathJ == neighs[1].PathJ)
            {

                PathIntersectionNode neigh0 = neighs[0];
                PathIntersectionNode neigh1 = neighs[1];
                PathIntersectionNode.ConnectIntersections(neigh0, neigh1);
                PathIntersectionNode.DisconnectIntersections(neigh0, node);
                PathIntersectionNode.DisconnectIntersections(neigh1, node);
                PathTile pt = (PathTile)tileMap.Map[node.PathI, node.PathJ];
                pt.IntersectionNode = null;
                SimplifyStraightPath(neigh1);
                SimplifyStraightPath(neigh0);


            }

            return;


        }
      



        private int TotalCost(Tile tile, int distance)
        {
            //manhattan táv (h) + költség (g)       A*: f = h + g
            return Math.Abs(tile.I - exit.I) + Math.Abs(tile.J - exit.J) + distance;
        }

        public void ShortestPathAStar()
        {
          
            shortestPathEntranceToExit.Clear();
            shortestPathExitToEntrance.Clear();

            // Reset distances and visited flags
            foreach (var node in PathIntersectionNode.inst)
            {
                node.Distance = int.MaxValue;
                node.IsVisited = false;
            }

            entrance.IntersectionNode!.Distance = 0;

            var priorityQueue = new PriorityQueue<PathIntersectionNode, int>();
            priorityQueue.Enqueue(entrance.IntersectionNode, TotalCost(entrance, 0));
            
            Dictionary<PathIntersectionNode, PathIntersectionNode?> cameFrom = new();
            cameFrom[entrance.IntersectionNode] = null;
            while (priorityQueue.Count > 0)
            {
                PathIntersectionNode current = priorityQueue.Dequeue();

                if (current.IsVisited) continue;
                current.IsVisited = true;

                if (current.PathI == exit.I && current.PathJ == exit.J)
                {
                    ReconstructPath(current, cameFrom);
                    return;
                }

                foreach (PathIntersectionNode neighbor in current.NextIntersections)
                {
                    if (neighbor.IsVisited) continue;

                    int moveCost = Math.Abs(neighbor.PathI - current.PathI) + Math.Abs(neighbor.PathJ - current.PathJ);
                    int tentativeG = current.Distance + moveCost;

                    if (tentativeG < neighbor.Distance)
                    {
                        neighbor.Distance = tentativeG;
                        cameFrom[neighbor] = current;

                        int fCost = TotalCost(tileMap.Map[neighbor.PathI, neighbor.PathJ], neighbor.Distance);
                        priorityQueue.Enqueue(neighbor, fCost);
                    }
                }
            }

            // No path found
            Debug.WriteLine("No path found from entrance to exit.");
        }

        private void ReconstructPath(PathIntersectionNode endNode, Dictionary<PathIntersectionNode, PathIntersectionNode?> cameFrom)
        {
            var current = endNode;
            foundShortestPath = true;
            while (true)
            {
                if (tileMap.Map[current.PathI, current.PathJ] is PathTile pt)
                {
                  
                    shortestPathEntranceToExit.Add(pt);
                    shortestPathExitToEntrance.Add(pt);
                }
                if (cameFrom[current] != null)
                {
                    current = cameFrom[current];
                }
                else
                {
                    shortestPathEntranceToExit.Add(entrance);
                    shortestPathExitToEntrance.Add(entrance);
                    break;
                }
            }
            
           
            shortestPathEntranceToExit.Reverse();
        }



        }
    }
