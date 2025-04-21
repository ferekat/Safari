using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;
using SafariModel.Persistence;
using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using System.Drawing;
using System.Diagnostics;
using SafariModel.Model.EventArgsClasses;

namespace SafariModel.Model
{
    public class Model
    {
        const int HOURS_PER_DAY = 24;
        const int DAYS_PER_WEEK = 7;
        const int WEEKS_PER_MONTH = 4;

        public static readonly int MAPSIZE = 100;
        private Tile[,] tileMap;
        private GameData? data;

        private EntityHandler entityHandler;
        private EconomyHandler economyHandler;
        private int tickCount;
        private int tickPerGameSpeedCount;
        private int secondCounterHunter;

        private GameSpeed gameSpeed;

        // entityk helyének térképen való eloszlására
        private Dictionary<(int, int), List<Entity>> spatialMap = new();

        #region Properties
        public GameSpeed GameSpeed { get { return gameSpeed; } set { gameSpeed = value; } }
        #endregion

        #region Events
        public event EventHandler? NewGameStarted;
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        public event EventHandler<(int,int)>? TileMapUpdated;
        public event EventHandler? NewMessage;
        #endregion

        public Model()
        {
            entityHandler = new EntityHandler();
            secondCounterHunter = 0;

            tileMap = new Tile[MAPSIZE, MAPSIZE];
            for (int i = 0; i < MAPSIZE; i++)
            {
                for (int j = 0; j < MAPSIZE; j++)
                {
                    tileMap[i, j] = new Tile(i, j, null);
                }
            }

            TileCollision tc = new TileCollision(tileMap);
            MovingEntity.RegisterTileCollision(tc);

            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Lion(100, 200));


            economyHandler = new EconomyHandler(9999);

            tickCount = 0;
            tickPerGameSpeedCount = 0;
            gameSpeed = GameSpeed.Slow;

            data = new GameData();
        }

        #region Get tile and entity based on coordinates
        public (int, int) GetTileFromCoords(int x, int y)
        {
            return (x / Tile.TILESIZE, y / Tile.TILESIZE);
        }
        public int GetEntityIDFromCoords(int x, int y)
        {
            return entityHandler.GetEntityIDFromCoords(x, y);
        }
        public Entity? GetEntityByID(int id)
        {
            return entityHandler.GetEntityByID(id);
        }
        #endregion

        #region Tick update
        public void UpdatePerTick()
        {
            //Ide jön gamelogic
            tickCount++;
            tickPerGameSpeedCount++;
            if (tickCount % 120 == 0)
            {
                secondCounterHunter++;
                Hunter? hunter = entityHandler.GetNextHunter();
                if (hunter != null)
                {
                    if (secondCounterHunter == hunter.EnterField)
                    {
                        hunter.HasEntered = true;
                        hunter.TookDamage += OnNewMessage;
                        entityHandler.SpawnHunter();
                        secondCounterHunter = 0;
                    }
                }
            }
            entityHandler.TickEntities();
            entityHandler.UpdateSpatialMap(spatialMap, Tile.TILESIZE);
            foreach (Guard g in entityHandler.GetGuards())
            {
                g.NearbyHunters.Clear();
                foreach (Entity f in GetNearbyEntities(g, g.HunterRange))
                {
                    if (f is Hunter h)
                    {
                        g.NearbyHunters.Add(h);
                    }
                }
            }
            InvokeTickPassed();
        }
        #endregion

