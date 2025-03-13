using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class MovingEntity : Entity
    {
        private int speed;
        private int range;
        private bool isMoving;

        public int Speed { get { return speed; } }
        public int Range { get { return range; } } 
        public bool IsMoving { get { return isMoving; } }
        public MovingEntity(int x, int y) : base(x, y)
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
