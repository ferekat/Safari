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
        private EconomyHandler economyHandler;

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
                    tileMap[i, j] = new Tile(i, j,null);
                }
            }

            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Lion(100, 200));
            entityHandler.LoadEntity(new Gazelle(1200, 500));
            entityHandler.LoadEntity(new Giraffe(1500, 1000));
            entityHandler.LoadEntity(new Leopard(400, 384));

            economyHandler = new EconomyHandler(9999);

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
            data.money = economyHandler.Money;
            TickPassed?.Invoke(this, data);
        }

        private void InvokeGameOver()
        {
            bool win = false;
            //Decide if the player won or not
            GameOver?.Invoke(this, win);
        }
       
        
        
        public void BuyItem(string name, int x, int y)
        {
            int tileX = GetTileFromCoords(x, y).Item1;
            int tileY = GetTileFromCoords(x, y).Item2;
            if (Tile.tileTypeMap.ContainsKey(name) && Tile.tileTypeMap[name] is TileType tiletype)
            {
                if (economyHandler.BuyTile(tiletype))
                {
                    tileMap[tileX,tileY].SetType(tiletype);
                }
            }
            if ( Tile.tileConditionMap.ContainsKey(name) && Tile.tileConditionMap[name] is TileCondition cond )
            {
                if (economyHandler.BuyTileCondition(cond))
                {
                    tileMap[tileX,tileY].SetCondition(cond);
                }
            }

            Type? type = null;
            Entity? entity = null;
            
            switch(name)
            {
                case "Lion": type = typeof(Lion); entity = new Lion(x,y); break;
                case "Leopard": type = typeof(Leopard); entity = new Leopard(x, y); break;
                case "Gazelle": type = typeof(Gazelle); entity = new Gazelle(x, y); break;
                case "Giraffe": type = typeof(Giraffe); entity = new Giraffe(x, y); break;
                case "Cactus": type = typeof(Cactus); entity = new Cactus(x, y); break;
                case "Greasewood": type = typeof(Greasewood); entity = new Greasewood(x, y); break;
                case "PalmTree": type = typeof(PalmTree); entity = new PalmTree(x, y); break;
                
               
            }


            if (type == null) return;

            if (!economyHandler.BuyEntity(type!)) return;

          

            if (entity == null) return;
            entityHandler.LoadEntity(entity!);

        }
        public void SellEntity(int id)
        {
            Entity? e = entityHandler.GetEntityByID(id);
            if (e == null) return;

            economyHandler.SellEntity(e.GetType());
            entityHandler.RemoveEntity(e);
        }
    }
}