        private void OnNewGameStarted()
        {
            NewGameStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnTileMapUpdated(int tileX, int tileY)
        {
            TileMapUpdated?.Invoke(this, (tileX,tileY));
        }

        private void InvokeTickPassed()
        {
            //Itt lehet esetleg klónozni jobb lenne az adatokat?
            data!.tileMap = tileMap;
            data.entities = entityHandler.GetEntities();
            data.money = economyHandler.Money;
            data.gameTime = tickCount;
            if(tickPerGameSpeedCount >= 36000)
            {
                tickPerGameSpeedCount = 0;
                CountTimePassed(data);
            }
            TickPassed?.Invoke(this, data);
        }
        private void CountTimePassed(GameData data)
        {
            switch (gameSpeed)
            {
                case GameSpeed.Slow:
                    {
                        data.hour++;
                        if (data.hour >= HOURS_PER_DAY)
                        {
                            data.hour = 0;
                            data.day++;
                            if (data.day >= DAYS_PER_WEEK)
                            {
                                data.day = 0;
                                data.week++;
                                if (data.week >= WEEKS_PER_MONTH)
                                {
                                    data.week = 0;
                                    data.month++;
                                    if (data.month >= 12)
                                    {
                                        InvokeGameOver();
                                    }
                                }
                            }
                        }  
                        break;
                    }
                case GameSpeed.Medium:
                    {
                        data.day++;
                        if (data.day >= DAYS_PER_WEEK)
                        {
                            data.day = 0;
                            data.week++;
                            if (data.week >= WEEKS_PER_MONTH)
                            {
                                data.week = 0;
                                data.month++;
                                if (data.month >= 12)
                                {
                                    InvokeGameOver();
                                }
                            }
                        }
                        break;
                    }
                case GameSpeed.Fast:
                    {
                        data.week++;
                        if (data.week >= WEEKS_PER_MONTH)
                        {
                            data.week = 0;
                            data.month++;
                            if (data.month >= 12)
                            {
                                InvokeGameOver();
                            }
                        }
                        break;
                    }
            }
        }
        private void InvokeGameOver()
        {
            bool win = false;
            //Decide if the player won or not
            GameOver?.Invoke(this, win);
        }



        public void BuyItem(string itemName, int x, int y)
        {
            int tileX = GetTileFromCoords(x, y).Item1;
            int tileY = GetTileFromCoords(x, y).Item2;
            Tile clickedTile = tileMap[tileX, tileY];

            if (Tile.tileTypeMap.ContainsKey(itemName) && Tile.tileTypeMap[itemName] is TileType tiletype)
            {
                if (economyHandler.BuyTile(clickedTile.Type, tiletype))
                {
                    clickedTile.SetType(tiletype);
                    OnTileMapUpdated(tileX,tileY);
                }
                return;
            }
            if (Tile.placeableMap.ContainsKey(itemName) && Tile.placeableMap[itemName] is TilePlaceable placeable)
            {
                if (economyHandler.BuyPlaceable(clickedTile.Type, placeable))
                {
                    clickedTile.SetPlaceable(placeable);
                    OnTileMapUpdated(tileX, tileY);
                }
                return;
            }
            Entity? entity = EntityFactory.CreateEntity(itemName, x, y);
            Type? type = entity?.GetType();

            if (entity == null) return;

            if (entity is Guard guardEntity)
            {
                guardEntity.KilledAnimal += new EventHandler<KillAnimalEventArgs>(entityHandler.KillAnimal);
                guardEntity.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(entityHandler.RemoveGunman);
                guardEntity.TookDamage += OnNewMessage;
                guardEntity.LevelUp += OnNewMessage;
                if (!economyHandler.PaySalary(guardEntity)) return;
            }

            if (!economyHandler.BuyEntity(type!)) return;



            entityHandler.LoadEntity(entity!);

        }
        public void SellEntity(int id)
        {
            Entity? e = entityHandler.GetEntityByID(id);
            if (e == null) return;

            economyHandler.SellEntity(e.GetType());
            entityHandler.RemoveEntity(e);
        }
        public List<Entity> GetNearbyEntities(MovingEntity me, int range)
        {
            var rangeInCells = (range + Tile.TILESIZE - 1) / Tile.TILESIZE;
            var (cx, cy) = entityHandler.GetCellCoords(me, Tile.TILESIZE);
            List<Entity> nearbyEntities = new();

            for (int dx = -rangeInCells; dx <= rangeInCells; dx++)
            {
                for (int dy = -rangeInCells; dy <= rangeInCells; dy++)
                {
                    var cell = (cx + dx, cy + dy);
                    if (spatialMap.TryGetValue(cell, out var entitesInCell))
                    {
                        foreach (var e in entitesInCell)
                        {
                            if (e != me && Math.Abs(e.X - me.X) <= range && Math.Abs(e.Y - me.Y) <= range)
                            {
                                nearbyEntities.Add(e);
                            }
                        }
                    }
                }
            }
            return nearbyEntities;
        }
        private void OnNewMessage(object? sender, MessageEventArgs e)
        {
            NewMessage?.Invoke(sender, e);
        }
    }
}
