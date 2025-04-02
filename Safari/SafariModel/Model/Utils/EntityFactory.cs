using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;

namespace SafariModel.Model.Utils
{
    public class EntityFactory
    {
        public static Entity? CreateEntity(string name,int x, int y)
        {
            switch (name)
            {
                case "Guard": return new Guard(x, y);
                case "Lion": return new Lion(x, y); 
                case "Leopard": return new Leopard(x, y);
                case "Gazelle": return new Gazelle(x, y);
                case "Giraffe": return new Giraffe(x, y);
                case "Cactus": return new Cactus(x, y);
                case "Greasewood": return new Greasewood(x, y);
                case "PalmTree": return new PalmTree(x, y);
                default: return null;
            }
        }
    }
}
