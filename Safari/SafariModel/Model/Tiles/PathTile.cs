using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
  
    public enum PathTileType
    {
        EMPTY,
        ROAD,
        BRIDGE
    }
    public class PathIntersectionNode
    {
        public readonly static List<PathIntersectionNode> allNodes = new List<PathIntersectionNode>();
       
        private static int CurrentID = 0;
        
        private bool isVisitedByAStar;
        private int shortestPathId;
        private int id;
        private int pathI;
        private int pathJ;
        private List<PathIntersectionNode> nextIntersections = new();
        private int distance;


        public int ID { get { return id; } }
        public int PathI { get { return pathI; } }
        public int PathJ { get { return pathJ; } }
        public int ShortestPathId { get { return shortestPathId; } set { shortestPathId = value; } }
   
        public bool IsVisitedByAStar { get { return isVisitedByAStar; } set { isVisitedByAStar = value; } }
      
        public List<PathIntersectionNode> NextIntersections { get { return nextIntersections; } }
        public int Distance { get { return distance; } set { distance = value; } }
        public static void ConnectIntersections(PathIntersectionNode intersect1, PathIntersectionNode intersect2)
        {
            intersect1.nextIntersections.Add(intersect2);
            intersect2.nextIntersections.Add(intersect1);
            
        }
        public static void DisconnectIntersections(PathIntersectionNode intersect1, PathIntersectionNode intersect2)
        {
           
            intersect1.nextIntersections.Remove(intersect2);
            intersect2.nextIntersections.Remove(intersect1);
            if (intersect1.NextIntersections.Count == 0)
            {

                allNodes.Remove(intersect1);
            }
            if (intersect2.NextIntersections.Count == 0)
            {
                allNodes.Remove(intersect2);
            }
        }


        

        public void ConnectIntersection(PathIntersectionNode other)
        {
            nextIntersections.Add(other);
        }


        public PathIntersectionNode(int pathI, int pathJ)
        {
            this.id = CurrentID++;
            this.pathI = pathI;
            this.pathJ = pathJ;
            allNodes.Add(this);
            isVisitedByAStar = false;
            shortestPathId = 0;
     
        }

       
      
  

        public PathIntersectionNode(int id,int pathI, int pathJ, int distance,int shortestPathId, bool isVisitedByAStar)
        {
            this.id = id;
            if (id >= CurrentID)
                CurrentID = id + 1;
            this.pathI = pathI;
            this.pathJ = pathJ;
            this.distance = distance;
            this.shortestPathId = shortestPathId;
            this.isVisitedByAStar = isVisitedByAStar;
            nextIntersections = new List<PathIntersectionNode>();
        }

    }
    public class PathTile : Tile
    {
 


        private PathTileType pathType;
    
        private PathIntersectionNode? intersectionNode;

     
        public PathTileType PathType { get { return pathType; } /*set {/*if (intersectionNode == null) pathType = cachedType; else { pathType = PathTileType.NODE; } } */}
      
        public PathIntersectionNode? IntersectionNode { get { return intersectionNode; } set { intersectionNode = value; } }
        public PathTile(Tile t,PathTileType pathType) : base(t.I, t.J, t.H, TileType.EMPTY)
        {
            this.pathType = pathType;
            this.intersectionNode = new PathIntersectionNode(t.I, t.J);
        }
        //akkor jön létre a PathTile amikor leteszünk egy utat/hidat
        public readonly static Dictionary<string, PathTileType> pathTileShopMap = new Dictionary<string, PathTileType>()
        {
            {"Road",PathTileType.ROAD},
            {"Bridge",PathTileType.BRIDGE }
        };
    }
    
}
