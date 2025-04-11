using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.InstanceEntity;

namespace SafariModel.Model.EventArgsClasses
{
    public class HunterEscapeEventArgs : EventArgs
    {
        private Hunter? h;
        public Hunter Hunter { get { return h!; } private set { h = value; } }
        public HunterEscapeEventArgs(Hunter h)
        {
            Hunter = h;
        }
    }
}
