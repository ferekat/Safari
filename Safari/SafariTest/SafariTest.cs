using SafariModel.Model;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;

using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System.Diagnostics;
using System.Drawing;


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
    }
}
