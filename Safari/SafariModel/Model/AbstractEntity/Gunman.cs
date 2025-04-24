using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.EventArgsClasses;
using SafariModel.Model.InstanceEntity;

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
        private Random random;

        public int Health { get; protected set; }
        public int Damage { get; protected set; }
        public MovingEntity Target { get { return target!; } }
        public Animal? TargetAnimal { get { return targetAnimal!; } set { targetAnimal = value; } }
        public int TargX { get { return targX; } set { targX = value; } }
        public int TargY { get { return targY; } set { targY = value; } }

        public event EventHandler<KillAnimalEventArgs>? KilledAnimal;
        public event EventHandler<GunmanRemoveEventArgs>? GunmanRemove;
        public event EventHandler<MessageEventArgs>? TookDamage;

        public Gunman(int x, int y, int health, int damage, Animal? targetAnimal) : base(x, y)
        {
            Health = health;
            Damage = damage;
            TargetAnimal = targetAnimal;
            random = new Random();
        }
        protected void Fire(Guard g, Hunter h)
        {
            if (g == null || h == null) return;
            h.Duel = true;
            //guard levelje alapján eltalálja a huntert valamekkora eséllyel
            double guardChance = Math.Sqrt(g.Level) / 4.5;
            if (random.NextDouble() < guardChance)
            {
                h.Health -= g.Damage;
                TookDamage?.Invoke(this, new MessageEventArgs($"-{g.Damage}", h.X, h.Y));
                if (h.Health > 0)
                {
                    g.IncreaseLevel(1);
                }
                else
                {
                    RemoveGunman(h);
                    g.TargetHunter = null;
                    g.IncreaseLevel(2);
                    g.TickBeforeFire = 0;
                    return;

                }
            }

            //hunter 60% eséllyel eltalálja a guardot
            if (random.Next(10) < 6)
            {
                h.Damage = random.Next(10, 30);
                g.Health -= h.Damage;
                TookDamage?.Invoke(this, new MessageEventArgs($"-{h.Damage}", g.X, g.Y));
                if (g.Health <= 0)
                {
                    RemoveGunman(g);
                    h.Duel = false;
                }
            }
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
        protected void RemoveGunman(Gunman g)
        {
            GunmanRemove?.Invoke(this, new GunmanRemoveEventArgs(g));
        }

        protected override void EntityLogic()
        {

        }
    }
}
