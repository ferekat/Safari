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
        private int salary;
        private int level;
        public Guard(int x, int y, int health, int damage) : base(x, y, health, damage)
        {
            level = 1;
            salary = SetSalary();
        }
        public void CollectSalary()
        {

        }

        private int SetSalary()
        {
            Random r = new Random();
            return r.Next(500);
        }

        private void IncreaseLevel()
        {

        }
    }
}
