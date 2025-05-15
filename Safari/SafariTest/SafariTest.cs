using SafariModel.Model;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;

using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System.Diagnostics;


namespace SafariTest
{
    [TestClass]
    public sealed class SafariTest
    {
        Model model;

        [TestInitialize]
        public void Init()
        {
            //Todo mockkal helyettesíteni
            model = new Model(null);
        }
        private Tile GoDown(Tile currentTile,RoadNetworkHandler roadNetworkHandler)
        {
            Tile next = roadNetworkHandler.TileMap.Map[currentTile.I + 1, currentTile.J];
            roadNetworkHandler.ConnectToNetwork(next, PathTileType.ROAD);
            return next;
        }
        private Tile GoLeft(Tile currentTile,RoadNetworkHandler roadNetworkHandler)
        {
            Tile next = roadNetworkHandler.TileMap.Map[currentTile.I, currentTile.J+1];
            roadNetworkHandler.ConnectToNetwork(next, PathTileType.ROAD);
            return next;
        }
    
       
        [TestMethod]
       
        public void TestPathNodeCount()
        {
            Tile[,] map = model.TileMap.Map;
            PathTile entrance = model.TileMap.Entrance;
            PathTile exit = model.TileMap.Exit;
            Tile currentTile = entrance;
            RoadNetworkHandler roadNetworkHandler = model.RoadNetworkHandler;
            
            PathIntersectionNode.allNodes.Clear();
            PathIntersectionNode.allNodes.Add(entrance.IntersectionNode!);
            PathIntersectionNode.allNodes.Add(exit.IntersectionNode!);

            int ei = entrance.I;
            int ej = entrance.J;


            Assert.AreEqual(2,PathIntersectionNode.allNodes.Count);
            //első lépés

            currentTile = GoDown(currentTile, roadNetworkHandler);
            Assert.AreEqual(3, PathIntersectionNode.allNodes.Count);


            //többi lépés

            currentTile =  GoDown(currentTile,roadNetworkHandler);
           ; 
            Assert.AreEqual(3, PathIntersectionNode.allNodes.Count);

            currentTile = GoLeft(currentTile, roadNetworkHandler);
           
            Assert.AreEqual(4, PathIntersectionNode.allNodes.Count);


            currentTile = GoLeft(currentTile, roadNetworkHandler);
            
            Assert.AreEqual(4, PathIntersectionNode.allNodes.Count);

            currentTile = GoDown(currentTile, roadNetworkHandler);
            
            Assert.AreEqual(5, PathIntersectionNode.allNodes.Count);

        }

        [TestMethod]

