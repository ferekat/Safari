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
using System.ComponentModel;
using SafariModel.Model.InstanceEntity;
namespace SafariModel.Model
{
    public class Jeep : MovingEntity
    {

        public readonly static int MAX_CAPACITY = 4;
        private bool atExit;
        private bool groupOnBoard;
        private int seenHunterCount;
        private int touristCount;
     
        private int waitAtEndpointDuration;
        private int waitTimer;

        private PathIntersectionNode prev;
        private List<Animal> seenAnimals = new();

        #region Loading helpers
        int? prevID;
        List<int>? seenAnimalIDs;
        #endregion


        private Random random = new();
        private static TouristHandler touristHandler = null!;
        private static RoadNetworkHandler roadNetworkHandler = null!;
  
        public Jeep(int x, int y) : base(x, y)
        {

            range = 150;
            
            touristCount = 0;
            entitySize = 50;
            waitTimer = 0;
            atExit = false;
            waitAtEndpointDuration = random.Next(100, 130);
            groupOnBoard = false;
            seenHunterCount = 0;
        }

        private List<PathTile> NextRandomPath()
        {
            List<PathTile> randomPath = new List<PathTile>();
            List<PathIntersectionNode> validNodes = new();


        PathIntersectionNode currentNode = roadNetworkHandler.Entrance.IntersectionNode!;

            int smallestVisitedSPId = currentNode.ShortestPathId;
            PathIntersectionNode to = roadNetworkHandler.ShortestPathExitToEntrance[currentNode.ShortestPathId - 1].IntersectionNode!;
            prev = currentNode;
            while (smallestVisitedSPId > 1)
            {


                if (currentNode.ShortestPathId < smallestVisitedSPId && currentNode.ShortestPathId>0) // ha spbe visszalép úgy hogy közelebb van exithez mint az anchor 
                {
                    smallestVisitedSPId = currentNode.ShortestPathId;

                }
                if (currentNode.ShortestPathId > smallestVisitedSPId) // ha spbe visszalép úgy hogy messzebb van exithez mint anchor
                {
                   

                    PathTile dest = roadNetworkHandler.ShortestPathExitToEntrance[currentNode.ShortestPathId - 2];
                    currentNode = dest.IntersectionNode!;
                    randomPath.Add(dest);
                    if (dest.I == roadNetworkHandler.Exit.I && dest.J == roadNetworkHandler.Exit.J)
                    {
                        break;
                    }
                    continue;
                }
           //    Debug.WriteLine( $"{prev.PathI},{prev.PathJ},{currentNode.PathI},{currentNode.PathJ}");
                foreach (PathIntersectionNode neigh in currentNode.NextIntersections)//ha az id kisebb(0: mellékág !0: sp) || ha mellékágból spbe
                {
                //    Debug.WriteLine($"{neigh.ShortestPathId},{from.ShortestPathId},{from.NextIntersections.Count}");
                    if (neigh.ShortestPathId < currentNode.ShortestPathId ||
                        (currentNode.ShortestPathId == 0 && neigh.ShortestPathId >= 0)
                        //|| 
                        //(currentNode.NextIntersections.Count >= 2 && neigh.PathI != prev.PathI && neigh.PathJ != prev.PathJ) ||
                        //(prev.NextIntersections.Count == 1 && prev.ShortestPathId == 0 && neigh.PathI != prev.PathI && neigh.PathJ != prev.PathJ)
                        )
                    {
                        if (prev.PathI != neigh.PathI || prev.PathJ != neigh.PathJ)
                        {
                       //     Debug.WriteLine("add");
                        validNodes.Add(neigh);
                        }
                    }
                }
                if (validNodes.Count == 0)
                {
                    to = prev;
                }
                else
                {
                    to = validNodes[random.Next(validNodes.Count)];
                }
                if (roadNetworkHandler.TileMap.Map[to.PathI, to.PathJ] is PathTile nextTile)
                {
                    randomPath.Add(nextTile);
                    if (nextTile.I == roadNetworkHandler.Exit.I && nextTile.J == roadNetworkHandler.Exit.J)
                    {
                        break;
                    }
                }
                validNodes.Clear();
                prev = currentNode;
                currentNode = to;
         
            }
          
            return randomPath;
        }
        public static void RegisterNetworkHandler(RoadNetworkHandler rnh)
        {
            roadNetworkHandler = rnh;
        }
        public static void RegisterTouristHandler(TouristHandler th)
        {
            touristHandler = th;
        }
        protected override void EntityLogic()
        {


            if (seenAnimalIDs != null)
            {
                if (seenAnimals == null) seenAnimals = new List<Animal>();
                foreach (int id in seenAnimalIDs)
                {
                    Entity? e = GetEntityByID(id);
                    if (e != null && e is Animal a)
                        seenAnimals.Add(a);
                }
                seenAnimalIDs = null;
            }
            if (prevID != null)
            { 
                prev = PathIntersectionNode.allNodes.First(n => (n.ID == prevID));
                prevID = null;
            }
           
            //betöltés



            if (!isMoving && roadNetworkHandler.FoundShortestPath)  //nem mozog 
            {
          
                waitTimer++;
                if (waitTimer > waitAtEndpointDuration)     
                {
                    if (!atExit)        
                    {
                        TouristsBoard();
                        if (groupOnBoard)   
                        {
                            atExit = true;
                            PathToEndpoint(NextRandomPath());  //elindul a kijárat felé
                        }
                    }
                    else
                    {
                    
                        TouristsTakeOff();
                        atExit = false;
                        PathToEndpoint(roadNetworkHandler.ShortestPathExitToEntrance);  //elindul a bejárat felé
                      
                    }
                    waitTimer = 0;
                    waitAtEndpointDuration = random.Next(100, 130);
                }
            }
            else // mozog
            {
                if (groupOnBoard)
                {
                    TouristSeesEntities();
                }
            }
        }
        private void TouristsBoard()
        {
            if (!groupOnBoard)
            {
            int enteringTourists =  touristHandler.TouristsEnterPark();
                if (enteringTourists > 0)
                {
                    touristCount = enteringTourists;
                    groupOnBoard = true;
                   

                }
            }
        }
        private void TouristSeesEntities()
        {

            List<Entity> entities = GetEntitiesInRange();
                  
            foreach(Entity entity in entities)
            {
                if (entity is Animal a && !seenAnimals.Contains(a))
                {
                    
                    seenAnimals.Add(a);
                }
                if (entity is Hunter)
                {
                    seenHunterCount++;
                }
            }
        }
       
