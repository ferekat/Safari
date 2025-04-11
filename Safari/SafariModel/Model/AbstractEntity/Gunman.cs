using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.EventArgsClasses;

namespace SafariModel.Model.AbstractEntity
{
    public abstract class Gunman : MovingEntity
    {
        private int health;
        private int damage;
        private MovingEntity? target;
        private Animal? targetAnimal;
        private int targX;
        private int targY;


        public int Health { get; protected set; }
        public int Damage { get; protected set; }
        public MovingEntity Target { get { return target!; } }
        public Animal? TargetAnimal { get { return targetAnimal!; } set { targetAnimal = value; } }
        public int TargX { get { return targX; } set { targX = value; } }
        public int TargY { get { return targY; } set { targY = value; } }

        public event EventHandler<KillAnimalEventArgs>? KilledAnimal;

        public Gunman(int x, int y, int health, int damage, Animal? targetAnimal) : base(x, y)
        {
            Health = health;
            Damage = damage;
            TargetAnimal = targetAnimal;
        }
        public void Fire()
        {

        }
        protected void KillAnimal()
        {
            KilledAnimal?.Invoke(this, new KillAnimalEventArgs(TargetAnimal!, this));
        }
        protected void ChaseTarget()
        {
            targX = TargetAnimal!.X;
            targY = TargetAnimal!.Y;
            this.SetTarget(new Point(targX, targY));
        }


        protected override void EntityLogic()
        {

        }
    }
}
