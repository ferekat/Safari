using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Tiles
{
    public class VisitedRoad
    {
        private bool isVisited;
        public bool IsVisited { get { return isVisited; } set { isVisited = value; } }
        public VisitedRoad(bool isVisited)
        {
            this.isVisited = isVisited;
        }
    }
}
