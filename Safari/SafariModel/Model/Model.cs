using SafariModel.Model.Tiles;
using SafariModel.Persistence;
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

        #region Events
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        #endregion

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

        #region Event methods
        private void InvokeTickPassed()
        {
            GameData data = new GameData();
            //Copy game state to data
            //...
            TickPassed?.Invoke(this, data);
        }

        private void InvokeGameOver()
        {
            bool win = false;
            //Decide if the player won or not
            GameOver?.Invoke(this, win);
        }
        #endregion

    }
}
