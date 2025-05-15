using SafariModel.Model.Tiles;
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
        public Lion(int x, int y) : base(x, y, 0, 300, 80, 80,0,0, 5000)
        {
            entitySize = 75;
            range = Tile.TILESIZE * 20;
            BaseSpeed = 4.75f;
        }
        public Lion(int x, int y, int age, int health, int food, int water, int hunger, int thirst, int breedingCooldown) : base(x, y, age, health, food, water, hunger, thirst, breedingCooldown)
        {
            entitySize = 75;
            range = Tile.TILESIZE * 20;
            BaseSpeed = 4.75f;
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
