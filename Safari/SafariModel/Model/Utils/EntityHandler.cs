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
        //entityk térképen való elhelyezkedésének lekérdezése
        public (int, int) GetCellCoords(Entity e, int tileSize) => (e.X / tileSize, e.Y / tileSize);
        // a térképen lévő cellákban lévő entityk lekérdezése
        public void UpdateSpatialMap(Dictionary<(int, int), List<Entity>> spatialMap, int tileSize)
        {
            spatialMap.Clear();
            foreach (Entity entity in entities)
            {
                var coords = GetCellCoords(entity, tileSize);
                if (!spatialMap.ContainsKey(coords))
                {
                    spatialMap[coords] = new List<Entity>();
                }
                spatialMap[coords].Add(entity);
            }
        }
        public void TickEntities()
        {
            foreach (Entity entity in entities.ToList())
            {
                entity.EntityTick();
            }
        }
        public Hunter? GetNextHunter(int speed)
        {
            if (hunters.Count == 0) SpawnHunter(speed);
            return hunters.Last();
        }
        public void SpawnHunter(int speed)
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
                hunter!.EnterField /= speed;
                hunter!.KilledAnimal += new EventHandler<KillAnimalEventArgs>(KillAnimal);
                hunter!.HunterTarget += new EventHandler<HunterTargetEventArgs>(SetHunterTarget);
                hunter!.GunmanRemove += new EventHandler<GunmanRemoveEventArgs>(RemoveGunman);
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
        public List<Guard> GetGuards()
        {
            return guards;
        }
        public void KillAnimal(object? sender, KillAnimalEventArgs e)
        {
            RemoveEntity(e.Animal);
        }

        public void SetHunterTarget(object? sender, HunterTargetEventArgs e)
        {
            e.Hunter.TargetAnimal = SetHunterTargetAnimal();
        }
        public void RemoveGunman(object? sender, GunmanRemoveEventArgs e)
        {
            RemoveEntity(e.Gunman);
        }
    }
}
