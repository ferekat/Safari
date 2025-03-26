using SafariModel.Model.Tiles;
using SafariModel.Model.Utils;
using SafariModel.Persistence;
using SafariModel.Model.InstanceEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SafariModel.Model.AbstractEntity;
using System.Drawing;

namespace SafariModel.Model
{
    public class Model
    {
        
        public static readonly int MAPSIZE = 100;
        private Tile[,] tileMap;

        private EntityHandler entityHandler;

        #region Events
        public event EventHandler? NewGameStarted;
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        #endregion

        public Model()
        {
            entityHandler = new EntityHandler();

            tileMap = new Tile[MAPSIZE,MAPSIZE];
            for (int i = 0; i < MAPSIZE; i++)
            {
                for (int j = 0; j < MAPSIZE; j++)
                {
                    tileMap[i, j] = new Tile(i, j);
                }
            }

            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Lion(100, 200));
            entityHandler.LoadEntity(new Gazelle(1200, 500));
            entityHandler.LoadEntity(new Giraffe(1500, 1000));
            entityHandler.LoadEntity(new Leopard(400, 384));

        }

        #region Get tile and entity based on coordinates
        public (int,int) GetTileFromCoords(int x,int y)
        {
            return (x / Tile.TILESIZE, y / Tile.TILESIZE);
        }
        public int GetEntityIDFromCoords(int x, int y)
        {
            return entityHandler.GetEntityIDFromCoords(x, y);
        }
        #endregion

        #region Tick update
        public void UpdatePerTick()
        {
            //Ide jön gamelogic
            entityHandler.TickEntities();

            InvokeTickPassed();
        }
        #endregion

        private void OnNewGameStarted()
        {
            NewGameStarted?.Invoke(this, EventArgs.Empty);  
        }
        private void InvokeTickPassed()
        {
            GameData data = new GameData();
            //Itt lehet esetleg klónozni jobb lenne az adatokat?
            data.tileMap = tileMap;
            data.entities = entityHandler.GetEntities();
            TickPassed?.Invoke(this, data);
        }

        private void InvokeGameOver()
        {
            bool win = false;
            //Decide if the player won or not
            GameOver?.Invoke(this, win);
        }
       

    }
}
