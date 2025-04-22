using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public abstract class Carnivore : Animal
    {
        #region Constructor
        protected Carnivore(int x, int y, int age, int health, int food, int water, int hunger, int thrist) : base(x, y, age, health, food, water, hunger, thrist)
        {
            //Egyéb húsevő specifikus dolgok
        }
        #endregion
    }
}
