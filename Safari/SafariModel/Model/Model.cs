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
using System.Xml.XPath;
using System.Diagnostics.Metrics;

namespace SafariModel.Model
{
    public class Model
    {
        public string ParkName { get; private set; }

        const int TICK_PER_TIME_UNIT = 25200; //teszt -> 168;
        const int HOURS_PER_DAY = 24;
        const int DAYS_PER_WEEK = 7;
        const int WEEKS_PER_MONTH = 4;

        public static readonly int MAPSIZE = 100;
        private TileMap tileMap;
        private GameData? data;

        private IDataAccess? dataAccess;

        private EntityHandler entityHandler;
        private EconomyHandler economyHandler;
        private RoadNetworkHandler roadNetworkHandler;
        private TouristHandler touristHandler;
        private WorldGenerationHandler worldGenerationHandler;

        private int tickCount;
        private int tickPerGameSpeedCount;
        private int secondCounterHunter;

        private GameSpeed gameSpeed;
        private int speedBoost;

        // entityk helyének térképen való eloszlására
        private Dictionary<(int, int), List<Entity>> spatialMap = new();
        #region Properites
        public GameSpeed GameSpeed
        {
            get { return gameSpeed; }
            set
            {
                gameSpeed = value;
                speedBoost = gameSpeed switch
                {
                    GameSpeed.Slow => 1,
                    GameSpeed.Medium => 3,
                    GameSpeed.Fast => 9,
                    _ => 1
                };

                foreach (Entity entity in entityHandler.GetEntities())
                {
                    if (entity is MovingEntity me)
                    {
                        me.UpdateSpeedMultiplier(speedBoost);
                        if(me is Gunman g)
                        {
                            g.Multiplier = speedBoost;
                        }
                    }
                }
                secondCounterHunter = 0;
            }
        }
        public TileMap TileMap { get { return tileMap; } }

        public EconomyHandler EconomyHandler { get { return economyHandler; } }
        public EntityHandler EntityHandler { get { return entityHandler; } }  
        public RoadNetworkHandler RoadNetworkHandler { get { return roadNetworkHandler; } }

        public TouristHandler TouristHandler {  get { return touristHandler; } }
        #endregion

        #region Events
        public event EventHandler? NewGameStarted;
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        public event EventHandler<(int,int)>? TileMapUpdated;
        public event EventHandler? NewMessage;
        #endregion