        private void TouristsTakeOff()
        {
            
            touristHandler.TouristsLeavePark(seenAnimals, seenHunterCount, touristCount);
            seenAnimals.Clear();
            seenHunterCount = 0;
            groupOnBoard = false;
            touristCount = 0;
        }
        private void PathToEndpoint(List<PathTile> tilePath)
        {
                  Queue<Point> path = new();
            foreach (Tile tile in tilePath)
            {
               // Debug.WriteLine($"{tile.I},{tile.J}");
                Point point = tile.TileCenterPointForEntity(this);
                if (path.Count == 0 && x == point.X && y == point.Y)
                {
                    continue;
                }
                path.Enqueue(point);
            }
             //   Debug.WriteLine("---");
            SetPath(path);
         
        }
      
        protected override TileType[] ImPassableTileTypes()
        {
            return new TileType[] { TileType.DEEP_WATER };
        }





      
        public override void CopyData(EntityData dataholder)
        {
            base.CopyData(dataholder);
            
            dataholder.bools.Enqueue(atExit);
            dataholder.bools.Enqueue(groupOnBoard);
            dataholder.ints.Enqueue(seenHunterCount);
            dataholder.ints.Enqueue(touristCount);
            dataholder.ints.Enqueue(waitAtEndpointDuration);
            dataholder.ints.Enqueue(waitTimer);
            dataholder.ints.Enqueue(prev == null ? null : prevID);

            foreach (Animal a in seenAnimals)
            {
                dataholder.ints.Enqueue(a.ID);
            }
            dataholder.ints.Enqueue(null);
        }

        public override void LoadData(EntityData dataholder)
        {
            base.LoadData(dataholder);

            atExit = dataholder.bools.Dequeue() ?? atExit;
            groupOnBoard = dataholder.bools.Dequeue() ?? groupOnBoard;
            seenHunterCount = dataholder.ints.Dequeue() ?? seenHunterCount; 
            touristCount = dataholder.ints.Dequeue() ?? touristCount;
            waitAtEndpointDuration = dataholder.ints.Dequeue() ?? waitAtEndpointDuration;
            waitTimer = dataholder.ints.Dequeue() ?? waitTimer;
            prevID = dataholder.ints.Dequeue() ?? prevID;


            int? readInt;
            readInt = dataholder.ints.Dequeue();
            if (readInt != null) seenAnimalIDs = new List<int>();
            while (readInt != null)
            {
                int actualInt = (int)readInt;
                seenAnimalIDs!.Add(actualInt);
                readInt = dataholder.ints.Dequeue();
            }
            

        }
    }
}
