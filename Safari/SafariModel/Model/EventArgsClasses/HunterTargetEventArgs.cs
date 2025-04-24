using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.EventArgsClasses
{
    public class HunterTargetEventArgs : EventArgs
    {
        private Hunter? hunter;
        public Hunter Hunter { get { return hunter!; } private set { hunter = value; } }

        public HunterTargetEventArgs(Hunter h)
        {
            Hunter = h;
        }
    }
}

