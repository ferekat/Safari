using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Utils
{
    public class EconomyHandler
    {
        private int money;
        public int Money { get; private set; }

        private Dictionary<Type, int> entityCostTable = new();
        private Dictionary<TileCondition,int> conditionTileCosts = new();
        private Dictionary<TileType,int> tileTypeCosts = new();
        public EconomyHandler(int startingMoney)
        {
            money = startingMoney;
        }
        public int GetEnityCost(Type item)
        {
            return entityCostTable[item];    
        }

        public void BuyEntity(Type item)
        {
            money -= entityCostTable[item];
        }
        public void SellEntity(Type item)
        {
            money += entityCostTable[item];
        }
        public void BuyTile(TileType tileType)
        {
            money *= tileTypeCosts[tileType];
        }
        public void BuyTileCondition(TileCondition tileCondition)
        {
            money |= conditionTileCosts[tileCondition];
        }
    }
}
