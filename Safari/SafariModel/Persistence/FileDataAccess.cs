using SafariModel.Model;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Persistence
{
    public class FileDataAccess : IDataAccess
    {
        //entityk mentése/betöltése nem működik!

        public async Task<GameData> LoadAsync(string filepath)
        {

            using (StreamReader reader = new StreamReader(filepath))
            {
                GameData data = new GameData();

                data.money = int.Parse(await reader.ReadLineAsync() + "");
                data.touristAtGate = int.Parse(await reader.ReadLineAsync() + "");
                data.happiness = int.Parse(await reader.ReadLineAsync() + "");
                data.hour = int.Parse(await reader.ReadLineAsync() + "");
                data.day = int.Parse(await reader.ReadLineAsync() + "");
                data.week = int.Parse(await reader.ReadLineAsync() + "");
                data.month = int.Parse(await reader.ReadLineAsync() + "");
                data.gameTime = int.Parse(await reader.ReadLineAsync() + "");
                data.winningMonths = int.Parse(await reader.ReadLineAsync() + "");

                Tile[,] tileMap = new Tile[Model.Model.MAPSIZE, Model.Model.MAPSIZE];

                //Entrance, exit pathtileok
                Tile t = DeSerializeTile(await reader.ReadLineAsync() + "");
                if (t is PathTile ent)
                    data.entrance = ent;
                t = DeSerializeTile(await reader.ReadLineAsync() + "");
                if (t is PathTile ex)
                    data.exit = ex;
                //

                for (int i = 0; i < tileMap.GetLength(0); i++)
                {
                    for (int j = 0; j < tileMap.GetLength(1); j++)
                    {
                        tileMap[i, j] = DeSerializeTile(await reader.ReadLineAsync() + "");
                    }
                }

                data.tileMap = tileMap;

                data.entities = new List<Model.AbstractEntity.Entity>();

                return data;
            }
        }

        public async Task SaveAsync(string filepath, GameData data)
        {
            if (data == null) throw new GameDataException("Game data is null");

            try
            {
                using (StreamWriter writer = new StreamWriter(filepath))
                {
                    StringBuilder builder = new StringBuilder();

                    //alap statok
                    builder.AppendLine(data.money.ToString());
                    builder.AppendLine(data.touristAtGate.ToString());
                    builder.AppendLine(data.happiness.ToString(CultureInfo.CreateSpecificCulture("C")));
                    builder.AppendLine(data.hour.ToString());
                    builder.AppendLine(data.day.ToString());
                    builder.AppendLine(data.week.ToString());
                    builder.AppendLine(data.month.ToString());
                    builder.AppendLine(data.gameTime.ToString());
                    builder.AppendLine(data.winningMonths.ToString());

                    await writer.WriteAsync(builder.ToString());

                    //tileok
                    builder.Clear();

                    //Entrance, exit pathtileok
                    builder.AppendLine(SerializeTile(data.entrance));
                    builder.AppendLine(SerializeTile(data.exit));
                    //

                    for (int i = 0; i < data.tileMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < data.tileMap.GetLength(1); j++)
                        {
                            builder.AppendLine(SerializeTile(data.tileMap[i, j]));
                        }
                    }
                    await writer.WriteAsync(builder.ToString());
                }
            }
            catch (Exception)
            {
                throw new GameDataException("Couldn't save to file");
            }
        }

        private string SerializeTile(Tile t)
        {
            if (t is PathTile pt)
            {
                //pt PathIntersectionNode-ja : new PathIntersectionNode(pt.I,pt.J);
                return string.Format("PT,{0},{1},{2},{3},{4}", pt.I, pt.J, pt.H, t.Type, pt.PathType);
            }
            else return string.Format("T,{0},{1},{2},{3}", t.I, t.J, t.H, t.Type);
        }

        private Tile DeSerializeTile(string serializedTile)
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
