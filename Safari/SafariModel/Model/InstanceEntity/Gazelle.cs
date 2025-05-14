using SafariModel.Model.AbstractEntity;
using SafariModel.Model.Tiles;
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
        public Gazelle(int x, int y) : base(x, y, 0, 200, 80, 80, 0, 0, 5000)
        {
            entitySize = 60;
            range = Tile.TILESIZE * 20;
            BaseSpeed = 5.5f;
        }
        public Gazelle(int x, int y, int age, int health, int food, int water, int hunger, int thirst, int breedingCooldown) : base(x, y, age, health, food, water, hunger, thirst, breedingCooldown)
        {
            entitySize = 60;
            range = Tile.TILESIZE * 20;
            BaseSpeed = 5.5f;
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
