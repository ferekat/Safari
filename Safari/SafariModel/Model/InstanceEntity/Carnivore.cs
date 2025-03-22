using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Carnivore : Animal
    {
        #region Constructor
        protected Carnivore(int x, int y, int age, int health, int hunger, int thrist) : base(x, y, age, health, hunger, thrist)
        {
            //Egyéb húsevő specifikus dolgok
        }
        #endregion
    }
}
