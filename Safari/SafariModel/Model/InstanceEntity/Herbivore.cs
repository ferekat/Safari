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
        protected Herbivore(int x, int y, int age, int health,int food,int water, int hunger, int thrist, int breedCooldown) : base(x, y, age, health, food, water, hunger, thrist, breedCooldown)
        {
            //Egyéb növényevő specifikus dolgok
        }
        #endregion

        protected override bool IsPreferredFood(Entity e)
        {
            return e is Plant;
        }

        protected override void EatInteraction(Entity e)
        {
            Food = 100;
            if (e is Plant p)
            {
                p.GetEaten();
            }
        }
    }
}
