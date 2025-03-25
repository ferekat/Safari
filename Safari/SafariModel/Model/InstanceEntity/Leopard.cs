using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Leopard : Carnivore
    {
        #region Constructor
        public Leopard(int x, int y) : base(x, y, 0, 200, 250, 200)
        {

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
