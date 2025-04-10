using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Utils
{
    public class RoadNetworkHandler
    {

        private static List<Tile> roads = new();
        private static List<Tile> shortestPathBetweenGates = new();

        public RoadNetworkHandler()
        {

        }

        public static void AddRoadToNetwork(Tile road)
        {
            roads.Add(road);
        }
    }
}
