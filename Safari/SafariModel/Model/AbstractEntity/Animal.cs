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
        private List<Animal> members;
        private int wanderTimer;
        private Random random;

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
            healTimer = 0;

            exploredFoodPlaces = new List<Point>();
            exploredWaterPlaces = new List<Point>();
            members = new List<Animal>();

            //random járkálás
            random = new Random();
            wanderTimer = random.Next(600);
        }
        #endregion

        #region Methods

        protected void SearchForFood()
        {
            throw new NotImplementedException();
        }

        protected void SearchForWater()
        {
            throw new NotImplementedException();
        }
        protected void Follow(MovingEntity entity)
        {
            throw new NotImplementedException();
        }

        protected void RandomWander()
        {
            wanderTimer = random.Next(600);
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
            
            if (Water < 80 && Food < 80) return AnimalActions.Wandering;
            if (Water < 40) return AnimalActions.GoDrink;
            if (Food < 40) return AnimalActions.GoEat;

            return AnimalActions.Resting;
        }

        #region Action methods

        private void GoEat()
        {

        }

        private void GoDrink()
        {

        }

        private void Wander()
        {
            wanderTimer--;
            if (wanderTimer <= 0) RandomWander();
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

        #endregion
    }
}
