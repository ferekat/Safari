using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Lion : Carnivore
    {
        #region Constructor
        public Lion(int x, int y) : base(x, y, 0, 300, 200, 150,0,0)
        {
            entitySize = 40;
        }
        public Lion(int x, int y,int age, int health,  int food, int water,int hunger, int thirst) : base(x, y, age, health, food, water, hunger, thirst)
        {
            entitySize = 40;
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
