using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SafariModel.Model.Utils;
using SafariModel.Model.EventArgsClasses;

namespace SafariModel.Model.InstanceEntity
{
    public class Guard : Gunman
    {
        #region Private Fields
        private int salary;
        private int level;
        private int shotWeight;
        private List<Hunter> nearbyHunters;
        private int hunterRange;
        private int tickBeforeFire;
        private int baseDamage;
        private Hunter? targetHunter;
        private int timeShots;
        #endregion
        public int Salary { get { return salary; } }
        public List<Hunter> NearbyHunters { get { return nearbyHunters; } }
        public int HunterRange { get { return hunterRange; } }
        public int Level { get { return level; } }
        public int TickBeforeFire { get { return tickBeforeFire; } set { tickBeforeFire = value; } }
        public Hunter? TargetHunter { get { return targetHunter; } set { targetHunter = value; } }

        public event EventHandler<MessageEventArgs>? LevelUp;

        #region Constructor
        public Guard(int x, int y, Animal? targetAnimal) : base(x, y, 100, 0, targetAnimal)
        {
            level = 1;
            shotWeight = 0;
            salary = SetSalary();
            entitySize = 12;
            nearbyHunters = new List<Hunter>();
            hunterRange = 200;
            baseDamage = 15;
            tickBeforeFire = 0;
            Damage = 15;
            targetHunter = null;
            timeShots = 72;
        }
        #endregion
        #region Public methods
        public void CollectSalary()
        {

        }
        #endregion
        protected override void EntityLogic()
        {
            if (NearbyHunters.Count > 0)
            {
                if (tickBeforeFire % (timeShots/Multiplier) == 0)
                {
                    if (targetHunter == null)
                    {
                        targetHunter = NearbyHunters[0];
                    }
                    Fire(this, targetHunter);
                }
                tickBeforeFire++;
            }
            else if (TargetAnimal != null)
            {
                if (TargX != TargetAnimal.X || TargY != TargetAnimal.Y)
                {
                    ChaseTarget();
                }
                if (TargX == x && TargY == y)
                {
                    KillAnimal();
                    TargetAnimal = null;
                    IncreaseLevel(0);
                }
            }
        }
        #region Private methods
        private int SetSalary()
        {
            Random r = new Random();
            return r.Next(50)+50;
        }

        public void IncreaseLevel(int n)
        {
            if (n == 1)
            {
                shotWeight += 10;
            }
            if (n == 2)
            {
                shotWeight += 20;
            }
            else
            {
                shotWeight += 3;
            }
            if (shotWeight >= 10 + level * 5)
            {
                level++;
                shotWeight -= 10;
                Damage = (int)(baseDamage * Math.Pow(1.2, level));
                LevelUp?.Invoke(this, new MessageEventArgs($"Level {level}!", X, Y));
            }
        }
        #endregion
    }
}
