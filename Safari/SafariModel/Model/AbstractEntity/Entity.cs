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
        protected int entitySize;
        private int id;

        private static int CurrentID = 0;

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public int EntitySize { get { return entitySize; } }

        public int ID { get { return id; } }

        protected Entity(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.id = CurrentID++;
        }
        public void EntityTick()
        {

        }
    }
}
