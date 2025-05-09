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
        public List<Entity> entities;
        public Tile[,] tileMap;
        public PathTile entrance;
        public PathTile exit;
        public int money;
        public int touristAtGate;
        public double happiness;
        public int hour;
        public int day;
        public int week;
        public int month;
        public int gameTime;
        public int winningMonths;
        public List<PathIntersectionNode> intersections;

        public GameData()
        {
            //TODO: Finish constructor
            //throw new NotImplementedException();
            hour = 0;
            day = 1;
            week = 1;
            month = 0;
        }
    }
}
