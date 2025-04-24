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
        
        private bool atExit;
        private static int MAX_CAPACITY = 4;
        private int touristCount;
        private double happiness;
        private int waitAtEndpointDuration;
        private int waitTimer;
        private Random random = new();
        public Jeep(int x,int y) : base(x,y) 
        {
         
            happiness = 0;
            touristCount = 0;
            entitySize = 20;
            waitTimer = 0;
            atExit = false;
            waitAtEndpointDuration = random.Next(100,130);
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

            if (!isMoving && RoadNetworkHandler.FoundShortestPath)
            {
             //   Debug.WriteLine("ex");
                waitTimer++;
                if (waitTimer > waitAtEndpointDuration)
                {
                    if (!atExit)
                    {
                        atExit = true;
                        PathToEndpoint(RoadNetworkHandler.ShortestPathEntranceToExit);
                    }
                    else
                    {
                        atExit = false;
                        PathToEndpoint(RoadNetworkHandler.ShortestPathExitToEntrance);
                  
                    }
                    waitTimer = 0;
                    waitAtEndpointDuration = random.Next(100, 130);
                }
            }
        }

        private void PathToEndpoint(List<PathTile> shortestPath)
        {
            Queue<Point> path = new();
            foreach(Tile tile in shortestPath)
            {

                Point point = tile.TileCenterPoint(this);
                if (path.Count == 0 && x == point.X && y == point.Y)
                {
                    continue;
                }
                path.Enqueue(point);
            }
            SetPath(path);
        }
       
    }
}
