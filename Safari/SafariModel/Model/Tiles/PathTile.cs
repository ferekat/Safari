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
        BRIDGE,

        
        //SMALL_BRIDGE,
        //SMALL_BRIDGE_VERT,
        //SMALL_BRIDGE_HOR,
        //SMALL_BRIDGE_DR,
        //SMALL_BRIDGE_DL,
        //SMALL_BRIDGE_UR,
        //SMALL_BRIDGE_UL,
        //LARGE_BRIDGE_VERT,
        //LARGE_BRIDGE_HOR,
        //LARGE_BRIDGE_D,
        //LARGE_BRIDGE_U,

    }
    public class PathIntersectionNode
    {
        private int id;
        private bool isVisited;
        private int pathI;
        private int pathJ;
        private List<PathIntersectionNode> nextIntersections = new();
        private int distance;
        public readonly static List<PathIntersectionNode> allNodes = new List<PathIntersectionNode>();

        private static int CurrentID = 0;

        public int ID { get { return id; } }
        public int PathI { get { return pathI; } }
        public bool IsVisited { get { return isVisited; } set { isVisited = value; } }
        public int PathJ { get { return pathJ; } }
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
            isVisited = false;
        }

        public PathIntersectionNode(int id,int pathI, int pathJ, int distance, bool isVisited)
        {
            this.id = id;
            if (id >= CurrentID)
                CurrentID = id + 1;
            this.pathI = pathI;
            this.pathJ = pathJ;
            this.distance = distance;
            this.isVisited = isVisited;
            nextIntersections = new List<PathIntersectionNode>();
        }

    }
    public class PathTile : Tile
    {
 


        private PathTileType pathType;
        private PathTileType cachedType;
        private PathIntersectionNode? intersectionNode;

        public PathTileType CachedType { get { return cachedType; } }
        public PathTileType PathType { get { return pathType; } /*set {/*if (intersectionNode == null) pathType = cachedType; else { pathType = PathTileType.NODE; } } */}
      
        public PathIntersectionNode? IntersectionNode { get { return intersectionNode; } set { intersectionNode = value; } }
        public PathTile(Tile t,PathTileType pathType) : base(t.I, t.J, t.H,t.TileType)
        //akkor jön létre a PathTile amikor leteszünk egy utat/hidat
        public PathTile(Tile t,PathTileType pathType,PathIntersectionNode intersectionNode) : base(t.I, t.J, t.H,TileType.EMPTY)
        {
            this.pathType = pathType;
            cachedType = pathType;
            this.intersectionNode = new PathIntersectionNode(t.I, t.J);
    
       
        }
        public readonly static Dictionary<string, PathTileType> pathTileShopMap = new Dictionary<string, PathTileType>()
        {
            {"Road",PathTileType.ROAD},
            {"Bridge",PathTileType.BRIDGE }
        };
    }
    
}
