using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.Tiles;

namespace SafariModel.Persistence
{
    public class GameData
    {
        public string parkName;
        public List<Entity> entities;
        public Tile[,] tileMap;
        public PathTile entrance;
        public PathTile exit;
        public int money;

        public int touristAtGate;
        public int touristsVisited;
        public int entryFee;
        public double avgRating;
        public int currentGroupSize;
        
        public int hour;
        public int day;
        public int week;
        public int month;
        public int gameTime;
        public int winningMonths;
        public List<PathIntersectionNode> intersections;
        public int tourists;
        public int gazelles;
        public int giraffes;
        public int lions;
        public int leopards;
        public int jeeps;
        public int guards;

        public GameData()
        {
            //TODO: Finish constructor
            //throw new NotImplementedException();
            hour = 0;
            day = 1;
            week = 1;
            month = 0;
            gazelles = 0;
            lions = 0;
            giraffes = 0;
            leopards = 0;
            jeeps = 0;
            guards = 0;
        }
    }
}
