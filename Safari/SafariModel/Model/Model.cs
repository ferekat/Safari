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

        private TileMap tileMap;
        private EntityHandler entityHandler;
        private EconomyHandler economyHandler;
        private RoadNetworkHandler roadNetworkHandler;


        public TileMap TileMap { get { return tileMap; } }
        #region Events
        public event EventHandler? NewGameStarted;
        public event EventHandler<GameData>? TickPassed;
        public event EventHandler<bool>? GameOver;
        public event EventHandler? TileMapUpdated;
        #endregion

        public Model()
        {
            entityHandler = new EntityHandler();


            //-------- !!IDEIGLENES!! térkép példányosítás
            
            tileMap = TileMap.CreateMapTmp();
            //-------------

            TileCollision tc = new TileCollision(tileMap);
            MovingEntity.RegisterTileCollision(tc);

            //Alap entityk hozzáadása
            entityHandler.LoadEntity(new Lion(100, 200));

            economyHandler = new EconomyHandler(99999);
            roadNetworkHandler = new RoadNetworkHandler(tileMap);
        }

        #region Get tile and entity based on coordinates
        public (int, int) GetTileFromCoords(int x, int y)
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
            data.tileMap = tileMap.Map;
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
            Tile clickedTile = tileMap.Map[tileX, tileY];

            if (Tile.tileShopMap.ContainsKey(itemName) && Tile.tileShopMap[itemName] is TileType tileToBuy/* && Tile.tileShopMap[itemName] is TileType tileToBuy && Tile.tileInteractionMap[tileToBuy] == clickedTile.Type*/)
            {
                if (economyHandler.BuyTile(tileToBuy))
                {
                    clickedTile.SetType(tileToBuy);
                    OnTileMapUpdated();
                }
                return;
            }
            if (PathTile.pathTileShopMap.ContainsKey(itemName) && PathTile.pathTileShopMap[itemName] is PathTileType pathToBuy && PathTile.CanPlacePath(pathToBuy,clickedTile.Type))
            {
                    //ha be lehet kötni a hálózatba és meg lehet venni
                if (roadNetworkHandler.ConnectToNetwork(clickedTile,pathToBuy) && economyHandler.BuyPathTile(pathToBuy))
                {
                   
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
