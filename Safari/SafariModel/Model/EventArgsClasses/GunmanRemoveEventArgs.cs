using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;

namespace SafariModel.Model.EventArgsClasses
{
    public class GunmanRemoveEventArgs : EventArgs
    {
        private Gunman? g;
        public Gunman Gunman { get { return g!; } private set { g = value; } }
        public GunmanRemoveEventArgs(Gunman g)
        {
            Gunman = g;
        }
    }
}
