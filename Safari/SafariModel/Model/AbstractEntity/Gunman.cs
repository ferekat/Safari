using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class Gunman : MovingEntity
    {
        private int health;
        private int damage;
        private MovingEntity? target;

        public int Health { get { return health; } }
        public int Damage { get { return damage; } }
        public MovingEntity Target { get { return target!; } }

        public Gunman(int x, int y, int health, int damage) : base(x, y)
        {
            this.health = health;
            this.damage = damage;
        }

        public void SetTarget()
        {

        }
        public void Fire()
        {

        }
        public void KillAnimal()
        {

        }

        protected override void EntityLogic()
        {
            throw new NotImplementedException();
        }
    }
}
