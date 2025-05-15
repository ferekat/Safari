using SafariModel.Model.Tiles;
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
        public Giraffe(int x, int y) : base(x, y, 0, 350, 80, 80, 0, 0, 5000)
        {
            entitySize = 90;
            range = Tile.TILESIZE * 30;
            BaseSpeed = 3.25f;
        }
        public Giraffe(int x, int y, int age, int health, int food, int water, int hunger, int thirst, int breedingCooldown) : base(x, y, age, health, food, water, hunger, thirst, breedingCooldown)
        {
            entitySize = 90;
            range = Tile.TILESIZE * 30;
            BaseSpeed = 3.25f;
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
