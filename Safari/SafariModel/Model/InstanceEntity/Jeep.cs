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
using System.Net.Http.Headers;
namespace SafariModel.Model
{
    public class Jeep : MovingEntity
    {
        bool atExit = false;
        private static int MAX_CAPACITY = 4;
        private int touristCount;
        private double happiness;
        private Tile entrance;
        private Tile exit;
        private bool waitingAtEntrance;
        private bool waitingAtExit;
        private int waitingTimer;
        private int frameBlocker;
        public Jeep(int x,int y) : base(x,y) 
        {
         
            happiness = 0;
            touristCount = 0;
            entitySize = 20;
            waitingAtEntrance = true;
            waitingAtExit = false;
            waitingTimer = 50;
            //Queue<Point> path = new();
            //path.Enqueue(new Point(200, 200));
            //path.Enqueue(new Point(20, 0));
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

        

        protected override void EntityLogic()
        {

            if (!isMoving)
            {
                Debug.WriteLine("exec");
                if (!atExit)
                {
                    atExit = true;
                    SetTarget(entrance.TileCenterPoint(this));
                    //isMoving = true;
                    //PathToEntrance(true);
                }
                else
                {
                    atExit = false;
                    SetTarget(exit.TileCenterPoint(this));
                    //isMoving = true;
                //    PathToEntrance(false);
                   // ShortestPathToEntrance();
                }
            }
        }

        private void PathToEntrance(bool reverse)
        {
           
            List<Tile> sp = RoadNetworkHandler.ShortestPathExitToEntrance;
            List<Tile> spCopy = new List<Tile>();

            foreach (Tile t in sp)
            {
                spCopy.Add(t);
            }

            
            if (reverse)
            {
                spCopy.Reverse();
            }

            Queue<Point> path = new();
            foreach(Tile tile in spCopy)
            {
                path.Enqueue(tile.TileCenterPoint(this));
            }
            Debug.WriteLine("call ");
            SetPath(path);

        }
        private void ShortestPathToEntrance()
        {
            List<Tile> sp = RoadNetworkHandler.ShortestPathExitToEntrance;


            Queue<Point> path = new();
            foreach (Tile tile in sp)
            {
                path.Enqueue(tile.TileCenterPoint(this));
            }
            Debug.WriteLine("call setpath toentr");
            SetPath(path);
        }
    }
}
