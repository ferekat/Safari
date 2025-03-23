using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;
using SafariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model
{
    public class Model
    {
        private static readonly int MAP_SIZE = 100;
        private static readonly int STARTING_MONEY = 10000;
        private Tile[,] tileMap;
        private EconomyHandler economyHandler = new(STARTING_MONEY);
        private EntityHandler entityHandler = new();



        #region Events
        public event EventHandler NewGameStarted;
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        #endregion

        public Model()
        {
            tileMap = new Tile[MAP_SIZE,MAP_SIZE];
        }
        #region Event methods
        public void NewGame()
        {
            //tilemap feltöltése

            OnNewGameStarted();
        }

        private void OnNewGameStarted()
        {
            NewGameStarted?.Invoke(this, EventArgs.Empty);  
        }
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
