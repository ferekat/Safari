using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public abstract class Gunman : MovingEntity
    {
        private int health;
        private int damage;
        private MovingEntity? target;

        public int Health { get; protected set; }
        public int Damage { get; protected set; }
        public MovingEntity Target { get { return target!; } }

        public Gunman(int x, int y, int health, int damage) : base(x, y)
        {
            Health = health;
            Damage = damage;
        }
        public void Fire()
        {

        }
        protected abstract void KillAnimal();
        protected abstract void ChaseTarget();

        protected override void EntityLogic()
        {

        }
    }
}
