using SafariModel.Model.AbstractEntity;
using SafariModel.Model.EventArgsClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Hunter : Gunman
    {
        private bool isVisible;
        private Animal? caughtAnimal;
        private int enterField;
        private Random random;
        private bool hasEntered;
        private int waitingTime;
        private int tickBeforeTarget;
        private bool leavingMap;
        private int mapSizeConvert;
        private bool duel;

        public bool IsVisible { get { return isVisible; } }
        public Animal CaughtAnimal { get { return caughtAnimal!; } }
        public int EnterField { get { return enterField; } set { enterField = value; } }
        public bool HasEntered { get { return hasEntered; } set { hasEntered = value; } }
        public int WaitingTime { get { return waitingTime; } set { waitingTime = value; } }
        public bool Duel { get { return duel; } set { duel = value; } }

        public event EventHandler<HunterTargetEventArgs>? HunterTarget;
        public Hunter(int x, int y, Animal? targetAnimal) : base(x, y, 100, 0, targetAnimal)

        {
            entitySize = 12;
            random = new Random();
            enterField = TimeNextHunter();
            hasEntered = false;
            waitingTime = 0;
            tickBeforeTarget = 0;
            leavingMap = false;
            mapSizeConvert = (Model.MAPSIZE + 1) * 49 - 12;
            duel = false;

        }
        private void TakeAnimal()
        {
            //itt majd megy vele az állat is, ha az Animal-ben a leader logika megvalósul
            hasEntered = false;

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
        private void DecideTask()
        {
            if (random.Next(2) == 0)
            {
                KillAnimal();
                TargetAnimal = null;
                waitingTime = SetWaitingTime();
            }
            else
            {
                TakeAnimal();
                leavingMap = true;
            }
        }
        protected override void EntityLogic()
        {
            if (!duel)
            {
                if (!leavingMap)
                {
                    if (TargetAnimal == null)
                    {
                        tickBeforeTarget++;
                        if (tickBeforeTarget == waitingTime)
                        {
                            HunterTarget?.Invoke(this, new HunterTargetEventArgs(this));
                            tickBeforeTarget = 0;
                            waitingTime = SetWaitingTime();
                        }
                    }
                    else if (hasEntered)
                    {
                        if (TargX != TargetAnimal!.X || TargY != TargetAnimal.Y)
                        {
                            ChaseTarget();
                        }
                        if (TargX == x && TargY == y)
                        {
                            DecideTask();
                        }

                    }
                }
                else if (X == 50 || X == mapSizeConvert || Y == 50 || Y == mapSizeConvert)
                {
                    RemoveGunman(this);
                }
            }
            else if (X != TargX || Y != TargY)
            {
                TargX = X;
                TargY = Y;
                SetTarget(new Point(TargX, TargY));
            }
        }
        private int TimeNextHunter()
        {
            int x = 3;// random.Next(30, 120);
            return x;

        }
        private int SetWaitingTime()
        {
            int x = 3;// random.Next(1200, 7200);
            return x;
        }
    }
}
