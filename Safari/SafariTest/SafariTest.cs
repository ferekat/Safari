using SafariModel.Model;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;

using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System.Diagnostics;
using System.Drawing;
using SafariModel.Persistence;
using System.Xml.Linq;


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
        private Tile GoDown(Tile currentTile, RoadNetworkHandler roadNetworkHandler)
        {
            Tile next = roadNetworkHandler.TileMap.Map[currentTile.I + 1, currentTile.J];
            roadNetworkHandler.ConnectToNetwork(next, PathTileType.ROAD);
            return next;
        }
        private Tile GoLeft(Tile currentTile, RoadNetworkHandler roadNetworkHandler)
        {
            Tile next = roadNetworkHandler.TileMap.Map[currentTile.I, currentTile.J + 1];
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


            Assert.AreEqual(2, PathIntersectionNode.allNodes.Count);
            //első lépés

            currentTile = GoDown(currentTile, roadNetworkHandler);
            Assert.AreEqual(3, PathIntersectionNode.allNodes.Count);


            //többi lépés

            currentTile = GoDown(currentTile, roadNetworkHandler);
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
            Gazelle g2 = new Gazelle(g1.X + g1.Range - 1, g1.Y);
            Gazelle g3 = new Gazelle(g1.X, g1.Y + g1.Range - 1);
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
        public void TestEntitySavingAndLoading()
        {
            EntityData entityData = EntityData.GetInstance();

            Gazelle g1 = new Gazelle(100, 200, 4000, 176, 46, 23, 12, 16, 673);
            g1.CopyData(entityData);
            Gazelle g2 = new Gazelle(0, 0);
            g2.LoadData(entityData);

            Assert.AreEqual(g1.X, g2.X);
            Assert.AreEqual(g1.Y, g2.Y);
            Assert.AreEqual(g1.ID, g2.ID);
            Assert.AreEqual(g1.Age, g2.Age);
            Assert.AreEqual(g1.Health, g2.Health);
            Assert.AreEqual(g1.Water, g2.Water);
            Assert.AreEqual(g1.Food, g2.Food);
            Assert.AreEqual(g1.Hunger, g2.Hunger);
            Assert.AreEqual(g1.Thirst, g2.Thirst);
        }
        [TestMethod]
        public void TestMovementDirection()
        {
            Gazelle g = new Gazelle(100, 100);
            g.SetTarget(new Point(200, 200));
            g.EntityTick();
            Assert.IsTrue(g.X > 100);
            Assert.IsTrue(g.Y > 100);
        }
        [TestMethod]
        public void TestDataSerializer()
        {
            //Tile serializálása
            Tile t1 = new Tile(3, 4, 0, TileType.GROUND);
            string t1ser = DataSerializer.SerializeTile(t1);
            Tile t2 = DataSerializer.DeSerializeTile(t1ser);
            Assert.IsFalse(t2 is PathTile);
            Assert.AreEqual(t1.I, t2.I);
            Assert.AreEqual(t1.J, t2.J);
            Assert.AreEqual(t1.H, t2.H);
            Assert.AreEqual(t1.TileType, t2.TileType);

            //PathTile serializálása
            PathTile pt1 = new PathTile(t1, PathTileType.BRIDGE);
            string pt1ser = DataSerializer.SerializeTile(pt1);
            Tile pt2 = DataSerializer.DeSerializeTile(pt1ser);
            Assert.IsTrue(pt2 is PathTile);
            if (pt2 is PathTile ptconv)
                Assert.AreEqual(pt1.PathType, ptconv.PathType);

            //Entityk serializálása
            Gazelle g1 = new Gazelle(100, 200, 4000, 176, 46, 23, 12, 16, 673);
            string g1ser = DataSerializer.SerializeEntity(g1);
            Entity? e2 = DataSerializer.DeSerializeEntity(g1ser);
            Assert.IsNotNull(e2);
            Assert.IsTrue(e2 is Gazelle);
            if (e2 is Gazelle g2)
            {
                Assert.AreEqual(g1.X, g2.X);
                Assert.AreEqual(g1.Y, g2.Y);
                Assert.AreEqual(g1.ID, g2.ID);
                Assert.AreEqual(g1.Age, g2.Age);
                Assert.AreEqual(g1.Health, g2.Health);
                Assert.AreEqual(g1.Water, g2.Water);
                Assert.AreEqual(g1.Food, g2.Food);
                Assert.AreEqual(g1.Hunger, g2.Hunger);
                Assert.AreEqual(g1.Thirst, g2.Thirst);
            }

            //IntersectionNode-ok serializálása
            List<PathIntersectionNode> nodes1 = new List<PathIntersectionNode>();
            PathIntersectionNode node1 = new PathIntersectionNode(2, 3);
            PathIntersectionNode node2 = new PathIntersectionNode(6, 1);
            PathIntersectionNode node3 = new PathIntersectionNode(8, 0);
            node1.ConnectIntersection(node2);
            node2.ConnectIntersection(node3);
            nodes1.Add(node1);
            nodes1.Add(node2);
            nodes1.Add(node3);

            string nodesser = DataSerializer.SerializePathIntersections(nodes1);
            List<PathIntersectionNode> nodes2 = DataSerializer.DeSerializePathIntersections(nodesser);

            Assert.AreEqual(nodes1.Count, nodes2.Count);

            for (int i = 0; i < nodes1.Count; i++)
            {
                Assert.AreEqual(nodes1[i].ID, nodes2[i].ID);
                Assert.AreEqual(nodes1[i].PathI, nodes2[i].PathI);
                Assert.AreEqual(nodes1[i].PathJ, nodes2[i].PathJ);
                Assert.AreEqual(nodes1[i].NextIntersections.Count, nodes2[i].NextIntersections.Count);
                for (int j = 0; j < nodes1[i].NextIntersections.Count; j++)
                {
                    Assert.AreEqual(nodes1[i].NextIntersections[j].ID, nodes2[i].NextIntersections[j].ID);
                }
            }
        }

        [TestMethod]
        public void TestAnimalFoodAndWaterSearch()
        {

            EntityHandler handler = new EntityHandler();

            Tile[,] testMap = new Tile[Model.MAPSIZE, Model.MAPSIZE];
            for (int i = 0; i < Model.MAPSIZE; i++)
            {
                for (int j = 0; j < Model.MAPSIZE; j++)
                {
                    Tile t = new Tile(i, j, 0, TileType.SHALLOW_WATER);
                    testMap[i, j] = t;
                }
            }

            Entity.RegisterTileMap(testMap);
            Entity.RegisterHandler(handler);

            Gazelle g = new Gazelle(Entity.CHUNK_SIZE, Entity.CHUNK_SIZE);
            Greasewood gr = new Greasewood(Entity.CHUNK_SIZE + 1, Entity.CHUNK_SIZE + 1);

            handler.LoadEntity(g);
            handler.LoadEntity(gr);
            g.CheckArea();
            Assert.AreEqual(1, g.ExploredFoodPlaceCount);
            Assert.AreEqual(1, g.ExploredWaterPlaceCount);

            //Ugyanabból a chunkból 2 pontot nem fog felvenni:
            g.CheckArea();
            Assert.AreEqual(1, g.ExploredFoodPlaceCount);
            Assert.AreEqual(1, g.ExploredWaterPlaceCount);

            MovingEntity.RegisterTileCollision(new TileCollision(new TileMap(testMap)));

            //De másik chunkból már igen
            g.SetTarget(new Point(0, 0));
            g.EntityTick();
            g.CheckArea();
            Assert.AreEqual(2, g.ExploredFoodPlaceCount);
            Assert.AreEqual(2, g.ExploredWaterPlaceCount);
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
            var spatialMap = new Dictionary<(int, int), List<Entity>>();
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

            Gazelle g = new Gazelle(50, 50);

            var nearby1 = new Lion(55, 52);
            var nearby2 = new Lion(60, 45);
            var far1 = new Giraffe(90, 90);
            var exactRange = new Gazelle(60, 50);

            var coordsG = (g.X / 50, g.Y / 50);
            spatialMap[coordsG] = new List<Entity> { g, nearby1, nearby2, exactRange };
            spatialMap[(9, 9)] = new List<Entity> { far1 };

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
