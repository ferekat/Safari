using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.EventArgsClasses
{
    public class KillAnimalEventArgs : EventArgs
    {
        private Carnivore? carnivore;

        public Carnivore Carnivore { get { return carnivore!; } private set { carnivore = value; } }

        public KillAnimalEventArgs(Carnivore c)
        {
            Carnivore = c;
        }
    }
}
