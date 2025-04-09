using SafariModel.Model.AbstractEntity;
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
        private int wanderTimer;

        public bool IsVisible { get { return isVisible; } }
        public Animal CaughtAnimal { get { return caughtAnimal!; } }
        public int EnterField { get { return enterField; } set { enterField = value; } }
        public bool HasEntered { get { return hasEntered; } set { hasEntered = value; } }
        public Hunter(int x, int y) : base(x, y, 100, 0)
        {
            //caughtAnimal = a;
            isVisible = false;
            entitySize = 12;
            random = new Random();
            enterField = TimeNextHunter();
            wanderTimer = random.Next(600);
            hasEntered = false;
        }
        public void TakeAnimal()
        {

        }
        protected override void KillAnimal()
        {
            //throw new NotImplementedException();
        }
        protected override void ChaseTarget()
        {
            wanderTimer = random.Next(600);
            int newX = random.Next(-300, 300);
            int newY = random.Next(-300, 300);
            this.SetTarget(new Point(this.x + newX, this.y + newY));
        }
        protected override void EntityLogic()
        {
            if (hasEntered)
            {
                wanderTimer--;
                if (wanderTimer <= 0) ChaseTarget();
            }
        }
        private int TimeNextHunter()
        {
            int x = 3;// random.Next(5, 30);
            return x;

        }
    }
}
