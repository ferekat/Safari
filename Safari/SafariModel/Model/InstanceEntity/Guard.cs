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
        #endregion
        public int Salary { get { return salary; } }

        public event EventHandler<KillAnimalEventArgs>? KilledAnimal;
        #region Constructor
        public Guard(int x, int y, Animal? targetAnimal) : base(x, y, 100, 0, targetAnimal)
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
        protected override void EntityLogic()
        {
            if (TargetAnimal != null)
            {
                if (TargX != TargetAnimal.X || TargY != TargetAnimal.Y)
                {
                    ChaseTarget();
                }
                if (TargX == x && TargY == y)
                {
                    KillAnimal();
                    TargetAnimal = null;
                }
            }
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
