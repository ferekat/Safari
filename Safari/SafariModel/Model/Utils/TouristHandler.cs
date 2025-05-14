using SafariModel.Model.AbstractEntity;
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
        private readonly static int RATING_TIERS = 5;
        private readonly static int MAX_RATING = 100;
        private readonly static int MIN_ENTRY_FEE = 100;
        private readonly static int MAX_ENTRY_FEE = 2000;

        private readonly static int MAX_GROUP_SIZE = Jeep.MAX_CAPACITY;
        
        private int touristsAtEntrance;
        private int touristsVisited;
        private int entryFee; //[100;2000]
        private double avgRating; //[-RATING_TIERS,RATING_TIERS]
        private double avgTouristSpawn; //ennyi másodpercenként jön új turista
        private int tick = 0;

        private int currentGroupSize;
        private EconomyHandler economyHandler;

        public event EventHandler<int> TouristArrived;
        public int EntryFee {  get { return entryFee; } }   
        public int TouristsAtGate { get { return touristsAtEntrance; } }
        public int CurrentGroupSize { get { return currentGroupSize; } }
     



        public TouristHandler(EconomyHandler economyHandler)
        {
            this.economyHandler = economyHandler;
            touristsAtEntrance = 0;
            entryFee = 1000;
            avgRating = 0;
            touristsVisited = 0;    
            currentGroupSize = random.Next(MAX_GROUP_SIZE) + 1;
            avgTouristSpawn = CalcSpawnChance();
            
        }
        private double CalcSpawnChance()
        {
            return 2;
            return 50.0 - 6.0 * avgRating + entryFee / 100.0;
        }
        private double ChanceAtEverySec(double sec)
        {
    
            return 1 / (sec * 120.0);
        }
       
        public void NewTouristAtGatePerTick()
        {
            
            double spawn = random.NextDouble();
                if (tick % 120 == 0)
                {
                  //  Debug.WriteLine(tick/120);
                }
            if (spawn < ChanceAtEverySec(avgTouristSpawn) && touristsAtEntrance < MAX_WAITING_TOURIST)
            {
                touristsAtEntrance+= 1;
                TouristArrived?.Invoke(this, touristsAtEntrance);
                
            }
            avgTouristSpawn = CalcSpawnChance();
            tick++;
            //Debug.WriteLine(touristsAtEntrance + " TOURIST | " + tick + " TICK | " + spawn + " SPAWN");
        }
        public int TouristsEnterPark()
        {
            int group = currentGroupSize; 
            if (touristsAtEntrance >= group)
            {
                touristsAtEntrance -= group;
                TouristArrived?.Invoke(this, touristsAtEntrance);
                currentGroupSize = random.Next(MAX_GROUP_SIZE)+1;
                economyHandler.TicketSell(group*entryFee);    
                return group;
            }
            return 0;
        }
        public void TouristsLeavePark(List<Animal> seenAnimals, int seenHunterCount, int touristCount)
        {
            List<double> ratings = CalcTourRatings(seenAnimals, seenHunterCount,touristCount);
            double sum = ratings.Sum();
            

            avgRating = (avgRating * touristsVisited + sum) / (touristsVisited + touristCount);   //mozgóátlag
            touristsVisited += touristCount;
           
           
        }
        private List<double> CalcTourRatings(List<Animal> seenAnimals, int seenHunterCount, int touristCount)
        {
            double rating;
            double animalCount = seenAnimals.Count;
            double uniqueTypesCount = seenAnimals
            .Select(a => a.GetType())
            .Distinct()
            .Count();

            rating = animalCount + uniqueTypesCount * 10 - entryFee / (animalCount * 2+1) - seenHunterCount * 50; //értékelés képlete 
            rating = Math.Clamp(rating, -MAX_RATING, MAX_RATING);
            rating = rating * RATING_TIERS / MAX_RATING; //normalizálás [-5,5] re
            
            List<double> result = new List<double>();
            for (int i = 0; i < touristCount; i++)
            {
                result.Add(Math.Clamp(rating + (random.NextDouble() - 0.1),-RATING_TIERS,RATING_TIERS));  //random fluktuáció
            }
            return result;
        }
    }
}
