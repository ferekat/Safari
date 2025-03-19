using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Giraffe : Herbivore
    {
        #region Constructor
        public Giraffe(int x, int y) : base(x, y, 0, 350, 150, 200)
        {

        }
        #endregion
    }
}
