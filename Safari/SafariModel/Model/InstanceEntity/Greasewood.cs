using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class Greasewood : Plant
    {
        #region Constructor
        public Greasewood(int x, int y) : base(x, y, 20) 
        {
            entitySize = 18;
        }
        #endregion
    }
}
