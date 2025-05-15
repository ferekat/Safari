using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace SafariModel.Model.Utils
{

    public class RoadNetworkHandler
    {

        private PathTile entrance = null!;
        private PathTile exit = null!;
        private bool foundShortestPath;
        private int SPNodeCount = 0;
        private List<PathTile> shortestPathExitToEntrance = new();
        private List<PathTile> shortestPathEntranceToExit = new();
      


        public PathTile Entrance { get { return entrance; } }
        public PathTile Exit { get { return exit; } }
        public bool FoundShortestPath { get { return foundShortestPath; } }
        public List<PathTile> ShortestPathExitToEntrance { get { return shortestPathExitToEntrance; } set { shortestPathExitToEntrance = value; } }
        public List<PathTile> ShortestPathEntranceToExit { get { return shortestPathEntranceToExit; } set { shortestPathEntranceToExit = value; } }
        public TileMap TileMap { get { return tileMap; } }  
        private TileMap tileMap;
        public RoadNetworkHandler(TileMap tileMap)
        {

            this.tileMap = tileMap;
            entrance = tileMap.Entrance;
            exit = tileMap.Exit;
            entrance.IntersectionNode!.Distance = 0;
            foundShortestPath = false;
            //Visszatöltéshez kell
            ShortestPathAStar();
        }
       
        public bool ConnectToNetwork(Tile tileToConnect, PathTileType pathToConnect)
        {
            if (tileToConnect is PathTile)
            {
                return false;
            }
            List<Tile> traversableTiles = new List<Tile>();
            foreach (Tile neigh in tileMap.GetNeighbourTiles(tileToConnect))
            {
                if(tileMap.IsTraversablePath(neigh.I, neigh.J, tileToConnect.I, tileToConnect.J, true))
                {
                    traversableTiles.Add(neigh);
                }
            }
           
            int pathNeighbours = 0;
            List<PathIntersectionNode> neighNodes = new();
            PathTile connectedTile = null!;
         
            
            PathIntersectionNode neighNode = null!;
            PathIntersectionNode connectedTileNode = null!;/* = connectedTile.IntersectionNode!;*/
            bool succ = false;
            foreach (Tile neigh in traversableTiles)  //HEGYEK HIDAK
            {
           // Debug.WriteLine("exe");
                if (neigh is PathTile neighPathTile)
                {
                    succ = true;
                    connectedTile = new PathTile(tileToConnect, pathToConnect);
                    connectedTileNode = connectedTile.IntersectionNode!;
                    if (neighPathTile.IntersectionNode == null)
                    {

                        neighPathTile.IntersectionNode = new PathIntersectionNode(neighPathTile.I, neighPathTile.J);
                 
                        
                        int deltaI = Math.Abs(connectedTile.I - neighPathTile.I);
                        int deltaJ = Math.Abs(neighPathTile.J - neighPathTile.J);

                        PathIntersectionNode[] closestNodes = ClosestNodesOnAxis(neighPathTile, deltaI, deltaJ);

                        PathIntersectionNode.ConnectIntersections(neighPathTile.IntersectionNode, closestNodes[0]);
                        PathIntersectionNode.ConnectIntersections(neighPathTile.IntersectionNode, closestNodes[1]);
                        PathIntersectionNode.DisconnectIntersections(closestNodes[0],closestNodes[1]);

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

                foreach (PathIntersectionNode node in neighNodes)
                {
                    SimplifyStraightPath( node);
                }
            
                if (pathNeighbours >= 2)
                {
                    ShortestPathEntranceToExit = ShortestPathAStar();
                    ShortestPathExitToEntrance = shortestPathEntranceToExit.AsEnumerable().Reverse().ToList();
                }
            }


            foreach (PathIntersectionNode n in PathIntersectionNode.allNodes)
            {
                Debug.WriteLine($"{n.PathI},{n.PathJ}");
            }
            Debug.WriteLine($"count: {PathIntersectionNode.allNodes.Count}");
            return pathNeighbours > 0;
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
        private void SimplifyStraightPath( PathIntersectionNode node) //node szomszédai egyenes utat alkotnak e 
        {


            List<PathIntersectionNode> neighs = node.NextIntersections;

            if (neighs.Count != 2)
            {
                //foreach (PathIntersectionNode n in neighs)
                //{
                //    roadNetwork.Add(n);
                //}
                return;
            }



            if (neighs[0].PathI == neighs[1].PathI || neighs[0].PathJ == neighs[1].PathJ)
            {
                
                PathIntersectionNode neigh0 = neighs[0];
                PathIntersectionNode neigh1 = neighs[1];
                PathIntersectionNode.ConnectIntersections(neigh0, neigh1);
                PathIntersectionNode.DisconnectIntersections(neigh0, node);
                PathIntersectionNode.DisconnectIntersections(neigh1, node);

                PathTile straightTile = (PathTile)tileMap.Map[node.PathI, node.PathJ];
                straightTile.IntersectionNode = null;
                
                SimplifyStraightPath(neigh1);
                SimplifyStraightPath(neigh0);


            }

            return;


        }




        private int ManhattanDist(int i0, int j0, int i1, int j1,int distance)
        {
            //manhattan táv (h) + költség (g)       A*: f = h + g
            return Math.Abs(i0 - i1) + Math.Abs(j0 - j1) + distance;
        }
       
        private List<PathTile> ShortestPathAStar()
        {

            
            // Reset distances and visited flags
           
            foreach (PathIntersectionNode node in PathIntersectionNode.allNodes)
            {
                node.Distance = int.MaxValue;               
                node.IsVisitedByAStar = false;
                node.ShortestPathId = 0;
            }
            SPNodeCount = 0;
            entrance.IntersectionNode!.Distance = 0;

            var priorityQueue = new PriorityQueue<PathIntersectionNode, int>();
            priorityQueue.Enqueue(entrance.IntersectionNode!, ManhattanDist(entrance.I,entrance.J,exit.I,exit.J, 0));

            Dictionary<PathIntersectionNode, PathIntersectionNode?> cameFrom = new();
            cameFrom[entrance.IntersectionNode] = null;
            while (priorityQueue.Count > 0)
            {
                PathIntersectionNode current = priorityQueue.Dequeue();

                if (current.IsVisitedByAStar) continue;
                current.IsVisitedByAStar = true;


                if (current.PathI == exit.I && current.PathJ == exit.J)
                {

                    return ReconstructPath(current, cameFrom);
                }

                foreach (PathIntersectionNode neighbour in current.NextIntersections)
                {
                    if (neighbour.IsVisitedByAStar) continue;

                    int moveCost = Math.Abs(neighbour.PathI - current.PathI) + Math.Abs(neighbour.PathJ - current.PathJ);
                    int tentativeG = current.Distance + moveCost;

                    if (tentativeG < neighbour.Distance)
                    {
                        neighbour.Distance = tentativeG;
                        cameFrom[neighbour] = current;

                        int fCost = ManhattanDist(entrance.I, entrance.J, exit.I, exit.J, neighbour.Distance);
                        priorityQueue.Enqueue(neighbour, fCost);
                    }
                }
            }



            // No path found
            Debug.WriteLine("No path found from entrance to exit.");
            
            return new List<PathTile>();
        }

        private List<PathTile> ReconstructPath(PathIntersectionNode endNode, Dictionary<PathIntersectionNode, PathIntersectionNode?> cameFrom)
        {
            List<PathTile> path = new List<PathTile>();
            PathIntersectionNode currentNode = endNode;
            foundShortestPath = true;
           
            while (true)
            {
  
                currentNode.ShortestPathId = ++SPNodeCount;
                


                if (tileMap.Map[currentNode.PathI, currentNode.PathJ] is PathTile pt)
                {
                    path.Add(pt);
                }
                if (cameFrom[currentNode] != null)
                {
                    currentNode = cameFrom[currentNode]!;
                }
                else
                {
                    break;
                }
            }
            path.Reverse();
            return path;
        }
       
        

       
    }
}
