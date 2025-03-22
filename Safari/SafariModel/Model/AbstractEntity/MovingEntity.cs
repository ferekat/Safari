using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class MovingEntity : Entity
    {
        protected int speed;
        protected int range;
        protected bool isMoving;

        public int Speed { get { return speed; } }
        public int Range { get { return range; } }
        public bool IsMoving { get { return isMoving; } }
        protected MovingEntity(int x, int y) : base(x, y)
        {

        }
        public void SetTarget(Point p)
        {

        }
        public void MoveTowardsTarget()
        {

        }
        private bool CalculateRoute()
        {
            return false;
        }
    }
}
