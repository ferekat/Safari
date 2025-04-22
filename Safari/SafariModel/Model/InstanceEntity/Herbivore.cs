using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public abstract class Herbivore : Animal
    {
        #region Constructor
        protected Herbivore(int x, int y, int age, int health,int food,int water, int hunger, int thrist) : base(x, y, age, health, food, water, hunger, thrist)
        {
            //Egyéb növényevő specifikus dolgok
        }
        #endregion
    }
}
