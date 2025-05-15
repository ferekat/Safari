using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public abstract class Entity
    {
        protected int x;
        protected int y;
        protected int entitySize;
        private int id;

        public static readonly int CHUNK_SIZE = 200;

        private static EntityHandler? entityHandler;
        protected static Tile[,]? tileMap;

        private static int CurrentID = 0;
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public int EntitySize { get { return entitySize; } }

        public int ID { get { return id; } }

        public event EventHandler<((int, int), (int, int))>? ChunkCoordinateChanged;

        public static void RegisterHandler(EntityHandler handler)
        {
            entityHandler = handler;
        }

        public static void RegisterTileMap(Tile[,] tm)
        {
            tileMap = tm;
        }

        public (int, int) GetChunkCoordinates()
        {
            return (this.X / CHUNK_SIZE, this.Y / CHUNK_SIZE);
        }

        public static (int, int) GetChunkCoordinates(int x, int y)
        {
            return (x / CHUNK_SIZE, y / CHUNK_SIZE);
        }

        public double DistanceToEntity(Entity other)
        {
            return Math.Sqrt(Math.Pow(this.X - other.X, 2)+ Math.Pow(this.Y-other.Y, 2));
        }

        protected void OnChunkCoordinatesChanged((int,int) prev,(int,int) current)
        {
            ChunkCoordinateChanged?.Invoke(this, (prev, current));
        }

        public static Entity? GetEntityByID(int id)
        {
            if(entityHandler != null)
                return entityHandler.GetEntityByID(id);
            return null;
        }

        protected static List<Entity> GetEntitiesInChunk((int, int) chunk)
        {
            List<Entity>? entities = entityHandler!.GetEntitiesInChunk(chunk);
            if (entities == null) return new List<Entity>();
            return entities;
        }

        public void RemoveSelf()
        {
            if (entityHandler == null) return;
            entityHandler.RemoveEntity(this);
            RemoveEvent();
        }

        protected virtual void RemoveEvent() {}

        protected Entity(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.id = CurrentID++;
        }

        public virtual void CopyData(EntityData dataholder)
        {
            dataholder.Reset();
            dataholder.ints.Enqueue(X);
            dataholder.ints.Enqueue(Y);
            dataholder.ints.Enqueue(ID);
        }

        public virtual void LoadData(EntityData dataholder)
        {
            int? readData;
            readData = dataholder.ints.Dequeue();
            if (readData != null)
                x = (int)readData;
            readData = dataholder.ints.Dequeue();
            if (readData != null)
                y = (int)readData;
            readData = dataholder.ints.Dequeue();
            if (readData != null)
            {
                id = (int)readData;
                if (id >= CurrentID)
                    CurrentID = id + 1;
            }
        }

        public abstract void EntityTick();
    }
}
