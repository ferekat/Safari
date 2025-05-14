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
            model = new Model("testseed");
            
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
            //Range: 200 pixels
            Gazelle g1 = new Gazelle(100, 200);
            Gazelle g2 = new Gazelle(200, 200);
            Gazelle g3 = new Gazelle(100, 100);
            //This entity is out of g1's range
            Gazelle g4 = new Gazelle(1000, 1000);
            handler.LoadEntity(g1);
            handler.LoadEntity(g2);
            handler.LoadEntity(g3);
            handler.LoadEntity(g4);

            Assert.AreEqual(2, g1.GetEntitiesInRange().Count);
        }
    }
}
