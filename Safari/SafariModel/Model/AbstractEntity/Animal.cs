using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public abstract class Animal : MovingEntity
    {
        #region AnimalActions Enum
        public enum AnimalActions
        {
            Resting,
            GoEat,
            GoDrink,
            Wandering
        }
        #endregion
       
        #region Private fields
        private List<Point> exploredFoodPlaces;
        private List<Point> exploredWaterPlaces;
        private HashSet<(int, int)> exploredFoodChunks;
        private HashSet<(int, int)> exploredWaterChunks;
        private List<Animal> members;
        private int wanderTimer;
        private int searchTimer;
        private Random random;
        private int interactionRange;

        private Entity? targetedFood;
        private Point targetedWater;

        private int searchLimit;
        private int hungerLimit;
        private int thirstLimit;
        private int healLimit;
        private int healTimer;
        #endregion

        #region Properties
        public int Age { get; protected set; }
        public int Hunger { get; protected set; }
        public int Thirst { get; protected set; }
        public int Food { get; protected set; }
        public int Water { get; protected set; }
        public int Health { get; protected set; }
        public AnimalActions Action { get; protected set; }
        public bool IsLeader { get; private set; }

        //Debug
        public int ExploredFoodCount { get { return exploredFoodPlaces.Count; } }
        public int ExploredWaterCount { get { return exploredWaterPlaces.Count; } }
        public int SearchTimer { get { return searchTimer; } }

        #endregion

        #region Constructor
        protected Animal(int x, int y, int age, int health, int food, int water, int hunger, int thirst) : base(x, y)
        {
            Age = age;
            Food = food;
            Water = water;
            Hunger = hunger;
            Thirst = thirst;
            Health = health;
            Action = AnimalActions.Resting;

            hungerLimit = 1000;
            thirstLimit = 600;
            healLimit = 1200;
            searchLimit = 360;
            searchTimer = 0;
            healTimer = 0;
            interactionRange = 70;

            exploredFoodPlaces = new List<Point>();
            exploredWaterPlaces = new List<Point>();
            exploredFoodChunks = new HashSet<(int, int)>();
            exploredWaterChunks = new HashSet<(int, int)>();
            members = new List<Animal>();

            //random járkálás
            random = new Random();
            wanderTimer = random.Next(300);
        }
        #endregion

        #region Methods

        protected void SearchForFood()
        {
            if(ReachedTarget && !IsMoving)
            {
                List<Entity> entitiesInRange = GetEntitiesInRange();
                foreach (Entity e in entitiesInRange)
                {
                    if (IsPreferredFood(e))
                    {
                        targetedFood = e;
                        SetTarget(new Point(e.X, e.Y));
                        return;
                    }
                }
                //Couldn't find food source
                //if this point is in the explored food points, remove it (because there is no accessible source from here)
                Point current = new Point(this.X, this.Y);
                if(exploredFoodPlaces.Contains(current))
                {
                    exploredFoodPlaces.Remove(current);
                    exploredFoodChunks.Remove(GetChunkCoordinates());
                }
                if (exploredWaterPlaces.Count > 0)
                {
                    SetTarget(exploredWaterPlaces[random.Next(0, exploredWaterPlaces.Count)]);
                }
                else //search randomly for food, if there are no explored food sources
                {
                    int newX = random.Next(-600, 600);
                    int newY = random.Next(-600, 600);
                    this.SetTarget(new Point(this.x + newX, this.y + newY));
                }
            }
            //if the animal found a point, go to it until it is in interaction range
            if (targetedFood != null)
            {
                if(DistanceToEntity(targetedFood) < interactionRange)
                {
                    EatFood(targetedFood);
                }
            }
        }

        protected void SearchForWater()
        {
            if (ReachedTarget && !IsMoving)
            {
                List<Tile> tilesInRange = GetTilesInRange();
                foreach (Tile t in tilesInRange)
                {
                    if (t.Type == TileType.WATER)
                    {
                        //Only go to water tile if it has a walkable tile adjacent to it
                        if(IsAccessibleTile(t.I,t.J,out (int,int) target))
                        {
                            int tileX = t.I * Tile.TILESIZE;
                            int tileY = t.J * Tile.TILESIZE;
                            int targetX = target.Item1 * Tile.TILESIZE;
                            int targetY = target.Item2 * Tile.TILESIZE;
                            targetedWater = new Point(tileX, tileY);
                            SetTarget(new Point(targetX, targetY));
                            return;
                        }
                    }
                }
                //Couldn't find water source
                //if this point is in the explored water points, remove it (because there is no accessible source from here)
                Point current = new Point(this.X, this.Y);
                if (exploredWaterPlaces.Contains(current))
                {
                    exploredWaterPlaces.Remove(current);
                    exploredWaterChunks.Remove(GetChunkCoordinates());
                }
                if(exploredWaterPlaces.Count > 0)
                {
                    SetTarget(exploredWaterPlaces[random.Next(0, exploredWaterPlaces.Count)]);
                }
                else //search randomly for water, if there are no explored water sources
                {
                    int newX = random.Next(-600, 600);
                    int newY = random.Next(-600, 600);
                    this.SetTarget(new Point(this.x + newX, this.y + newY));
                }
            }
            //if the animal found a point, go to it until it is in interaction range
            if (targetedWater.X >= 0 && targetedWater.Y >= 0)
            {
                if (Math.Sqrt(Math.Pow(this.X-targetedWater.X,2)+Math.Pow(this.Y-targetedWater.Y,2)) < interactionRange)
                {
                    DrinkWater();
                }
            }
        }
        protected void Follow(MovingEntity entity)
        {
            throw new NotImplementedException();
        }

        protected void RandomWander()
        {
            wanderTimer = random.Next(300);
            int newX = random.Next(-300, 300);
            int newY = random.Next(-300, 300);
            this.SetTarget(new Point(this.x + newX,this.y+newY));
        }

        protected override void EntityLogic()
        {
            PassTick();
            Action = SelectAction();
            switch(Action)
            {
                case AnimalActions.Resting: Rest();break;
                case AnimalActions.Wandering: Wander(); break;
                case AnimalActions.GoEat: GoEat(); break;
                case AnimalActions.GoDrink: GoDrink(); break;
            }

            AnimalLogic();
        }

        private void PassTick()
        {
            if (++Hunger >= hungerLimit)
            {
                Hunger = 0;
                if (--Food < 0) --Health;
            }
            if (++Thirst >= thirstLimit)
            {
                Thirst = 0;
                if (--Water < 0) --Health;
            }
        }

        private AnimalActions SelectAction()
        {
            if (Water < 40) return AnimalActions.GoDrink;
            if (Food < 40) return AnimalActions.GoEat;
            if (Water < 80 || Food < 80) return AnimalActions.Wandering;

            return AnimalActions.Resting;
        }

        private void Search()
        {
            if(++searchTimer >= searchLimit)
            {
                searchTimer = 0;
                CheckArea();
            }
        }

        private void DrinkWater()
        {
            Water = 100;
            targetedWater.X = -1;
            targetedWater.Y = -1;
        }

        private void EatFood(Entity e)
        {
            EatInteraction(e);
            targetedFood = null;
        }

        private void CheckArea()
        {
            List<Tile> tilesInRange = GetTilesInRange();
            List<Entity> entitiesInRange = GetEntitiesInRange();

            foreach (Tile t in tilesInRange)
            {
                if(t.Type == TileType.WATER)
                {
                    //mark water's location (limited to 1 per chunk)
                    if(!exploredWaterChunks.Contains(GetChunkCoordinates()))
                    {
                        exploredWaterChunks.Add(GetChunkCoordinates());
                        exploredWaterPlaces.Add(new Point(this.X,this.Y));
                    }
                }
            }

            foreach(Entity e in entitiesInRange)
            {
                if (IsPreferredFood(e))
                {
                    //mark food's location (limited to 1 per chunk)
                    if (!exploredFoodChunks.Contains(GetChunkCoordinates()))
                    {
                        exploredFoodChunks.Add(GetChunkCoordinates());
                        exploredFoodPlaces.Add(new Point(this.X, this.Y));
                    }
                }
            }
        }

        #region Action methods

        private void GoEat()
        {
            Search();
            SearchForFood();
        }

        private void GoDrink()
        {
            Search();
            SearchForWater();
        }

        private void Wander()
        {
            wanderTimer--;
            if (wanderTimer <= 0) RandomWander();
            Search();
        }
        private void Rest()
        {
            if(++healTimer >= healLimit)
            {
                ++Health;
                healTimer = 0;
            }
        }
        #endregion

        protected abstract void AnimalLogic();

        protected abstract bool IsPreferredFood(Entity e);

        protected abstract void EatInteraction(Entity e);

        #endregion
    }
}
