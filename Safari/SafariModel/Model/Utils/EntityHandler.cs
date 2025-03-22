using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public EntityHandler() { }

        public void LoadEntity(Entity entity)
        {

        }
        public void RemoveEntity(Entity entity)
        {

        }

        public void TickEntities()
        {
            foreach (Entity entity in entities)
            {
                entity.EntityTick();
            }
        }
        public void SpawnHunter()
        {
            //Spawn mechanika TileType.Fence-re
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
