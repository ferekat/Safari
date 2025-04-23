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
        public Gazelle(int x, int y) : base(x, y, 0, 200, 79, 79, 0, 0)
        {
            entitySize = 30;
        }
        public Gazelle(int x, int y, int age, int health, int food, int water, int hunger, int thirst) : base(x, y, age, health, food, water, hunger, thirst)
        {
            entitySize = 30;
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
