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

        //<Amire szeretném hogy változzon,Amit tipusu mezőt lehet megváltoztatni>
        public readonly static Dictionary<TileType, TileType> tileInteractionMap = new Dictionary<TileType, TileType>()
        {       
            {TileType.GRASS_GROUND,TileType.GROUND},
            {TileType.WATER,TileType.GROUND}
        };

        //<Amit le akarok helyezni,Amilyen tipusu mezőre le lehet helyezni>
        public readonly static Dictionary<TilePlaceable,TileType> placeableInteractionMap = new Dictionary<TilePlaceable, TileType>()
        {
            { TilePlaceable.IS_ROAD,TileType.GROUND},
            { TilePlaceable.IS_SMALL_BRIDGE,TileType.WATER},
            { TilePlaceable.IS_LARGE_BRIDGE,TileType.WATER}
        };
        public readonly static Dictionary<string, TileType> tileTypeMap = new Dictionary<string, TileType>()
        {
            {"Lake",TileType.WATER},
            {"Grass",TileType.GRASS_GROUND}
        };
        public readonly static Dictionary<string, TilePlaceable> placeableMap = new Dictionary<string, TilePlaceable>()
        {
            {"Road",TilePlaceable.IS_ROAD},
            {"Bridge",TilePlaceable.IS_LARGE_BRIDGE}
        };


        private TilePlaceable placeable;

        public int I { get { return i; } }
        public int J { get { return j; } }

        public int Z { get { if (z == null) return 0;else return z.Z ; } }
        public TilePlaceable Placeable { get { return placeable; } }
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

            placeable = TilePlaceable.EMPTY;
        }
        
        public bool HasPlaceable()
        {
            return placeable != TilePlaceable.EMPTY;
        }
        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }
        public void SetPlaceable(TilePlaceable placeable)
        {
            this.placeable = placeable;
        }
        
    }
}
