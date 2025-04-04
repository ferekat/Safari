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
using System.Diagnostics;

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
        public event EventHandler? TileMapUpdated;
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

            TileCollision tc = new TileCollision(tileMap);
            MovingEntity.RegisterTileCollision(tc);

            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Lion(100, 200));

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

        private void OnTileMapUpdated()
        {
            TileMapUpdated?.Invoke(this, EventArgs.Empty);
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
       
        
        
        public void BuyItem(string itemName, int x, int y)
        {
            int tileX = GetTileFromCoords(x, y).Item1;
            int tileY = GetTileFromCoords(x, y).Item2;
            Tile clickedTile = tileMap[tileX, tileY];
            
            if (Tile.tileTypeMap.ContainsKey(itemName) && Tile.tileTypeMap[itemName] is TileType tiletype)
            {
                if (economyHandler.BuyTile(clickedTile.Type,tiletype))
                {
                    clickedTile.SetType(tiletype);
                    OnTileMapUpdated();
                }
                return;
            }
            if ( Tile.placeableMap.ContainsKey(itemName) && Tile.placeableMap[itemName] is TilePlaceable placeable)
            {
                if (economyHandler.BuyPlaceable(clickedTile.Type,placeable))
                {
                    clickedTile.SetPlaceable(placeable);
                    OnTileMapUpdated();
                }
                return;
            }
            Entity? entity = EntityFactory.CreateEntity(itemName, x, y);
            Type? type = entity?.GetType();

            

      
            if (entity == null) return;

            if (entity is Guard guardEntity)
            {
                if (!economyHandler.PaySalary(guardEntity)) return;
            }

            if (!economyHandler.BuyEntity(type!)) return;

          

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
