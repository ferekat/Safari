using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Jeep : MovingEntity
    {
        private static int MAX_CAPACITY = 4;
        private int touristCount;
        private double happiness;
        public Jeep(int x,int y) : base(x,y) 
        {
            happiness = 0;
            touristCount = 0;
        }

        protected override void EntityLogic()
        {
            throw new NotImplementedException();
        }

        private void PathToExit()
        {

        }
        private void ShortestPathToEntrance()
        {

        }
    }
}
