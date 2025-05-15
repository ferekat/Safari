using SafariModel.Model;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Persistence
{
    public class FileDataAccess : IDataAccess
    {

        public async Task<GameData> LoadAsync(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                GameData data = new GameData();

                data.parkName = await reader.ReadLineAsync() + "";
                data.money = int.Parse(await reader.ReadLineAsync() + "");

                data.touristAtGate = int.Parse(await reader.ReadLineAsync() + "");
                data.touristsVisited = int.Parse(await reader.ReadLineAsync() + "");
                data.entryFee = int.Parse(await reader.ReadLineAsync() + "");
                double.TryParse(await reader.ReadLineAsync() + "", NumberStyles.Float, CultureInfo.InvariantCulture, out double avgRating);
                data.avgRating = avgRating;
                data.currentGroupSize = int.Parse(await reader.ReadLineAsync() + "");

                data.hour = int.Parse(await reader.ReadLineAsync() + "");
                data.day = int.Parse(await reader.ReadLineAsync() + "");
                data.week = int.Parse(await reader.ReadLineAsync() + "");
                data.month = int.Parse(await reader.ReadLineAsync() + "");
                data.gameTime = int.Parse(await reader.ReadLineAsync() + "");
                data.winningMonths = int.Parse(await reader.ReadLineAsync() + "");

                Tile[,] tileMap = new Tile[Model.Model.MAPSIZE, Model.Model.MAPSIZE];

                //Intersection node-ok
                data.intersections = DataSerializer.DeSerializePathIntersections(await reader.ReadLineAsync() + "");
                //

                //Entrance, exit pathtileok
                Tile t = DataSerializer.DeSerializeTile(await reader.ReadLineAsync() + "");
                if (t is PathTile ent)
                    data.entrance = ent;
                t = DataSerializer.DeSerializeTile(await reader.ReadLineAsync() + "");
                if (t is PathTile ex)
                    data.exit = ex;
                //

                for (int i = 0; i < tileMap.GetLength(0); i++)
                {
                    for (int j = 0; j < tileMap.GetLength(1); j++)
                    {
                        tileMap[i, j] = DataSerializer.DeSerializeTile(await reader.ReadLineAsync() + "");
                    }
                }

                data.tileMap = tileMap;

                //Entityk
                List<Entity> entities = new List<Entity>();

                while (!reader.EndOfStream)
                {
                    Entity? e = DataSerializer.DeSerializeEntity(await reader.ReadLineAsync() + "");
                    if (e != null)
                        entities.Add(e);
                }
                data.entities = entities;

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
                    builder.AppendLine(data.parkName);
                    builder.AppendLine(data.money.ToString());

                    builder.AppendLine(data.touristAtGate.ToString());
                    builder.AppendLine(data.touristsVisited.ToString());
                    builder.AppendLine(data.entryFee.ToString());
                    builder.AppendLine(data.avgRating.ToString("G",CultureInfo.CreateSpecificCulture("en-US")));
                    builder.AppendLine(data.currentGroupSize.ToString());  
                    builder.AppendLine(data.hour.ToString());
                    builder.AppendLine(data.day.ToString());
                    builder.AppendLine(data.week.ToString());
                    builder.AppendLine(data.month.ToString());
                    builder.AppendLine(data.gameTime.ToString());
                    builder.AppendLine(data.winningMonths.ToString());

                    await writer.WriteAsync(builder.ToString());

                    //Intersection node-ok
                    builder.Clear();
                    builder.AppendLine(DataSerializer.SerializePathIntersections(data.intersections));

                    await writer.WriteAsync(builder.ToString());
                    //

                    //tileok
                    builder.Clear();

                    //Entrance, exit pathtileok
                    builder.AppendLine(DataSerializer.SerializeTile(data.entrance));
                    builder.AppendLine(DataSerializer.SerializeTile(data.exit));
                    //

                    for (int i = 0; i < data.tileMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < data.tileMap.GetLength(1); j++)
                        {
                            builder.AppendLine(DataSerializer.SerializeTile(data.tileMap[i, j]));
                        }
                    }
                    await writer.WriteAsync(builder.ToString());

                    builder.Clear();
                    //Entityk
                    foreach (Entity e in data.entities)
                    {
                        builder.AppendLine(DataSerializer.SerializeEntity(e));
                    }

                    await writer.WriteAsync(builder.ToString());
                }
            }
            catch (Exception)
            {
                throw new GameDataException("Couldn't save to file");
            }
        }
    }
}
