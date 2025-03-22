using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class Entity
    {
        protected int x;
        protected int y;

        public int X { get { return x; } }
        public int Y { get { return y; } }

        protected Entity(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void EntityTick()
        {

        }
    }
}
