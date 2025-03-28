using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class Tile
    {
        public static readonly int TILESIZE = 50;
        private int i;
        private int j;
        private TileZ? z;
        private TileType tileType;

        public readonly static Dictionary<string, TileType> tileTypeMap = new Dictionary<string, TileType>()
        {
            {"Lake",TileType.WATER}
        };
        public readonly static Dictionary<string, TileCondition> tileConditionMap = new Dictionary<string, TileCondition>()
        {
            {"Road",TileCondition.IS_ROAD},
            {"Bridge",TileCondition.IS_LARGE_BRIDGE}
        };


        private TileCondition tileCondition;

        public int I { get { return i; } }
        public int J { get { return j; } }

        public int Z { get { if (z == null) return 0;else return z.Z ; } }
        public TileCondition Condition { get { return tileCondition; } }
        public TileType Type { get { return tileType; } }


        public Tile(int i, int j,TileZ? z)
        {
            this.i = i;
            this.j = j;
            tileType = TileType.GROUND;
            //Csak tesztelésre!!!
            Random r = new Random();
            tileType = r.Next(2) == 0 ? TileType.GROUND : TileType.HILL;
            if (i == 0 || i == Model.MAPSIZE - 1 || j == 0 || j == Model.MAPSIZE - 1) tileType = TileType.FENCE;
            if (tileType == TileType.HILL) z = new TileZ(r.Next(1,10));
            //

            tileCondition = TileCondition.EMPTY;
        }
        public bool HasCondition()
        {
            return tileCondition != TileCondition.EMPTY;
        }
        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }
        public void SetCondition(TileCondition tileCondition)
        {
            this.tileCondition = tileCondition;
        }
        //public void BuildRoad()
        //{
        //    tileCondition = TileCondition.IS_ROAD;
        //}
        //public void BuildLake()
        //{
        //    SetType(TileType.WATER);
        //}
    }
}
