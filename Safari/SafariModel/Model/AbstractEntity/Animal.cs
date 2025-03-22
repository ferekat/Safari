using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class Animal : MovingEntity
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
        #endregion

        #region Properties
        public int Age { get; protected set; }
        public int Hunger { get; protected set; }
        public int Thirst { get; protected set; }
        public int Health { get; protected set; }
        public AnimalActions Action { get; protected set; }
        public bool IsLeader { get; private set; }
        #endregion

        #region Constructor
        protected Animal(int x, int y, int age, int health, int hunger, int thrist) : base(x, y)
        {
            Age = age;
            Hunger = hunger;
            Thirst = thrist;
            Health = health;
            Action = AnimalActions.Resting;

            exploredFoodPlaces = new List<Point>();
            exploredWaterPlaces = new List<Point>();
            members = new List<Animal>();
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

        protected void Wander()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
