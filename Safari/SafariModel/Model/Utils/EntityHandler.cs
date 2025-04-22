using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using SafariModel.Model.EventArgsClasses;
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

        private Dictionary<int, Entity> entitiesByID = new();
        
        private Random random;

        public EntityHandler() 
        {
            random = new Random();
        }

        public void LoadEntity(Entity entity)
        {
            entities.Add(entity);

            if (entity is Carnivore c) carnivores.Add(c);
            if (entity is Herbivore h) herbivores.Add(h);
            if (entity is Hunter hu) hunters.Add(hu);
            if(entity is Guard g) guards.Add(g);

            entitiesByID.Add(entity.ID, entity);
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
            if (entitiesByID.ContainsKey(id)) e = entitiesByID[id];
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
        public Hunter? GetNextHunter()
        {
            if (hunters.Count == 0) SpawnHunter();
            return hunters.Last();
        }
        public void SpawnHunter()
        {
            if (hunters.Count < 10)
            {
                Hunter? hunter = null;
                switch (random.Next(4))
                {
                    case 0:
                        hunter = new Hunter(50, random.Next(50, (Model.MAPSIZE + 1) * 49 - 11), SetHunterTargetAnimal());
                        break;
                    case 1:
                        hunter = new Hunter(random.Next(50, (Model.MAPSIZE + 1) * 49 - 11), 50, SetHunterTargetAnimal());
                        break;
                    case 2:
                        hunter = new Hunter((Model.MAPSIZE + 1) * 49 - 12, random.Next(50, (Model.MAPSIZE + 1) * 49 - 11), SetHunterTargetAnimal());
                        break;
                    case 3:
                        hunter = new Hunter(random.Next(50, (Model.MAPSIZE + 1) * 49 - 11), (Model.MAPSIZE + 1) * 49 - 12, SetHunterTargetAnimal());
                        break;
                }
                hunter!.KilledAnimal += new EventHandler<KillAnimalEventArgs>(KillAnimal);
                hunter!.HunterTarget += new EventHandler<HunterTargetEventArgs>(SetHunterTarget);
                hunter!.HunterEscaped += new EventHandler<HunterEscapeEventArgs>(RemoveHunter);
                LoadEntity(hunter);
            }
        }
        public Animal? SetHunterTargetAnimal()
        {
            int car = GetCarnivoreCount();
            int her = GetHerbivoreCount();
            if (car == 0 && her == 0) return null;
            if (car == 0)
            {
                return herbivores[random.Next(her)];
            }
            if (her == 0)
            {
                return carnivores[random.Next(car)];
            }
            if (random.Next(2) == 0)
            {
                return carnivores[random.Next(car)];
            }
            else
            {
                return herbivores[random.Next(her)];
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
        public void KillAnimal(object? sender, KillAnimalEventArgs e)
        {
            RemoveEntity(e.Animal);
        }

        public void SetHunterTarget(object? sender, HunterTargetEventArgs e)
        {
            e.Hunter.TargetAnimal = SetHunterTargetAnimal();
        }
        public void RemoveHunter(object? sender, HunterEscapeEventArgs e)
        {
            RemoveEntity(e.Hunter);
        }
    }
}
