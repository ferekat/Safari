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
        private Carnivore? targetAnimal;
        #endregion
        public int Salary { get { return salary; } }
        public Carnivore TargetAnimal { get { return targetAnimal!; } set { targetAnimal = value; } }

        public event EventHandler<KillAnimalEventArgs>? KilledAnimal;
        #region Constructor
        public Guard(int x, int y) : base(x, y, 100, 0)
        {
            level = 1;
            salary = SetSalary();
            entitySize = 12;
        }
        #endregion
        #region Public methods
        public void CollectSalary()
        {

        }
        #endregion
        protected void ChaseTarget()
        {
            if (isMoving)
            {
                int targX = targetAnimal!.X;
                int targY = targetAnimal!.Y;
                this.SetTarget(new Point(targX, targY));
                if (targX == x && targY == y)
                {
                    KillAnimal();
                    targetAnimal = null;
                    isMoving = false;
                }
            }
        }
        protected override void EntityLogic()
        {
            if (targetAnimal != null)
            {
                isMoving = true;
                ChaseTarget();
            }
        }
        protected override void KillAnimal()
        {
            KilledAnimal?.Invoke(this, new KillAnimalEventArgs(TargetAnimal));
        }
        #region Private methods
        private int SetSalary()
        {
            Random r = new Random();
            return r.Next(50)+50;
        }

        private void IncreaseLevel()
        {

        }
        #endregion
    }
}
