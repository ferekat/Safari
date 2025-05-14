using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            { typeof(Jeep),300},
            { typeof(Lion),200},
            { typeof(Leopard),300},
            { typeof(Gazelle),150},
            { typeof(Giraffe),300},
            { typeof(Cactus),80 },
            { typeof(Greasewood),50 },
            { typeof(PalmTree),100 },
            { typeof(Guard),0 },
            { typeof(Hunter), 350 }
        };
        private Dictionary<PathTileType, int> pathTileCosts = new Dictionary<PathTileType, int>
        {
            {
                PathTileType.ROAD,100
            },
            {
                PathTileType.BRIDGE,200
            }
           
        };
        private Dictionary<TileType, int> tileCosts = new Dictionary<TileType, int>
        {
            {
                TileType.SHALLOW_WATER,200
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
        public void TicketSell(int cost)
        {
            money += cost;
        }
        public bool BuyTile(TileType tileType)
        {
            
            int cost = tileCosts[tileType];
            if (money < cost) return false;
            money -= cost;
            return true;
        }
        public bool BuyPathTile(PathTileType pathType)
        {
            int cost = pathTileCosts[pathType];
            if (money < cost ) return false;
            money -= cost;
            return true;
        }
        public void GetBounty(MovingEntity m)
        {
            //az entity eredeti árának 70%-át kapjuk fejpénznek
            int bounty = (int)Math.Floor(GetEnityCost(m.GetType())*0.7);
            money += bounty;
        }
    }
}
