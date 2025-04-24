using SafariModel.Model.AbstractEntity;
using SafariModel.Model.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        private TileType tileType;
       
        //<Amit szeretnék venni,Ahova le tudom tenni>
        //public readonly static Dictionary<TileType, TileType> tileInteractionMap = new Dictionary<TileType, TileType>()
        //{       
        //    {TileType.GRASS_GROUND,TileType.GROUND},
        //    {TileType.SHALLOW_WATER,TileType.GROUND}
        //};

        
       


        public readonly static Dictionary<string, TileType> tileShopMap = new Dictionary<string, TileType>()
        {
            {"Lake",TileType.SHALLOW_WATER},
            {"Grass",TileType.GRASS_GROUND}
        };

   

        public int I { get { return i; } }
        public int J { get { return j; } }
        public int H { get { return  h; } }
     
        public TileType Type { get { return tileType; } }
      
        public Tile(int i, int j,int h,TileType type)
        {
            this.i = i;
            this.j = j;
            this.h = h;
            this.tileType = type;
           
            
            
         
        }
        
        public bool HasPlaceable()
        {
            //hack
            return true;
           
        }
        public void SetType(TileType tileType)
        {
            this.tileType = tileType;
        }
      
       
        
        public Point TileCenterPoint(Entity entity)
        {
            return new Point((i * TILESIZE) + (TILESIZE / 2) - (entity.EntitySize / 2), (j * TILESIZE) + (TILESIZE / 2) - (entity.EntitySize / 2));
        }
    }
}
