using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.Utils
{
    public class EntityHandler
    {


       
        private List<Entity> entities = new();
        private List<Carnivore> carnivores = new();
        private List<Herbivore> herbivores = new();
        private List<Hunter> hunters = new();
        private List<Guard> guards = new();
        
        private Random random;

        private int CurrentID;

        public EntityHandler() 
        {
            CurrentID = 0;
            random = new Random();
        }

        public void LoadEntity(Entity entity)
        {
            entities.Add(entity);

            if (entity is Carnivore c) carnivores.Add(c);
            if (entity is Herbivore h) herbivores.Add(h);
            if (entity is Hunter hu) hunters.Add(hu);
            if(entity is Guard g) guards.Add(g);
        }
        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            if (entity is Herbivore h) herbivores.Remove(h);
            if (entity is Carnivore c) carnivores.Remove(c);
            if (entity is Hunter hu) hunters.Remove(hu);
            if (entity is Guard g) guards.Remove(g);
        }

        public Entity? GetEntityByID(int id)
        {
            Entity? e = null;
            foreach (Entity entity in entities)
            {
                if(entity.ID == id) e = entity;
            }
            return e;
        }
        public int GetEntityIDFromCoords(int x, int y)
        {
            foreach(Entity entity in entities) 
            {
                if (x >= entity.X && x <= (entity.X + entity.EntitySize) && y >= entity.Y && y <= (entity.Y + entity.EntitySize)) return entity.ID;
            }
            return -1;
        }

        public void TickEntities()
        {
            foreach (Entity entity in entities.ToList())
            {
                entity.EntityTick();
            }
        }
        public Hunter GetNextHunter()
        {
            return hunters.Last();
        }
        public void SpawnHunter()
        {
            switch (random.Next(4))
            {
                case 0:
                    LoadEntity(new Hunter(38, random.Next((Model.MAPSIZE + 1) * 49)));
                    break;
                case 1:
                    LoadEntity(new Hunter(random.Next((Model.MAPSIZE + 1) * 49), 38));
                    break;
                case 2:
                    LoadEntity(new Hunter((Model.MAPSIZE + 1) * 49 - 1, random.Next((Model.MAPSIZE + 1) * 49)));
                    break;
                case 3:
                    LoadEntity(new Hunter(random.Next((Model.MAPSIZE + 1) * 49), (Model.MAPSIZE + 1) * 49 - 1));
                    break;
            }
        }
        public void TargetHunter(Guard guard, Hunter hunter)
        {
            //SetTarget
        }
        public int GetCarnivoreCount()
        {
            return carnivores.Count;
        }
        public int GetHerbivoreCount()
        {
            return herbivores.Count;
        }
        public List<Entity> GetEntities()
        {
            return entities;
        }
    }
}
