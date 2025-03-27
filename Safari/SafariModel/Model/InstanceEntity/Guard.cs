using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Guard : Gunman
    {
        #region Private Fields
        private int salary;
        private int level;
        #endregion
        public int Salary { get { return salary; } }
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
