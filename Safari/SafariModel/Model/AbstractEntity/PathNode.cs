using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class PathNode
    {
        public PathNode? Parent { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public int DistanceToStart { get; private set; }
        public int Cost { get; private set; }
        public PathNode(PathNode? parent,int x,int y, int distanceToStart, int distancetoFinish)
        {
            this.Parent = parent;
            this.X = x;
            this.Y = y;
            DistanceToStart = distanceToStart;
            Cost = distanceToStart+distancetoFinish;
        }
    }
}
