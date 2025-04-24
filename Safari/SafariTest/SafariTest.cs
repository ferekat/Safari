using SafariModel.Model;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Utils;

namespace SafariTest
{
    [TestClass]
    public sealed class SafariTest
    {
        [TestMethod]
        public void TestEntityRange()
        {
            EntityHandler handler = new EntityHandler();
            Entity.RegisterHandler(handler);
            //Range: 200 pixels
            Gazelle g1 = new Gazelle(100,200);
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
