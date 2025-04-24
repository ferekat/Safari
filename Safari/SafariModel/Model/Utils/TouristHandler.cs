using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Utils
{
    public class TouristHandler
    {
        Random random = new Random();
        private readonly static int MAX_WAITING_TOURIST = 30;
        private readonly static int MIN_ENTRY_FEE = 300;
        private static int touristsAtEntrance;
        private int entryFee;
        private bool connectedGates;
        private double popularity;
        private static double avgHappiness;
        public static int TouristsAtGate { get { return touristsAtEntrance; } }
        public int EntryFee { get { return entryFee; } set { entryFee = value; } }
        public bool ConnectedGates { get { return RoadNetworkHandler.FoundShortestPath; } }
        public double Popularity { get { double ret = 1.0 / entryFee + avgHappiness; if (ret > 0.1) return 0.1;  else return ret; } }
        public TouristHandler()
        {
            popularity = 0.01;
            entryFee = 300;
            touristsAtEntrance = 0;
            avgHappiness = 0;
        }

        public void TouristUpdateTick()
        {
            double spawn = random.NextDouble(); //0.000-0.9999
            if (spawn < Popularity && touristsAtEntrance < MAX_WAITING_TOURIST)
            {
                touristsAtEntrance++;
            }
           // Debug.WriteLine(touristsAtEntrance + "T");
        }
        public static void TouristsEnter(int touristNum)
        {
            touristsAtEntrance -= touristNum;
        }
        public static void TouristLeave(double happiness)
        {
            avgHappiness = happiness; 
        }
    }
}
