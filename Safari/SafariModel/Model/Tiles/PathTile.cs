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

        
        SMALL_BRIDGE,
        SMALL_BRIDGE_VERT,
        SMALL_BRIDGE_HOR,
        SMALL_BRIDGE_DR,
        SMALL_BRIDGE_DL,
        SMALL_BRIDGE_UR,
        SMALL_BRIDGE_UL,
        LARGE_BRIDGE_VERT,
        LARGE_BRIDGE_HOR,
        LARGE_BRIDGE_D,
        LARGE_BRIDGE_U,

    }
    public class PathIntersectionNode
    {
        private bool isVisited;
        private int pathI;
        private int pathJ;
        private List<PathIntersectionNode> nextIntersections = new();
        private int distance;
       
        public readonly static List<PathIntersectionNode> allNodes = new List<PathIntersectionNode>();
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
       
        public PathIntersectionNode(int pathI, int pathJ)
        {
         
            this.pathI = pathI;
            this.pathJ = pathJ;
            allNodes.Add(this);
            isVisited = false;
          
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

        //akkor jön létre a PathTile amikor leteszünk egy utat/hidat
        public PathTile(Tile t,PathTileType pathType) : base(t.I, t.J, t.H,t.TileType)
        {
            this.pathType = pathType;
            cachedType = pathType;
            this.intersectionNode = new PathIntersectionNode(t.I, t.J);
            
        }
        public void SetType(PathTileType pathType)
        {
            this.pathType = pathType;
        }
        public static bool CanPlacePath(PathTileType pathType,TileType on)
        {
            switch(pathType)
            {
                case PathTileType.ROAD:
                    if (on == TileType.DEEP_WATER ||on == TileType.SHALLOW_WATER) return false;
                    return true;
                case PathTileType.SMALL_BRIDGE_VERT or PathTileType.SMALL_BRIDGE_HOR or PathTileType.SMALL_BRIDGE_DR or PathTileType.SMALL_BRIDGE_DL or PathTileType.SMALL_BRIDGE_UL or PathTileType.SMALL_BRIDGE_UR:
                    if (on == TileType.SHALLOW_WATER) return true;
                    return false;
                case PathTileType.LARGE_BRIDGE_HOR or PathTileType.LARGE_BRIDGE_VERT or PathTileType.LARGE_BRIDGE_D or PathTileType.LARGE_BRIDGE_U :
                    if (on == TileType.DEEP_WATER || on == TileType.SHALLOW_WATER) return true;
                    return false;
                default:
                    return false;
            }
        }
        public readonly static Dictionary<PathTileType, TileType> pathTileInteractionMap = new Dictionary<PathTileType, TileType>()
        {
            { PathTileType.ROAD,TileType.GROUND},
            { PathTileType.SMALL_BRIDGE_HOR,TileType.SHALLOW_WATER},
            { PathTileType.SMALL_BRIDGE_VERT,TileType.SHALLOW_WATER},
            { PathTileType.SMALL_BRIDGE_DR,TileType.SHALLOW_WATER},
            { PathTileType.SMALL_BRIDGE_DL,TileType.SHALLOW_WATER},
            { PathTileType.SMALL_BRIDGE_UR,TileType.SHALLOW_WATER},
            { PathTileType.SMALL_BRIDGE_UL,TileType.SHALLOW_WATER},
            { PathTileType.LARGE_BRIDGE_HOR,TileType.DEEP_WATER},
            { PathTileType.LARGE_BRIDGE_VERT,TileType.DEEP_WATER},
            { PathTileType.LARGE_BRIDGE_U,TileType.DEEP_WATER},
            { PathTileType.LARGE_BRIDGE_D,TileType.DEEP_WATER},
        };
        public readonly static Dictionary<string, PathTileType> pathTileShopMap = new Dictionary<string, PathTileType>()
        {
            {"Road",PathTileType.ROAD},
            {"LargeBridge_hor",PathTileType.LARGE_BRIDGE_HOR},
            {"LargeBridge_vert",PathTileType.LARGE_BRIDGE_VERT},
            {"LargeBridge_u",PathTileType.LARGE_BRIDGE_U},
            {"LargeBridge_d",PathTileType.LARGE_BRIDGE_D},
            {"SmallBridge_hor",PathTileType.SMALL_BRIDGE_HOR},
            {"SmallBridge_vert",PathTileType.SMALL_BRIDGE_VERT},
            {"SmallBridge_dr",PathTileType.SMALL_BRIDGE_DR},
            {"SmallBridge_dl",PathTileType.SMALL_BRIDGE_DL},
            {"SmallBridge_ur",PathTileType.SMALL_BRIDGE_UR},
            {"SmallBridge_ul",PathTileType.SMALL_BRIDGE_UL},
        };
    }
    
}
