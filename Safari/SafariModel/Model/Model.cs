using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Model
    {



        private const int MAPSIZE = 30;
        private Tile[,] tileMap;

        public Model()
        {
            tileMap = new Tile[MAPSIZE,MAPSIZE];
            for (int i = 0; i < MAPSIZE; i++)
            {
                for (int j = 0; j < MAPSIZE; j++)
                {
                    tileMap[i, j] = new Tile(i, j);
                }
            }
        }

    }
}
