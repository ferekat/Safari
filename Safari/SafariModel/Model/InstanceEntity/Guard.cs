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
        private bool isLeaving;

        #region Loading helpers
        int? targetID;
        #endregion
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
            entitySize = 50;
            nearbyHunters = new List<Hunter>();
            hunterRange = 200;
            baseDamage = 15;
            tickBeforeFire = 0;
            Damage = 15;
            targetHunter = null;
            timeShots = 72;
            isLeaving = false;
        }
        #endregion
        #region Public methods
        public void LeavePark()
        {
            isLeaving = true;
            FindNearestExit();
        }
        #endregion
        protected override void EntityLogic()
        {
            #region Betöltés után idk kiértékelése
            if (targetID != null)
            {
                Entity? e = GetEntityByID((int)targetID);
                if (e != null && e is Hunter h)
                    targetHunter = h;
                targetID = null;
            }
            #endregion

            SetTargetHunter();
            if (targetHunter != null)
            {
                if (tickBeforeFire % (timeShots / Multiplier) == 0)
                {
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
                if (!isLeaving)
                {
                    SetTargetHunter();
                    if (targetHunter != null)
                    {
                        if (tickBeforeFire % (timeShots / Multiplier) == 0)
                        {
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
                else if (X == 50 || X == MapSizeConvert || Y == 50 || Y == MapSizeConvert)
                {
                    RemoveGunman(this);
                }
            }
        }
        #region Private methods
        private int SetSalary()
        {
            Random r = new Random();
            return r.Next(50)+50;
        }
        private void SetTargetHunter()
        {
            if (NearbyHunters.Count > 0 && targetHunter == null)
            {
                foreach (Hunter h in NearbyHunters)
                {
                    if (h.HasEntered)
                    {
                        targetHunter = h;
                        break;
                    }
                }
            }
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

        public override void CopyData(EntityData dataholder)
        {
            base.CopyData(dataholder);
            dataholder.ints.Enqueue(salary);
            dataholder.ints.Enqueue(level);
            dataholder.ints.Enqueue(shotWeight);
            //A nearbyHunters futási idő alatt van kiszámolva(?) úgyhogy az nem kell
            dataholder.ints.Enqueue(hunterRange);
            dataholder.ints.Enqueue(tickBeforeFire);
            dataholder.ints.Enqueue(baseDamage);
            dataholder.ints.Enqueue(targetHunter == null ? null : targetHunter.ID);
            dataholder.ints.Enqueue(timeShots);
        }

        public override void LoadData(EntityData dataholder)
        {
            base.LoadData(dataholder);
            salary = dataholder.ints.Dequeue() ?? salary;
            level = dataholder.ints.Dequeue() ?? level;
            shotWeight = dataholder.ints.Dequeue() ?? shotWeight;
            hunterRange = dataholder.ints.Dequeue() ?? hunterRange;
            tickBeforeFire = dataholder.ints.Dequeue() ?? tickBeforeFire;
            baseDamage = dataholder.ints.Dequeue() ?? baseDamage;
            targetID = dataholder.ints.Dequeue();
            timeShots = dataholder.ints.Dequeue() ?? timeShots;
        }

        #endregion
    }
}
