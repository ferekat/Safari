using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.Tiles;

namespace SafariModel.Persistence
{
    public class DataSerializer
    {
        public static string SerializeTile(Tile t)
        {
            if (t is PathTile pt)
            {
                //pt PathIntersectionNode-ja : new PathIntersectionNode(pt.I,pt.J);
                return string.Format("PT,{0},{1},{2},{3},{4}", pt.I, pt.J, pt.H, t.Type, pt.PathType);
            }
            else return string.Format("T,{0},{1},{2},{3}", t.I, t.J, t.H, t.Type);
        }

        public static Tile DeSerializeTile(string serializedTile)
        {
            string[] values = serializedTile.Split(',');
            if (values[0].Equals("PT"))
            {
                //pathTile

                int i = int.Parse(values[1]);
                int j = int.Parse(values[2]);
                int h = int.Parse(values[3]);
                TileType type = TileType.EMPTY;
                Enum.TryParse(values[4], out type);
                PathTileType pathType = PathTileType.EMPTY;
                Enum.TryParse(values[5], out pathType);

                //PathTile PathIntersectionNode-ja : new PathIntersectionNode(pt.I,pt.J);
                return new PathTile(new Tile(i, j, h, type), pathType, new PathIntersectionNode(i, j));
            }
            else if (values[0].Equals("T"))
            {
                //Tile
                int i = int.Parse(values[1]);
                int j = int.Parse(values[2]);
                int h = int.Parse(values[3]);
                TileType type = TileType.EMPTY;
                Enum.TryParse(values[4], out type);

                return new Tile(i, j, h, type);
            }
            //Valami baj van
            else return null;
        }
    }
}
