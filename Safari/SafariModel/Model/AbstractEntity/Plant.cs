using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class Plant : Entity
    {
        #region Private Fields
        private int regrowthTime;
        private bool canBeEaten;
        #endregion

        #region Properties
        public int RegrowthTime { get { return regrowthTime; } }
        public bool CanBeEaten { get { return canBeEaten; } protected set { canBeEaten = value; } }
        #endregion
        #region Constructor
        public Plant(int x, int y, int regrowthTime) : base(x, y)
        {
            CanBeEaten = true;
        }
        #endregion
        #region Methods
        public override void EntityTick()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
