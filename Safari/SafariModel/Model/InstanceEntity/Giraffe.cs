using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Giraffe : Herbivore
    {
        #region Constructor
        public Giraffe(int x, int y) : base(x, y, 0, 350, 150, 200, 0, 0)
        {
            entitySize = 50;
        }
        #endregion

        #region Methods
        protected override void AnimalLogic()
        {
            //állat specifikus logika
        }
        #endregion
    }
}
