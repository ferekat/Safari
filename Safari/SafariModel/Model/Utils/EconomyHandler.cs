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
            { typeof(PalmTree),100 },
            { typeof(Guard),0 }
        };
        private Dictionary<TilePlaceable, int> placeableCosts = new Dictionary<TilePlaceable, int>
        {
            {
                TilePlaceable.IS_ROAD,100
            },
            {
                TilePlaceable.IS_SMALL_BRIDGE,500
            },
            {
                TilePlaceable.IS_LARGE_BRIDGE,1500
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
        public bool PaySalary(Guard guard)
        {
            if(money < guard.Salary) return false;
            money -= guard.Salary;
            return true;
        }
        public void SellEntity(Type item)
        {
            money += entityCostTable[item];
        }
        public bool BuyTile(TileType clickedType,TileType tileType)
        {
            
            int cost = tileTypeCosts[tileType];
            if (money < cost || Tile.tileInteractionMap[tileType] != clickedType) return false;
            money -= cost;
            return true;
        }
        public bool BuyPlaceable(TileType clickedType,TilePlaceable placeable)
        {
            int cost = placeableCosts[placeable];
            if (money < cost || Tile.placeableInteractionMap[placeable] != clickedType) return false;
            money -= cost;
            return true;
        }
    }
}
