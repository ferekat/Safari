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
        private int multiplier;
        private int mapSizeConvert;

        #region Loading helpers
        int? targetID;
        int? targetAnimalID;
        #endregion

        public int Health { get; protected set; }
        public int Damage { get; protected set; }
        public MovingEntity Target { get { return target!; } }
        public Animal? TargetAnimal { get { return targetAnimal!; } set { targetAnimal = value; } }
        public int TargX { get { return targX; } set { targX = value; } }
        public int TargY { get { return targY; } set { targY = value; } }
        public int Multiplier { get { return multiplier; } set { multiplier = value; } }
        public int MapSizeConvert { get { return mapSizeConvert; } }

        public event EventHandler<KillAnimalEventArgs>? KilledAnimal;
        public event EventHandler<GunmanRemoveEventArgs>? GunmanRemove;
        public event EventHandler<MessageEventArgs>? TookDamage;

        public Gunman(int x, int y, int health, int damage, Animal? targetAnimal) : base(x, y)
        {
            Health = health;
            Damage = damage;
            TargetAnimal = targetAnimal;
            random = new Random();
            mapSizeConvert = (Model.MAPSIZE + 1) * 49 - 12;
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
                    if (h.targetAnimal != null)
                    {
                        h.targetAnimal.IsCaught = false;
                        h.targetAnimal.Abductor = null;
                    }
                    h.IsKilled = true;
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
                    h.IsVisible = false;
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

        protected void FindNearestExit()
        {
            int l = X - 50;
            int r = mapSizeConvert - X;
            int t = Y - 50;
            int b = mapSizeConvert - Y;
            int min = Math.Min(Math.Min(l, r), Math.Min(t, b));
            switch (min)
            {
                case int _ when min == l:
                    TargX = 50;
                    TargY = Y;
                    break;
                case int _ when min == r:
                    TargX = mapSizeConvert;
                    TargY = Y;
                    break;
                case int _ when min == t:
                    TargX = X;
                    TargY = 50;
                    break;
                case int _ when min == b:
                    TargX = X;
                    TargY = mapSizeConvert;
                    break;
            }
            this.SetTarget(new Point(TargX, TargY));
        }

        public override void CopyData(EntityData dataholder)
        {
            base.CopyData(dataholder);

            dataholder.ints.Enqueue(health);
            dataholder.ints.Enqueue(damage);
            //targets
            dataholder.ints.Enqueue(target == null ? null : target.ID);
            dataholder.ints.Enqueue((targetAnimal == null || !targetAnimal.IsAlive) ? null : targetAnimal.ID);

            dataholder.ints.Enqueue(targX);
            dataholder.ints.Enqueue(targY);

            dataholder.ints.Enqueue(multiplier);
        }

        public override void LoadData(EntityData dataholder)
        {
            base.LoadData(dataholder);

            health = dataholder.ints.Dequeue() ?? health;
            damage = dataholder.ints.Dequeue() ?? damage;
            //targets
            targetID = dataholder.ints.Dequeue();
            targetAnimalID = dataholder.ints.Dequeue();

            targX = dataholder.ints.Dequeue() ?? targX;
            targY = dataholder.ints.Dequeue() ?? targY;

            multiplier = dataholder.ints.Dequeue() ?? multiplier;
        }

        protected override void EntityLogic(int gameSpeedMultiplier)
        {
            #region Betöltés után idk kiértékelése
            if (targetID != null)
            {
                Entity? e = GetEntityByID((int)targetID);
                if (e != null && e is MovingEntity me)
                    target = me;
                targetID = null;
            }
            if(targetAnimalID != null)
            {
                Entity? e = GetEntityByID((int)targetAnimalID);
                if (e != null && e is Animal a)
                    targetAnimal = a;
                targetAnimalID = null;
            }
            #endregion
        }
    }
}
