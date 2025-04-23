using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Gazelle : Herbivore
    {
        #region Constructor
        public Gazelle(int x, int y) : base(x, y, 0, 200, 200, 250)
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
