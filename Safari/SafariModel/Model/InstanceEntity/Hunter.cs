using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Hunter : Gunman
    {
        private bool isVisible;
        private Animal caughtAnimal;

        public bool IsVisible { get { return isVisible; } }
        public Animal CaughtAnimal { get { return caughtAnimal; } }
        public Hunter(int x, int y, int health, int damage, Animal a) : base(x, y, health, damage)
        {
            caughtAnimal = a;
            isVisible = false;
        }
        public void TakeAnimal()
        {

        }
        protected override void KillAnimal()
        {
            //throw new NotImplementedException();
        }
    }
}
