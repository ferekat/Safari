using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;

namespace SafariModel.Persistence
{
    public class DataSerializer
    {
        public static string SerializeTile(Tile t)
        {
            if (t is PathTile pt)
            {
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

        public static string SerializeEntity(Entity e)
        {
            //Felépítés:
            // <Entity típusnév>,<boolok szóközzel elválasztva>,<intek szóközzel elválasztva>,<doubleök szóközzel elválasztva>,<pontX;pontY (szóközökkel elválasztva)>

            StringBuilder sb = new StringBuilder();
            sb.Append(e.GetType().Name + ",");
            EntityData data = EntityData.GetInstance();
            e.CopyData(data);

            foreach (bool? val in data.bools)
            {
                sb.Append($"{(val == null ? "null " : val + " ")}");
            }
            sb.Append(",");
            foreach (int? val in data.ints)
            {
                sb.Append($"{(val == null ? "null " : val + " ")}");
            }
            sb.Append(",");
            foreach (double? val in data.doubles)
            {
                sb.Append($"{(val == null ? "null " : ((double)val).ToString(CultureInfo.CreateSpecificCulture("C")) + " ")}");
            }
            sb.Append(",");
            foreach (Point? val in data.points)
            {
                if (val == null) sb.Append("null ");
                else
                {
                    Point p = (Point)val;
                    sb.Append($"{p.X};{p.Y} ");
                }
            }

            return sb.ToString();
        }

        public static Entity? DeSerializeEntity(string serializedEntity)
        {
            string[] strings = serializedEntity.Split(",");
            Entity? e = EntityFactory.CreateEntity(strings[0], 0, 0);
            if (e == null) return e;
            EntityData data = EntityData.GetInstance();
            data.Reset();
            //boolok
            string[] strvalues = strings[1].Split(' ');
            foreach (string strvalue in strvalues)
            {
                if (strvalue.Equals(string.Empty)) continue;
                if (strvalue.Equals("null")) data.bools.Enqueue(null);
                else data.bools.Enqueue(Boolean.Parse(strvalue));
            }
            //intek
            strvalues = strings[2].Split(' ');
            foreach (string strvalue in strvalues)
            {
                if (strvalue.Equals(string.Empty)) continue;
                if (strvalue.Equals("null")) data.ints.Enqueue(null);
                else data.ints.Enqueue(int.Parse(strvalue));
            }
            //doubleök
            strvalues = strings[3].Split(' ');
            foreach (string strvalue in strvalues)
            {
                if (strvalue.Equals(string.Empty)) continue;
                if (strvalue.Equals("null")) data.doubles.Enqueue(null);
                else data.doubles.Enqueue(double.Parse(strvalue, CultureInfo.CreateSpecificCulture("C").NumberFormat));
            }
            //pontok
            strvalues = strings[4].Split(' ');
            foreach (string strvalue in strvalues)
            {
                if (strvalue.Equals(string.Empty)) continue;
                if (strvalue.Equals("null")) data.points.Enqueue(null);
                else
                {
                    string[] pointCoords = strvalue.Split(';');
                    data.points.Enqueue(new Point(int.Parse(pointCoords[0]), int.Parse(pointCoords[1])));
                }
            }

            e.LoadData(data);
            return e;
        }

        public static string SerializePathIntersections(List<PathIntersectionNode> nodes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (PathIntersectionNode node in nodes)
            {
                StringBuilder neighbors = new StringBuilder();
                foreach(PathIntersectionNode neighbor in node.NextIntersections)
                {
                    neighbors.Append($"{neighbor.ID} ");
                }
                builder.Append($"{node.ID},{node.PathI},{node.PathJ},{node.Distance},{node.IsVisited},{neighbors.ToString()};");
            }
            return builder.ToString();
        }

        public static List<PathIntersectionNode> DeSerializePathIntersections(string serializedNodes)
        {
            //Összes node visszaalakítása
            List<PathIntersectionNode> nodes = new List<PathIntersectionNode>();
            Dictionary<int,PathIntersectionNode> nodesByID = new Dictionary<int,PathIntersectionNode>();
            Dictionary<PathIntersectionNode, List<int>> nodeNeighborIDs = new Dictionary<PathIntersectionNode, List<int>>();
            string[] nodeStrings = serializedNodes.Split(';');

            foreach (string nodeString in nodeStrings) 
            {
                if (nodeString.Equals(string.Empty)) continue;
                string[] attributes = nodeString.Split(',');
                int id = int.Parse(attributes[0]);
                int pathI = int.Parse(attributes[1]);
                int pathJ = int.Parse(attributes[2]);
                int distance = int.Parse(attributes[3]);
                bool isVisited = Boolean.Parse(attributes[4]);
                string[] neighborStrings = attributes[5].Split(' ');
                List<int> neighborIDs = new List<int>();
                foreach(string neighborString in neighborStrings)
                {
                    if (neighborString.Equals(string.Empty)) continue;
                    neighborIDs.Add(int.Parse(neighborString));
                }

                PathIntersectionNode node = new PathIntersectionNode(id, pathI, pathJ, distance, isVisited);
                nodes.Add(node);
                nodesByID.Add(id, node);
                nodeNeighborIDs.Add(node, neighborIDs);
            }

            //node-ok összekötése
            foreach(PathIntersectionNode node in nodes)
            {
                foreach(int id in nodeNeighborIDs[node])
                {
                    node.ConnectIntersection(nodesByID[id]);
                }
            }

            return nodes;
        }
    }
}
