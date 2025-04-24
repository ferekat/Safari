using SafariModel.Model.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public abstract class MovingEntity : Entity
    {
        protected Queue<Point> targetPoints; 
        private Point currentTarget;
        private Vector2 movementVector;
        private float subX;
        private float subY;


        
        private static TileCollision tileCollision;
       
        
        private static (int, int)[] coordSets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        protected float speed;
        protected int range;
        protected bool isMoving;

        public float Speed { get { return speed; } protected set { speed = value; CalculateMovementVector(); } }
        public float BaseSpeed { get; private set; }
        public int Range { get { return range; } }
        public bool IsMoving { get { return isMoving; } }

        public bool ReachedTarget { get { return targetPoints.Count == 0; } }

        public Point CurrentTarget { get { return currentTarget; } private set { currentTarget = value; CalculateMovementVector(); } }
        protected MovingEntity(int x, int y) : base(x, y)

        {
            targetPoints = new Queue<Point>();
            movementVector = new Vector2();
            subX = 0;
            subY = 0;
            isMoving = false;
            //speed = 5.5F;
            BaseSpeed = 5.5f;
            Speed = BaseSpeed;
        }


        public static void RegisterTileCollision(TileCollision collision)
        {
            tileCollision = collision;  
        }
       

        public void SetTarget(Point p)
        {
            //Ha a target egy nem járható tileon van, nem csinálunk semmit
            int tileX;
            int tileY;
            (tileX, tileY) = GetTileCoords(p);
            if (!tileCollision.IsPassable(tileX, tileY)) return;

            targetPoints.Clear();

            //Ha van egyenes út a két pont között, akkor elég a végpontot célnak megadni
            if (IsSpacePassableBetweenPoints(x, y, p.X, p.Y))
            {
                targetPoints.Clear();
                CurrentTarget = p;
            }
            //Egyébként kiszámítjuk az utat
            else CalculateRoute(p);
            if (targetPoints.Count > 0) CurrentTarget = targetPoints.Dequeue();

            isMoving = true;
        }

        public void CancelMovement()
        {
            targetPoints.Clear();
            isMoving = false;
        }
        public void SetPath(Queue<Point> points)
        {
            targetPoints.Clear();
            targetPoints = points;
            if (targetPoints.Count > 0)
            {
                CurrentTarget = targetPoints.Dequeue();
                isMoving = true;
            }
        }

        private bool IsSpacePassableBetweenPoints(int x0, int y0, int x1, int y1)
        {

            (x0, y0) = GetTileCoords(x0, y0);
            (x1, y1) = GetTileCoords(x1, y1);

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int x = x0;
            int y = y0;
            int n = 1 + dx + dy;
            int x_inc = (x1 > x0) ? 1 : -1;
            int y_inc = (y1 > y0) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;


            for (; n > 0; --n)
            {
                if (!tileCollision.IsPassable(x, y)) return false;

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }

            return true;
        }

        public void UpdateSpeedMultiplier(int multiplier)
        {
            Speed = BaseSpeed * multiplier;
        }
        private static (int, int) GetTileCoords(int x, int y)
        {
            return (x / Tile.TILESIZE, y / Tile.TILESIZE);
        }

        private static (int, int) GetTileCoords(Point p)
        {
            return GetTileCoords(p.X, p.Y);
        }
       
        private void CalculateMovementVector()
        {
            currentTarget.X = Math.Clamp(currentTarget.X, 0, TileMap.MAPSIZE * Tile.TILESIZE);
            currentTarget.Y = Math.Clamp(currentTarget.Y, 0, TileMap.MAPSIZE * Tile.TILESIZE);

            movementVector.X = currentTarget.X - this.X;
            movementVector.Y = currentTarget.Y - this.Y;

            if (movementVector.Length() == 0)
            {
                NextTargetPoint();
                return;
            }

            movementVector = Vector2.Multiply(movementVector, speed / movementVector.Length());
        }

        private void MoveTowardsTarget()
        {
            //store previous chunk coordinate
            (int, int) prevChunkCoords = GetChunkCoordinates();

            subX += movementVector.X;
            subY += movementVector.Y;
            int wholeX = (int)float.Floor(subX);
            int wholeY = (int)float.Floor(subY);
            subX -= float.Floor(subX);
            subY -= float.Floor(subY);
            this.x += wholeX;
            this.y += wholeY;

            //check if chunk coordinate changed
            (int, int) currentChunkCoords = GetChunkCoordinates();
            if (!prevChunkCoords.Equals(currentChunkCoords)) OnChunkCoordinatesChanged(prevChunkCoords, currentChunkCoords);


            //entity is outside map
            //if (this.X < Tile.TILESIZE || this.Y < Tile.TILESIZE)
            //{
            //    this.x += 1;
            //    this.y += 1;
            //    NextTargetPoint();
            //}
            //if (this.X > TileMap.MAPSIZE * Tile.TILESIZE - entitySize - Tile.TILESIZE || this.Y > TileMap.MAPSIZE * Tile.TILESIZE - entitySize - Tile.TILESIZE)
            //{
            //    this.x -= 1;
            //    this.y -= 1;
            //    NextTargetPoint();
            //}


            if (Math.Sqrt(Math.Pow(currentTarget.X - this.x, 2) + Math.Pow(currentTarget.Y - this.y, 2)) < movementVector.Length() * 1.5) //In range of target point
            {
                this.x = currentTarget.X;
                this.y = currentTarget.Y;

                NextTargetPoint();
            }
        }

        private void NextTargetPoint()
        {
            if (targetPoints.Count > 0)
            {
                CurrentTarget = targetPoints.Dequeue();
            }
            else //reached initial target
            {
                isMoving = false;
            }
        }

        private void CalculateRoute(Point p)
        {

            targetPoints.Clear();

            int startX;
            int startY;
            int finishX;
            int finishY;
            (startX, startY) = GetTileCoords(this.x, this.y);
            (finishX, finishY) = GetTileCoords(p);
            Stack<Point>? pathStack = PathFindingAlgorithm(startX, startY, finishX, finishY);


            //Ha nem sikerült utat találni, legyen a saját pozíciója a target
            if (pathStack == null)
            {
                CurrentTarget = new Point(this.X, this.Y);
                return;
            }
            //stack + az utolsó pont egybefűzése egy listába
            List<Point> pathPoints = [.. pathStack, p];

            //Pontok listájának egyszerűsítése
            SimplifyPoints(pathPoints);

            foreach (Point pnt in pathPoints)
            {
                targetPoints.Enqueue(pnt);
            }

        }

        private void SimplifyPoints(List<Point> points)
        {
            if (points.Count < 3) return;

            int current = 1;
            while (current + 1 < points.Count)
            {
                if (points[current - 1].X == points[current + 1].X ||
                    points[current - 1].Y == points[current + 1].Y
                    )
                {
                    points.RemoveAt(current);
                    continue;
                }
                else current++;
            }

            if (points.Count < 3) return;
            bool removedpoints = true;
            while (removedpoints)
            {
                removedpoints = false;
                current = 1;
                while (current + 1 < points.Count)
                {
                    if (IsSpacePassableBetweenPoints(points[current - 1].X, points[current - 1].Y, points[current + 1].X, points[current + 1].Y))
                    {
                        points.RemoveAt(current);
                        removedpoints = true;
                        continue;
                    }
                    else current++;
                }
            }
        }

        private Stack<Point>? PathFindingAlgorithm(int startX, int startY, int finishX, int finishY)
        {
            PriorityQueue<PathNode, int> openList = new PriorityQueue<PathNode, int>();
            HashSet<(int, int)> openListCoords = new HashSet<(int, int)>();
            HashSet<(int, int)> closedList = new HashSet<(int, int)>();


            //Elindítjuk a keresést a start node megadásával
            PathNode startingNode = new PathNode(null, startX, startY, 0, HeuristicFunction(startX, startY, finishX, finishY));
            openList.Enqueue(startingNode, startingNode.Cost);
            openListCoords.Add((startX, startY));

            //keresés
            while (openList.Count > 0)
            {
                PathNode currentNode = openList.Dequeue();
                int nodeX = currentNode.X;
                int nodeY = currentNode.Y;
                openListCoords.Remove((nodeX, nodeY));

                //ha elértük a célt, végeztünk
                if (nodeX == finishX && nodeY == finishY) return RetrievePath(currentNode);

                foreach ((int, int) coordset in coordSets)
                {
                    //szomszédos tileok checkolása
                    int targetX = nodeX + coordset.Item1;
                    int targetY = nodeY + coordset.Item2;
                    //Ha ráléphet a tilera, és azt a tilet nem dolgoztuk még fel és nincs benne a feldolgozandó tileok között...
                    if (tileCollision.IsPassable(targetX, targetY) && !closedList.Contains((targetX, targetY)) && !openListCoords.Contains((targetX, targetY)))
                    {
                        //Új node, amit eltárolunk az openlistben
                        PathNode newNode = new PathNode(
                            currentNode,
                            targetX, targetY,
                            currentNode.DistanceToStart + tileCollision.GetTileWeight(targetX, targetY),
                            HeuristicFunction(targetX, targetY, finishX, finishY)
                            );
                        openList.Enqueue(newNode, newNode.Cost);
                        openListCoords.Add((targetX, targetY));
                    }
                }

                //node feldolgozásának vége
                closedList.Add((nodeX, nodeY));

            }

            //A pont nem elérhető innen
            return null;
        }

        private Stack<Point> RetrievePath(PathNode finish)
        {
            Stack<Point> path = new Stack<Point>();
            PathNode current = finish;

            while (current.Parent != null)
            {
                current = current.Parent;
                int pointX = current.X * Tile.TILESIZE + Tile.TILESIZE / 2;
                int pointY = current.Y * Tile.TILESIZE + Tile.TILESIZE / 2;
                path.Push(new Point(pointX, pointY));
            }

            return path;
        }

        private int HeuristicFunction(int x0, int y0, int x1, int y1)
        {
            //Tileok manhattan-távolsága , a koordináták tile-koordináták
            return Math.Abs(x0 - x1) + Math.Abs(y0 - y1);
        }

        public override void EntityTick()
        {
            if (isMoving) MoveTowardsTarget();
            EntityLogic();
        }

        public List<Entity> GetEntitiesInRange()
        {
            int startX;
            int startY;
            (startX, startY) = Entity.GetChunkCoordinates(this.X - Range, this.Y - Range);
            int endX;
            int endY;
            (endX, endY) = Entity.GetChunkCoordinates(this.X + Range, this.Y + Range);

            List<Entity> entities = new List<Entity>();
            List<Entity> entitiesInChunks = new List<Entity>();

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    entitiesInChunks.AddRange(Entity.GetEntitiesInChunk((i, j)));
                }
            }
            foreach(Entity e in entitiesInChunks)
            {
                double dist = this.DistanceToEntity(e);
                if (dist < Range && dist != 0) entities.Add(e);
            }
            return entities;
        }

        public List<Tile> GetTilesInRange()
        {

            if (tileMap == null) return new List<Tile>();

            int entityX;
            int entityY;
            (entityX, entityY) = GetTileCoords(this.X, this.Y);
            int startX;
            int startY;
            (startX, startY) = GetTileCoords(this.X - Range, this.Y - Range);
            int endX;
            int endY;
            (endX, endY) = GetTileCoords(this.X + Range, this.Y + Range);

            List<Tile> tilesInRange = new List<Tile>();

            for(int i = startX; i <= endX; i++ )
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (i < 0 || j < 0 || i >= Model.MAPSIZE || j >= Model.MAPSIZE) continue;

                    int xCorrection = (i < entityX) ? 1 : 0;
                    int yCorrection = (j < entityY) ? 1 : 0;

                    if (Math.Sqrt(Math.Pow(((i+xCorrection)*Tile.TILESIZE) - this.X,2)+ Math.Pow(((j+yCorrection) * Tile.TILESIZE) - this.Y, 2)) < Range) tilesInRange.Add(tileMap![i, j]);
                }
            }

            return tilesInRange;
        }

        protected bool IsAccessibleTile(int i, int j, out (int,int) walkableNeighbor)
        {
            foreach((int, int) possibleCoords in coordSets)
            {
                int xChange = possibleCoords.Item1;
                int yChange = possibleCoords.Item2;
                if (tileCollision.IsPassable(i+xChange,j+yChange))
                {
                    walkableNeighbor = (i + xChange, j + yChange);
                    return true;
                }
            }
            walkableNeighbor = (-1, -1);
            return false;
        }

        protected abstract void EntityLogic();
    }
}