        public Model(IDataAccess? dataAccess)
        {

            this.dataAccess = dataAccess;



            ParkName = "";

            tickCount = 0;
            tickPerGameSpeedCount = 0;
            gameSpeed = GameSpeed.Slow;
            speedBoost = 1;
            data = new GameData();

            entityHandler = new EntityHandler();
            Entity.RegisterHandler(entityHandler);


            worldGenerationHandler = new WorldGenerationHandler("testseed", entityHandler);
            tileMap = worldGenerationHandler.GenerateRandomMapFromSeed();
            Entity.RegisterTileMap(tileMap.Map);

            TileCollision tc = new TileCollision(tileMap);
            MovingEntity.RegisterTileCollision(tc);

            roadNetworkHandler = new RoadNetworkHandler(tileMap);
            Jeep.RegisterNetworkHandler(roadNetworkHandler);

            economyHandler = new EconomyHandler(99999);
            touristHandler = new TouristHandler(economyHandler);
            Jeep.RegisterTouristHandler(touristHandler);


            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Gazelle(100, 200, 18000, 300, 45, 45, 0, 0, 5000));
            Hunter h = new Hunter(50, 50, null);
            h.Multiplier = 1;
            h.KilledAnimal += new EventHandler<KillAnimalEventArgs>(HandleAnimalKill);
            h.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(HandleGunmanRemoval);
            entityHandler.LoadEntity(h);
        }
        public Model() : this(null) { }

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
                Hunter? hunter = entityHandler.GetNextHunter(speedBoost);
                if (hunter != null)
                {
                    hunter.KilledAnimal += new EventHandler<KillAnimalEventArgs>(HandleAnimalKill);
                    hunter.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(HandleGunmanRemoval);
                    if (secondCounterHunter == hunter.EnterField)
                    {
                        hunter.HasEntered = true;
                        hunter.TookDamage += OnNewMessage;
                        entityHandler.SpawnHunter(speedBoost);
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
                        h.IsVisible = true;
                    }
                }
            }
            touristHandler.NewTouristAtGatePerTick();



            InvokeTickPassed();
        }
        #endregion

        public void NewGame(string parkName)
        {

            ParkName = "";

          




            tickCount = 0;
            tickPerGameSpeedCount = 0;
            gameSpeed = GameSpeed.Slow;
            speedBoost = 1;
            data = new GameData();

            entityHandler = new EntityHandler();
            Entity.RegisterHandler(entityHandler);


            worldGenerationHandler = new WorldGenerationHandler("testseed", entityHandler);
            tileMap = worldGenerationHandler.GenerateRandomMapFromSeed();
            Entity.RegisterTileMap(tileMap.Map);

            TileCollision tc = new TileCollision(tileMap);
            MovingEntity.RegisterTileCollision(tc);

            roadNetworkHandler = new RoadNetworkHandler(tileMap);
            Jeep.RegisterNetworkHandler(roadNetworkHandler);

            economyHandler = new EconomyHandler(99999);
            touristHandler = new TouristHandler(economyHandler);
            Jeep.RegisterTouristHandler(touristHandler);


            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Gazelle(100, 200, 18000, 300, 45, 45, 0, 0, 5000));
            Hunter h = new Hunter(50, 50, null);
            h.Multiplier = 1;
            h.KilledAnimal += new EventHandler<KillAnimalEventArgs>(HandleAnimalKill);
            h.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(HandleGunmanRemoval);
            entityHandler.LoadEntity(h);
            OnNewGameStarted();
        }

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
            UpdateGameData();
            TickPassed?.Invoke(this, data);
        }

        private void UpdateGameData()
        {
            //Itt lehet esetleg klónozni jobb lenne az adatokat?
            data!.parkName = ParkName;
            data!.tileMap = tileMap.Map;
            data!.entrance = tileMap.Entrance;
            data!.exit = tileMap.Exit;
            data.entities = entityHandler.GetEntities();
            data.money = economyHandler.Money;
            data.gameTime = tickCount;
            data.intersections = PathIntersectionNode.allNodes;
            data.touristAtGate = touristHandler.TouristsAtGate;
            data.touristsVisited = touristHandler.TouristsVisited;
            data.entryFee = touristHandler.EntryFee;
            data.avgRating = touristHandler.AvgRating;
            CountTimePassed(data);
        }
        private void CountTimePassed(GameData data)
        {
            int divider = 1;
            switch (gameSpeed)
            {
                case GameSpeed.Slow:
                    divider = 1;
                    break;
                case GameSpeed.Medium:
                    divider = HOURS_PER_DAY;
                    break;
                case GameSpeed.Fast:
                    divider = HOURS_PER_DAY * DAYS_PER_WEEK;
                    break;
            }
            if (tickPerGameSpeedCount >= TICK_PER_TIME_UNIT / divider)
            {
                tickPerGameSpeedCount = 0;
                data.hour++;
                if (data.hour >= HOURS_PER_DAY)
                {
                    data.hour = 0;
                    data.day++;
                    if (data.day > DAYS_PER_WEEK)
                    {
                        data.day = 1;
                        data.week++;
                        if (data.week > WEEKS_PER_MONTH)
                        {
                            data.week = 1;
                            data.month++;
                            //guard salary
                            foreach(Guard g in entityHandler.GetGuards())
                            {
                                if (!economyHandler.PaySalary(g))
                                {
                                    g.LeavePark();
                                }
                            }
                            if (data.month >= 12)
                            {
                                InvokeGameOver();
                            }
                        }
                    }
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
            Tile clickedTile = tileMap.Map[tileX, tileY];

            if (Tile.tileShopMap.ContainsKey(itemName) && Tile.tileShopMap[itemName] is TileType tileToBuy)
            {

                bool canPlace = (clickedTile.TileType == TileType.GROUND && tileToBuy == TileType.SHALLOW_WATER);
                if (economyHandler.BuyTile(tileToBuy))
                {
                    clickedTile.SetType(tileToBuy);
                    OnTileMapUpdated(tileX,tileY);
                }
                return;
            }
            if (PathTile.pathTileShopMap.ContainsKey(itemName) && PathTile.pathTileShopMap[itemName] is PathTileType pathToBuy)
            {

                bool canPlace = ((clickedTile.IsWater() && pathToBuy == PathTileType.BRIDGE) || (!clickedTile.IsWater() && pathToBuy == PathTileType.ROAD));
                
                    //ha be lehet kötni a hálózatba és meg lehet venni
                
                if (canPlace && roadNetworkHandler.ConnectToNetwork(clickedTile,pathToBuy) && economyHandler.BuyPathTile(pathToBuy))
                {
                    OnTileMapUpdated(tileX, tileY);
                }
                return;
            }
            Entity? entity = EntityFactory.CreateEntity(itemName, x, y);
            Type? type = entity?.GetType();




            if (entity == null) return;
           
            if(entity is MovingEntity me)
            {
                me.UpdateSpeedMultiplier(speedBoost);
            }

            if (entity is Guard guardEntity)
            {
                guardEntity.Multiplier = speedBoost;
                guardEntity.KilledAnimal += new EventHandler<KillAnimalEventArgs>(HandleAnimalKill);
                guardEntity.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(HandleGunmanRemoval);
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
        private void HandleAnimalKill(object? sender, KillAnimalEventArgs e)
        {
            if (e.Killer is Guard g)
            {
                economyHandler.GetBounty(e.Animal);
            }
            foreach(Hunter h in entityHandler.GetHunters())
            {
                if(h.TargetAnimal == e.Animal)
                {
                    h.TargetAnimal = null;
                }
            }
            entityHandler.RemoveEntity(e.Animal);
        }
        private void HandleGunmanRemoval(object? sender, GunmanRemoveEventArgs e)
        {
            if (e.Gunman is Hunter h && h.IsKilled)
            {
                economyHandler.GetBounty(e.Gunman);
            }
            entityHandler.RemoveEntity(e.Gunman);
        }
        private void OnNewMessage(object? sender, MessageEventArgs e)
        {
            NewMessage?.Invoke(sender, e);
        }

        public async Task SaveGameAsync(string filePath)
        {
            if (dataAccess == null) return;
            if (data == null) return;
            UpdateGameData();

            await dataAccess.SaveAsync(filePath, data);
        }

        public async Task LoadGameAsync(string filePath)
        {
            if (dataAccess == null) return;
            if (data == null) return;

            data = await dataAccess.LoadAsync(filePath);

            ParkName = data.parkName;

            //statok visszatöltése
            this.economyHandler = new EconomyHandler(data.money);
            this.touristHandler = new TouristHandler(data.touristAtGate,data.touristsVisited,data.entryFee,data.avgRating,data.currentGroupSize,economyHandler);
         //   Jeep.RegisterTouristHandler(touristHandler);
            //Intersectionök visszatöltése
            PathIntersectionNode.allNodes.Clear();
            PathIntersectionNode.allNodes.AddRange(data.intersections);

            //tileok visszatöltése
            this.tileMap = new TileMap(data.tileMap);
            tileMap.Entrance = data.entrance;
            tileMap.Exit = data.exit;
            MovingEntity.RegisterTileCollision(new TileCollision(tileMap));

            //intersectionök tileokhoz kötése
            foreach(PathIntersectionNode node in PathIntersectionNode.allNodes)
            {
                Tile t = tileMap.Map[node.PathI, node.PathJ];
                if(t is PathTile pt)
                {
                    if (tileMap.Entrance.I == node.PathI && tileMap.Entrance.J == node.PathJ)
                        tileMap.Entrance.IntersectionNode = node;
                    else if (tileMap.Exit.I == node.PathI && tileMap.Exit.J == node.PathJ)
                        tileMap.Exit.IntersectionNode = node;
                    else
                        pt.IntersectionNode = node;
                }
            }

            roadNetworkHandler = new RoadNetworkHandler(tileMap);
          
            //entityk visszatöltése

            entityHandler.ClearAll();

            foreach (Entity e in data.entities)
            {
                entityHandler.LoadEntity(e);
            }
        }
    }
}
