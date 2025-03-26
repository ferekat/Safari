using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        protected float speed;
        protected int range;
        protected bool isMoving;

        public float Speed { get { return speed; } protected set { speed = value; CalculateMovementVector(); } }
        public int Range { get { return range; } }
        public bool IsMoving { get { return isMoving; } }

        private Point CurrentTarget { get { return currentTarget; }  set { currentTarget = value; CalculateMovementVector(); } }
        protected MovingEntity(int x, int y) : base(x, y)

        {
            targetPoints = new Queue<Point>();
            movementVector = new Vector2();
            subX = 0;
            subY = 0;
            isMoving = false;
            speed = 5.5F;
        }
        public void SetTarget(Point p)
        {
            targetPoints.Clear();
            //Calculate route with pathfinding algorithm and put resulting points into targetPoints
            CurrentTarget = p;

            isMoving = true;
        }

        private void CalculateMovementVector()
        {
            currentTarget.X = Math.Clamp(currentTarget.X,0, Model.MAPSIZE * Tile.TILESIZE);
            currentTarget.Y = Math.Clamp(currentTarget.Y, 0, Model.MAPSIZE * Tile.TILESIZE);

            movementVector.X = currentTarget.X - this.X;
            movementVector.Y = currentTarget.Y - this.Y;

            movementVector = Vector2.Multiply(movementVector, speed / movementVector.Length());
        }

        private void MoveTowardsTarget()
        {
            subX += movementVector.X;
            subY += movementVector.Y;
            int wholeX = (int)subX;
            int wholeY = (int)subY;
            subX -= float.Floor(subX);
            subY -= float.Floor(subY);
            this.x += wholeX;
            this.y += wholeY;

            if (Math.Sqrt(Math.Pow(currentTarget.X - this.x, 2) + Math.Pow(currentTarget.Y - this.y, 2)) < movementVector.Length()) //In range of target point
            {
                this.x = currentTarget.X;
                this.y = currentTarget.Y;

                if (targetPoints.Count > 0)
                {
                    CurrentTarget = targetPoints.Dequeue();
                }
                else //reached initial target
                {
                    isMoving = false;
                }
            }
        }
        private bool CalculateRoute()
        {
            return false;
        }

        public override void EntityTick()
        {
            if(isMoving) MoveTowardsTarget();
            EntityLogic();
        }

        protected abstract void EntityLogic();
    }
}
