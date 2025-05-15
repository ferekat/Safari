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
        private void GoDown(Tile[,] map)
        {

        }
        private void GoLeft(Tile[,] map)
        {

        }
        
        [TestMethod]
       
        public void TestPathNodeCount()
        {
            Tile[,] map = model.TileMap.Map;
            Tile entrance = model.TileMap.Entrance;
            int ei = entrance.I;
            int ej = entrance.J;
            Assert.AreEqual(2, PathIntersectionNode.allNodes.Count);
            //első lépés
            if (ei == 0)
            {   
                GoDown(map);
            }
            else if (ej == 0)
            {
                GoLeft(map);
            }

            //többi lépés
            GoDown(map);
            GoLeft(map);
            GoLeft(map);
            GoDown(map);

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
    }
}
