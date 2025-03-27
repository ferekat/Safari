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
        public int Money { get { return money; } private set { money = value; } }

        private Dictionary<Type, int> entityCostTable = new Dictionary<Type, int>
        {
            { typeof(Lion),200},
            { typeof(Leopard),300},
            { typeof(Gazelle),150},
            { typeof(Giraffe),300},
            { typeof(Cactus),80 },
            { typeof(Greasewood),50 },
            { typeof(PalmTree),100 }
        };
        private Dictionary<TileCondition, int> conditionTileCosts = new Dictionary<TileCondition, int>
        {
            {
                TileCondition.IS_ROAD,100
            },
            {
                TileCondition.IS_SMALL_BRIDGE,500
            },
            {
                TileCondition.IS_LARGE_BRIDGE,1500
            }
        };
        private Dictionary<TileType, int> tileTypeCosts = new Dictionary<TileType, int>
        {
            {
                TileType.WATER,200
            },
            {
                TileType.GRASS_GROUND,200
            }
        };
        public EconomyHandler(int startingMoney)
        {
            money = startingMoney;
        }
        public int GetEnityCost(Type item)
        {
            return entityCostTable[item];    
        }
        public bool BuyEntity(Type item)
        {
            int cost = entityCostTable[item];
            if (money < cost) return false;
            money -= cost;
            return true;
        }
        public void SellEntity(Type item)
        {
            money += entityCostTable[item];
        }
        public void BuyTile(TileType tileType)
        {
            money -= tileTypeCosts[tileType];
        }
        public void BuyTileCondition(TileCondition tileCondition)
        {
            money -= conditionTileCosts[tileCondition];
        }
    }
}
