using SafariModel.Model.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private Queue<Point> targetPoints;
        public Point currentTarget;
        public Vector2 movementVector;
        private float subX;
        private float subY;

        //Debug only
        public List<Point> targetList;
        public List<Point> passableCheck;
        public bool passableCheckUpdated = false;

        private static TileCollision tileCollision;
        private static (int, int)[] coordSets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

        protected float speed;
        protected int range;
        protected bool isMoving;

        public float Speed { get { return speed; } protected set { speed = value; CalculateMovementVector(); } }
        public int Range { get { return range; } }
        public bool IsMoving { get { return isMoving; } }

        private Point CurrentTarget { get { return currentTarget; } set { currentTarget = value; CalculateMovementVector(); } }
        protected MovingEntity(int x, int y) : base(x, y)

        {
            targetPoints = new Queue<Point>();
            movementVector = new Vector2();
            subX = 0;
            subY = 0;
            isMoving = false;
            speed = 5.5F;
            //debug
            targetList = new List<Point>();
            passableCheck = new List<Point>();
            currentTarget = new Point(-1, -1);
        }

        public static void RegisterTileCollision(TileCollision collision)
        {
            tileCollision = collision;
        }

        public void SetTarget(Point p)
        {
            targetPoints.Clear();

            //Ha van egyenes út a két pont között, akkor elég a végpontot célnak megadni
            if (IsSpacePassableBetweenPoints(x, y, p.X, p.Y))
            {
                targetPoints.Clear();
                CurrentTarget = p;
            }
            //Egyébként kiszámítjuk az utat
            else CalculateRoute(p);
            if(targetPoints.Count > 0) CurrentTarget = targetPoints.Dequeue();

            isMoving = true;
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

            //debug
            passableCheck.Clear();


            for (; n > 0; --n)
            {
                //debug
                passableCheck.Add(new Point(x * Tile.TILESIZE, y * Tile.TILESIZE));
                passableCheckUpdated = true;

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

        private static (int, int) GetTileCoords(int x, int y)
        {
            return (x / Tile.TILESIZE, y / Tile.TILESIZE);
        }

        private void CalculateMovementVector()
        {
            currentTarget.X = Math.Clamp(currentTarget.X, 0, Model.MAPSIZE * Tile.TILESIZE);
            currentTarget.Y = Math.Clamp(currentTarget.Y, 0, Model.MAPSIZE * Tile.TILESIZE);

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
            subX += movementVector.X;
            subY += movementVector.Y;
            int wholeX = (int)float.Floor(subX);
            int wholeY = (int)float.Floor(subY);
            subX -= float.Floor(subX);
            subY -= float.Floor(subY);
            this.x += wholeX;
            this.y += wholeY;

            //entity is outside map
            if (this.X < Tile.TILESIZE || this.Y < Tile.TILESIZE)
            {
                this.x += 1;
                this.y += 1;
                NextTargetPoint();
            }
            if (this.X > Model.MAPSIZE * Tile.TILESIZE - entitySize - Tile.TILESIZE || this.Y > Model.MAPSIZE * Tile.TILESIZE - entitySize - Tile.TILESIZE)
            {
                this.x -= 1;
                this.y -= 1;
                NextTargetPoint();
            }


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
            (finishX, finishY) = GetTileCoords(p.X, p.Y);
            Stack<Point>? pathStack = PathFindingAlgorithm(startX, startY, finishX, finishY);
            

            //Ha nem sikerült utat találni, legyen a saját pozíciója a target
            if (pathStack == null)
            {
                CurrentTarget = new Point(this.X, this.Y);
                return;
            }
            List<Point> pathPoints = [.. pathStack, p];

            //Pontok listájának egyszerűsítése - még nincs kész

            //

            foreach (Point pnt in pathPoints)
            {
                targetPoints.Enqueue(pnt);
            }
            //Debug only
            targetList = pathPoints;

        }

        private Stack<Point>? PathFindingAlgorithm(int startX, int startY, int finishX, int finishY)
        {
            PriorityQueue<PathNode, int> openList = new PriorityQueue<PathNode, int>();
            HashSet<(int, int)> openListCoords = new HashSet<(int, int)>();
            HashSet<(int, int)> closedList = new HashSet<(int, int)>();
            //Feldolgozott nodeok eltárolása, hogy a gc ne lője ki őket - nem biztos hogy kell
            List<PathNode> evaluatedNodes = new List<PathNode>();

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
                evaluatedNodes.Add(currentNode);
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

        protected abstract void EntityLogic();
    }
}
