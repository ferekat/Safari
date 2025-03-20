using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Utils
{
    public class EconomyHandler
    {
        private int money;
        public int Money { get; private set; }

        private Dictionary<Type, int> costTable = new();

        public EconomyHandler(int startingMoney)
        {
            money = startingMoney;
        }

        public int GetCost(Type item)
        {
            return costTable[item];    
        }

        public void Buy(Type item)
        {
            money -= costTable[item];
        }
        public void Sell(Type item)
        {
            money += costTable[item];
        }
    }
}
