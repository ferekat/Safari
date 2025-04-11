using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.Tiles;
using System.Drawing;
using SafariModel.Model.Utils;
using System.Collections;
using System.Diagnostics;
namespace SafariModel.Model
{
    public class Jeep : MovingEntity
    {
        int blocker = 0;
        private static int MAX_CAPACITY = 4;
        private int touristCount;
        private double happiness;
        private static Tile entrance;
        private static Tile exit;
        public Jeep(int x,int y) : base(x,y) 
        {
            happiness = 0;
            touristCount = 0;
            entitySize = 20;
            //Queue<Point> path = new();
            //path.Enqueue(new Point(200,200));
            //path.Enqueue(new Point(20, 30));
            //path.Enqueue(new Point(300, 500));
            //SetPath(path);

            //List<Tile> sp = RoadNetworkHandler.ShortestPathEntranceToExit;
            //sp.Reverse();
            //Queue<Tile> q = new Queue<Tile>(sp);
            //Debug.WriteLine(q.Count);
            //foreach (Tile tile in q)
            //{
            //    Debug.WriteLine($"{tile.I}, {tile.J}, {tile.Type}");
            //}
        }

        private Point TileCenterPoint(Tile tile)
        {
            return new Point((tile.I * Tile.TILESIZE) + (Tile.TILESIZE / 2)-(entitySize/2), (tile.J* Tile.TILESIZE) + (Tile.TILESIZE / 2)-(entitySize/2));
        }

        protected override void EntityLogic()
        {
          
            if (blocker == 0)
            {
                PathToExit();
            }            
            if (isMoving)
            {
                blocker = 1;
            }

            
        }

        private void PathToExit()
        {
            List<Tile> sp = RoadNetworkHandler.ShortestPathEntranceToExit;
            Queue<Tile> pathTiles = new Queue<Tile>(sp);
            Queue<Point> path = new();
            foreach(Tile tile in sp)
            {
                path.Enqueue(TileCenterPoint(tile));
            }
            SetPath(path);

        }
        private void ShortestPathToEntrance()
        {

        }
    }
}
