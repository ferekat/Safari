using SafariModel.Model;
using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;

using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;


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

        [TestMethod]
        public void TestPathNodeCount()
        {
            TileMap tilemap = model.TileMap;
            model.RoadNetworkHandler.ConnectToNetwork(tilemap.Map[1, 5], PathTileType.ROAD);
            Assert.AreEqual(PathIntersectionNode.inst.Count, 3);
            model.RoadNetworkHandler.ConnectToNetwork(tilemap.Map[2, 5], PathTileType.SMALL_BRIDGE);
            Assert.AreEqual(PathIntersectionNode.inst.Count, 3);
        }

        [TestMethod]

        public void TestBuyItem()
        {
            int initEntities = model.EntityHandler.Entities.Count;
            model.BuyItem("Fox", 30, 30);
            Assert.AreEqual(model.EntityHandler.Entities.Count, initEntities);
            model.BuyItem("Lion", 40, 40);
            Assert.AreEqual(model.EntityHandler.Entities.Count, initEntities + 1);
            Assert.AreEqual(model.EconomyHandler.Money, 9999 - 200);
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