        public void TestBuyItem()
        {
            int initEntities = model.EntityHandler.Entities.Count;
            model.BuyItem("Fox", 30, 30);
            Assert.AreEqual(model.EntityHandler.Entities.Count, initEntities);
            model.BuyItem("Lion", 40, 40);
            Assert.AreEqual(model.EntityHandler.Entities.Count, initEntities + 1);
            Assert.AreEqual(model.EconomyHandler.Money, 99999 - 200);
        }
        [TestMethod]
        public void TestEntityRange()
        {
            EntityHandler handler = new EntityHandler();
            Entity.RegisterHandler(handler);
            //Range: 1000 pixels
            Gazelle g1 = new Gazelle(100, 200);
            //These entities are in g1's range
            Gazelle g2 = new Gazelle(g1.X+g1.Range-1, g1.Y);
            Gazelle g3 = new Gazelle(g1.X, g1.Y + g1.Range-1);
            //These entities are out of g1's range
            Gazelle g4 = new Gazelle(g1.X + g1.Range, g1.Y);
            Gazelle g5 = new Gazelle(g1.X, g1.Y + g1.Range);
            handler.LoadEntity(g1);
            handler.LoadEntity(g2);
            handler.LoadEntity(g3);
            handler.LoadEntity(g4);
            handler.LoadEntity(g5);

            Assert.AreEqual(2, g1.GetEntitiesInRange().Count);
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        [TestMethod]
        public void TestSpawnHunter()
        {
            EntityHandler handler = new EntityHandler();
            Entity.RegisterHandler(handler);
            handler.SpawnHunter(3);

            Assert.AreEqual(1, handler.GetHunters().Count);

            int x = handler.GetHunters()[0].X;
            int y = handler.GetHunters()[0].Y;

            Assert.IsTrue((x == 50 || y == 50) || (x == 4937 || y == 4937));
            Assert.IsTrue(50 <= x && x <= 4937 && 50 <= y && y <= 4937);
        }

        [TestMethod]
        public void TestUpdateSpatialMap()
        {
            EntityHandler handler = new EntityHandler();
            Entity.RegisterHandler(handler);
            var spatialMap = new Dictionary<(int,int), List< Entity >> ();
            int tileSize = 10;
            var e1 = new Gazelle(5, 5);
            var e2 = new Gazelle(15, 5);
            var e3 = new Gazelle(5, 25);
            var e4 = new Gazelle(5, 5);

            handler.Entities.AddRange(new[] { e1, e2, e3, e4 });
            handler.UpdateSpatialMap(spatialMap, tileSize);

            Assert.AreEqual(3, spatialMap.Count);
            Assert.IsTrue(spatialMap.ContainsKey((0, 0)));
            Assert.IsTrue(spatialMap.ContainsKey((1, 0)));
            Assert.IsTrue(spatialMap.ContainsKey((0, 2)));

            Assert.AreEqual(2, spatialMap[(0, 0)].Count);
            Assert.AreEqual(1, spatialMap[(1, 0)].Count);
            Assert.AreEqual(1, spatialMap[(0, 2)].Count);

            CollectionAssert.Contains(spatialMap[(0, 0)], e1);
            CollectionAssert.Contains(spatialMap[(0, 0)], e4);
            CollectionAssert.Contains(spatialMap[(1, 0)], e2);
            CollectionAssert.Contains(spatialMap[(0, 2)], e3);
        }
        [TestMethod]
        public void TestGetNearbyEntities()
        {
            EntityHandler handler = new EntityHandler();
            Entity.RegisterHandler(handler);
            Dictionary<(int, int), List<Entity>> spatialMap = new();

            Gazelle g = new Gazelle(50,50);

            var nearby1 = new Lion(55, 52);
            var nearby2 = new Lion(60, 45);
            var far1 = new Giraffe(90, 90);
            var exactRange = new Gazelle(60, 50);

            var coordsG = (g.X / 50,  g.Y / 50);
            spatialMap[coordsG] = new List<Entity> { g, nearby1, nearby2, exactRange };
            spatialMap[(9,9)] = new List<Entity> {far1 };

            int range = 10;

            model.SpatialMap = spatialMap;
            var result = model.GetNearbyEntities(g, range);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.Contains(result, nearby1);
            CollectionAssert.Contains(result, nearby2);
            CollectionAssert.Contains(result, exactRange);
            CollectionAssert.DoesNotContain(result, g);
            CollectionAssert.DoesNotContain(result, far1);
        }
        [TestMethod]
        public void TestGuardIncreaseLevel()
        {
            Guard g = new Guard(100, 200, null);
            string? message = null;

            g.LevelUp += (s, e) => message = e.Message!;
            g.ShotWeight = 14;
            g.IncreaseLevel(1);

            Assert.AreEqual(2, g.Level);
            Assert.AreEqual(24 - 10, g.ShotWeight);
            Assert.AreEqual((int)(15 * Math.Pow(1.2, 2)), g.Damage);
            Assert.AreEqual("Level 2!", message);

            g = new Guard(100, 200, null);
            g.ShotWeight = 0;
            g.IncreaseLevel(1);
            Assert.AreEqual(1, g.Level);
            Assert.AreEqual(10, g.ShotWeight);

            g = new Guard(100, 200, null);
            g.ShotWeight = 10;
            g.IncreaseLevel(2);
            Assert.AreEqual(20, g.ShotWeight);

            g = new Guard(100, 200, null);
            g.IncreaseLevel(3);
            Assert.AreEqual(3, g.ShotWeight);
        }
    }
}
