using SafariModel.Model.AbstractEntity;
using SafariModel.Model.EventArgsClasses;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private int mapSizeConvert;
        private bool duel;

        public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
        public Animal CaughtAnimal { get { return caughtAnimal!; } }
        public int EnterField { get { return enterField / Multiplier; } set { enterField = value; } }
        public bool HasEntered { get { return hasEntered; } set { hasEntered = value; } }
        public int WaitingTime { get { return waitingTime / Multiplier; } set { waitingTime = value; } }
        public bool Duel { get { return duel; } set { duel = value; } }

        public event EventHandler<HunterTargetEventArgs>? HunterTarget;
        public Hunter(int x, int y, Animal? targetAnimal) : base(x, y, 100, 0, targetAnimal)

        {
            entitySize = 40;
            random = new Random();
            enterField = TimeNextHunter();
            hasEntered = false;
            waitingTime = 0;
            tickBeforeTarget = 0;
            leavingMap = false;
            //mapSizeConvert = (Model.MAPSIZE + 1) * 49 - 12;
            duel = false;
            isVisible = false;
        }
        private void TakeAnimal()
        {
            //itt majd megy vele az állat is, ha az Animal-ben a leader logika megvalósul
            hasEntered = false;

            FindNearestExit();
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
                        if (tickBeforeTarget == WaitingTime)
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
                else if (X == 50 || X == MapSizeConvert || Y == 50 || Y == MapSizeConvert)
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
            int x = random.Next(30, 120);
            return x;

        }
        private int SetWaitingTime()
        {
            int x = random.Next(1200, 7200);
            return x;
        }

        protected override TileType[] ImPassableTileTypes()
        {
            return new TileType[] { TileType.DEEP_WATER };
        }
    }
}
