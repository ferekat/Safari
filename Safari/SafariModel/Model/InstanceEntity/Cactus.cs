using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Cactus : Plant
    {
        #region Constructor
        public Cactus(int x, int y) : base(x, y, 50) 
        {
            entitySize = 10;
        }
        #endregion
    }
}
