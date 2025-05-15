using SafariModel.Model.Tiles;
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
        public Leopard(int x, int y) : base(x, y, 0, 200,80,80, 0, 0, 5000)
        {
            entitySize =70;
            range = Tile.TILESIZE * 25;
            BaseSpeed = 6.25f;
        }
        public Leopard(int x, int y, int age, int health, int food, int water, int hunger, int thirst, int breedingCooldown) : base(x, y, age, health, food, water, hunger, thirst, breedingCooldown)
        {
            entitySize = 70;
            range = Tile.TILESIZE * 25;
            BaseSpeed = 6.25f;
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
