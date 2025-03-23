using SafariModel.Model.AbstractEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.InstanceEntity
{
    public class PalmTree : Plant
    {
        #region Constructor
        public PalmTree(int x, int y) : base(x, y, 40) { }
        #endregion
    }
}
