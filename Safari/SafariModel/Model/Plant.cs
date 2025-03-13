using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Plant : Entity
    {
        private int regrowthTime;
        private bool canBeEaten;

        public int RegrowthTime { get { return regrowthTime; } }
        public bool CanBeEaten { get { return canBeEaten; } }

        public Plant(int x, int y, int regrowthTime) : base(x, y)
        {
            this.regrowthTime = regrowthTime;
            this.canBeEaten = true;
        }
    }
}
