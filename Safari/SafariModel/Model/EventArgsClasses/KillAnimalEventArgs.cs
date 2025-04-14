using SafariModel.Model.InstanceEntity;
using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.EventArgsClasses
{
    public class KillAnimalEventArgs : EventArgs
    {
        private Animal? animal;
        private Gunman? killer;

        public Animal Animal { get { return animal!; } private set { animal = value; } }
        public Gunman Killer { get { return killer!; } private set { killer = value; } }


        public KillAnimalEventArgs(Animal a, Gunman g)
        {
            Animal = a;
            Killer = g;

        }
    }
}
