using SafariModel.Model.AbstractEntity;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Markup;

namespace SafariModel.Model.Tiles
{

    public enum TileType
    {
        EMPTY,


        DEEP_WATER,
        SHALLOW_WATER,
        GROUND,
        GROUND_SMALL,
        SMALL_HILL,
        SMALL_MEDIUM,
        MEDIUM_HILL,
        MEDIUM_HIGH,
        HIGH_HILL,

        ENTRANCE,
        EXIT,
        FENCE,




        GRASS_GROUND
    }

    public class Tile
    {
        public static readonly int TILESIZE = 50;

        private int i;
        private int j;
        private int h;

        private int[] heightBounds = { -200, -100, 0, 120, 250, 300 };


        private TileType tileType;

        //<Amit szeretnék venni,Ahova le tudom tenni>
        //public readonly static Dictionary<TileType, TileType> tileInteractionMap = new Dictionary<TileType, TileType>()
        //{       
        //    {TileType.GRASS_GROUND,TileType.GROUND},
        //    {TileType.SHALLOW_WATER,TileType.GROUND}
        //};

        public readonly static List<TileType> OrderedTraversableHeights = new List<TileType>()
        {
            TileType.SHALLOW_WATER,
            TileType.GROUND,
            TileType.GROUND_SMALL,
            TileType.SMALL_HILL,
            TileType.SMALL_MEDIUM,
            TileType.MEDIUM_HILL,
            TileType.MEDIUM_HIGH,
            TileType.HIGH_HILL
        };

        public readonly static Dictionary<TileType, int> waterHeightMap = new()
        {
            {TileType.DEEP_WATER,int.MaxValue},
            { TileType.SHALLOW_WATER,100},
        };


        public readonly static Dictionary<string, TileType> tileShopMap = new Dictionary<string, TileType>()
        {
            {"Lake",TileType.SHALLOW_WATER},
            {"Grass",TileType.GRASS_GROUND}
        };

        public int I { get { return i; } }
        public int J { get { return j; } }
        public int H { get { return h; } set { h = value; SetHeightType(h); } }

        public TileType TileType { get { return tileType; } }

        public Tile(int i, int j, int h, TileType type)
        {
            this.i = i;
            this.j = j;
            this.h = h;
            this.tileType = type;




        }
        public bool IsWater()
        {
            return tileType == TileType.SHALLOW_WATER || tileType == TileType.DEEP_WATER;
        }
        public bool IsGate()
        {
            return tileType == TileType.ENTRANCE || tileType == TileType.EXIT;
        }

        private bool IsBetween(int num, int min, in int max)
        {
            return num >= min && num < max;
        }
        private void SetHeightType(int h)
        {
            if (IsBetween(h, int.MinValue, -100))
            {
                SetType(TileType.DEEP_WATER);
            }
            else if (IsBetween(h, -100, 0))
            {
                SetType(TileType.SHALLOW_WATER);
            }
            else if (IsBetween(h, 0, 100))
            {
                SetType(TileType.GROUND);
            }
            else if (IsBetween(h, 100, 200))
            {
                SetType(TileType.GROUND_SMALL);
            }
            else if (IsBetween(h, 200, 300))
            {
                SetType(TileType.SMALL_HILL);
            }
            else if (IsBetween(h, 300, 350)) //300-350
            {
                SetType(TileType.SMALL_MEDIUM);
            }
            else if (IsBetween(h, 350, 400))  //350-400
            {
                SetType(TileType.MEDIUM_HILL);
            }
            else if (IsBetween(h, 400, 450))   ///400-450
            {
                SetType(TileType.MEDIUM_HIGH);
            }
            else
            {
                SetType(TileType.HIGH_HILL);
            }
        }
        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }

        public Point TileCenterPoint()
        {
            return new Point((i * TILESIZE) + (TILESIZE / 2), (j * TILESIZE) + (TILESIZE / 2));
        }

        public Point TileCenterPointForEntity(Entity entity)
        {
            return new Point((i * TILESIZE) + (TILESIZE / 2) - (entity.EntitySize / 2), (j * TILESIZE) + (TILESIZE / 2) - (entity.EntitySize / 2));
        }
        public bool InTileCenterArea(Point point)
        {
            return point.X < i * TILESIZE - 10 && point.X > (i - 1) * TILESIZE + 10 &&
                   point.Y < j * TILESIZE - 10 && point.Y > (j - 1) * TILESIZE + 10;
        }
    }
}
