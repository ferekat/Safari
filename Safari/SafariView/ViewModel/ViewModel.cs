using SafariModel.Model;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.Tiles;
using SafariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.CodeDom;
using SafariModel.Model.InstanceEntity;
using System.Diagnostics.Eventing.Reader;
using System.Data.SqlTypes;

namespace SafariView.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        #region Private fields
        private List<TileRender> RenderedTiles;
        public ObservableCollection<EntityRender> RenderedEntities { get; private set; }
        private int money;
        private GameSpeed gameSpeed;
        private int cameraX;
        private int cameraY;

        private (int, int) selectedTile;
        private int selectedEntityID;
        private ClickAction cAction;
        private string selectedShopName;
        //Mennyi tile lesz látható a képernyőn
        private readonly int HORIZONTALTILECOUNT = 38;
        private readonly int VERTICALTILECOUNT = 16;

        private readonly int HORIZONTALCAMERACHANGERANGE = 150;
        private readonly int VERTICALCAMERACHANGERANGE = 200;

        private DispatcherTimer tickTimer;
        private string? indexPage;
        private string? newGamePage;
        private string? creditsPage;
        private string? loadGamePage;
        private string? optionName;

        private string moneyString;
        private string topRowHeightString;
        private string bottomRowHeightString;
        private float topRowHeightRelative;
        private float bottomRowHeightRelative;
        private float mid;

        private Model model;
        #endregion

        #region Tile brushes
        private static Dictionary<TileType, Brush> tileBrushes = new Dictionary<TileType, Brush>()
        {
            {TileType.WATER, new SolidColorBrush(Color.FromRgb(55, 55, 255))},
            { TileType.GROUND, new SolidColorBrush(Color.FromRgb(153, 76, 0))},
            { TileType.EMPTY,new SolidColorBrush(Color.FromRgb(0, 0, 0))},
            { TileType.FENCE,new SolidColorBrush(Color.FromRgb(30, 30, 30))},
            { TileType.HILL,new SolidColorBrush(Color.FromRgb(0, 102, 0))},
            { TileType.ENTRANCE,new SolidColorBrush(Color.FromRgb(255, 0, 0))},
            { TileType.EXIT,new SolidColorBrush(Color.FromRgb(0, 255, 0))}
        };
        #endregion

        #region Entity brushes
        private static Dictionary<Type, Brush> entityBrushes = new Dictionary<Type, Brush>()
        {
            {typeof(Lion),new SolidColorBrush(Color.FromRgb(204,204,0)) },
            {typeof(Leopard),new SolidColorBrush(Color.FromRgb(212,170,33)) },
            {typeof(Gazelle),new SolidColorBrush(Color.FromRgb(189,168,98)) },
            { typeof(Giraffe),new SolidColorBrush(Color.FromRgb(243,226,69))}
        };

        #endregion

        #region ClickAction enum
        public enum ClickAction
        {
            NOTHING,
            BUY,
            SELL,
            SELECT
        }
        #endregion

        #region GameSpeed enum
        public enum GameSpeed
        {
            Slow,
            Medium,
            Fast
        }
        #endregion

        #region Window bindings
        public string IndexPage { get { return indexPage!; } private set { indexPage = value; OnPropertyChanged(); } }
        public string NewGamePage { get { return newGamePage!; } private set { newGamePage = value; OnPropertyChanged(); } }
        public string CreditsPage { get { return creditsPage!; } private set { creditsPage = value; OnPropertyChanged(); } }
        public string LoadGamePage { get { return loadGamePage!; } private set { loadGamePage = value; OnPropertyChanged(); } }
        public string OptionName { get { return optionName!; } private set { optionName = value; OnPropertyChanged(); } }

        public Brush BackgroundBrush { get { return tileBrushes[TileType.GROUND]; } }
        public string TopRowHeightString { get { return topRowHeightString; } private set { topRowHeightString = value; OnPropertyChanged(); } }
        public string BottomRowHeightString { get { return bottomRowHeightString; } private set { bottomRowHeightString = value; OnPropertyChanged(); } }

        public ClickAction CAction { get { return cAction; } private set { cAction = value; OnPropertyChanged(); } }

        public string MoneyString { get { return moneyString; } private set { moneyString = value; OnPropertyChanged(); } }

        public string SelectedShopName { get { return selectedShopName; } private set { selectedShopName = value; OnPropertyChanged(); } }

        #endregion

        #region Properties
        public float Mid { get { return mid; } set { mid = value; OnPropertyChanged(); } }
        public int Money { get { return money; } private set { money = value; MoneyString = $"Money : {money}$"; } }
        public GameSpeed Gamespeed { get { return gameSpeed; } set { gameSpeed = value; OnPropertyChanged(); } }
        private float TopRowHeightRelative { get { return topRowHeightRelative!; } set { topRowHeightRelative = value; TopRowHeightString = topRowHeightRelative.ToString() + "*"; } }
        private float BottomRowHeightRelative { get { return bottomRowHeightRelative!; } set { bottomRowHeightRelative = value; BottomRowHeightString = bottomRowHeightRelative.ToString() + "*"; } }
        #endregion

        #region Commands
        public DelegateCommand SaveGameCommand;
        public DelegateCommand LoadGameCommand;
        public DelegateCommand ExitGameCommand { get; private set; }
        public DelegateCommand NewGamePageCommand { get; private set; }
        public DelegateCommand LoadGamePageCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand CreditsCommand { get; private set; }
        public DelegateCommand ClickedShopIcon { get; private set; }
        public DelegateCommand ChangedGameSpeed;
        #endregion

        #region EventHandlers
        public event EventHandler? ExitGame;
        public event EventHandler? StartGame;
        public event EventHandler? FinishedRendering;
        #endregion

        #region Constructor
        public ViewModel(Model model, List<TileRender> renderedTiles)
        {
            this.model = model;
            RenderedEntities = new ObservableCollection<EntityRender>();
            this.RenderedTiles = renderedTiles;
            tickTimer = new DispatcherTimer();
            tickTimer.Tick += new EventHandler(OnGameTimerTick);
            tickTimer.Interval = TimeSpan.FromSeconds((1 / 120.0));

            //Initialize commands
            SaveGameCommand = new DelegateCommand((param) => SaveGame());
            LoadGameCommand = new DelegateCommand((param) => LoadGame());
            ClickedShopIcon = new DelegateCommand((param) => ClickShop(param));
            ChangedGameSpeed = new DelegateCommand((param) => ChangeGameSpeed(param));
            ExitGameCommand = new DelegateCommand((param) => OnGameExit());
            NewGamePageCommand = new DelegateCommand((param) => OnNewGamePageClicked());
            LoadGamePageCommand = new DelegateCommand((param) => OnLoadPageClicked());
            BackCommand = new DelegateCommand((param) => OnBackClicked());
            StartCommand = new DelegateCommand((param) => OnStartClicked());
            CreditsCommand = new DelegateCommand((param) => OnCreditsClicked());

            //Subscribe to model's events
            model.TickPassed += new EventHandler<GameData>(Model_TickPassed);
            model.GameOver += new EventHandler<bool>(Model_GameOver);
            model.NewGameStarted += new EventHandler(Model_NewGameStarted);

            //Set window bindings
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            LoadGamePage = "Hidden";
            CreditsPage = "Hidden";
            OptionName = "SAFARI";
            CAction = ClickAction.NOTHING;

            TopRowHeightRelative = 0.08F;
            BottomRowHeightRelative = 0.15F;
            Mid = 1 - TopRowHeightRelative - BottomRowHeightRelative;
            selectedTile = (-1, -1);
            selectedEntityID = -1;
        }

        private void Model_NewGameStarted(object? sender, EventArgs e)
        {
            CreditsPage = "Hidden";
            LoadGamePage = "Hidden";
            OptionName = "SAFARI";
            StartGame?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Command methods
        private void SaveGame()
        {
            throw new NotImplementedException();
        }

        private void LoadGame()
        {
            throw new NotImplementedException();
        }

        private void ClickShop(object? clickParam)
        {

            if (clickParam is string shopString)
            {

                if (shopString == "Sell")
                {
                    CAction = CAction == ClickAction.SELL ? ClickAction.NOTHING : ClickAction.SELL;
                    SelectedShopName = "";
                    return;
                }
                if (CAction != ClickAction.BUY)
                {
                    CAction = ClickAction.BUY;
                    SelectedShopName = shopString;
                }
                else if(selectedShopName.Equals(shopString))
                {
                    CAction = ClickAction.NOTHING;
                    SelectedShopName = "";
                }
                else
                {
                    SelectedShopName = shopString;
                }
            }
        }

        private void ChangeGameSpeed(object? speedValue)
        {
            throw new NotImplementedException();
        }

        private void OnGameExit()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGamePageClicked()
        {
            IndexPage = "Hidden";
            NewGamePage = "Visible";
            OptionName = "New Game";
        }
        private void OnBackClicked()
        {
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            CreditsPage = "Hidden";
            LoadGamePage = "Hidden";
            OptionName = "SAFARI";
        }
        private void OnStartClicked()
        {
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            CreditsPage = "Hidden";
            LoadGamePage = "Hidden";
            OptionName = "SAFARI";

            StartGame?.Invoke(this, EventArgs.Empty);
            tickTimer.Start();

        }
        private void OnCreditsClicked()
        {
            IndexPage = "Hidden";
            CreditsPage = "Visible";
            OptionName = "Credits";
        }
        private void OnLoadPageClicked()
        {
            IndexPage = "Hidden";
            LoadGamePage = "Visible";
            OptionName = "Load Game";
        }
        #endregion

        #region Model event handlers
        private void Model_TickPassed(object? sender, GameData data)
        {
            Point mouse = Mouse.GetPosition(Application.Current.MainWindow);
            if (mouse.X > 0 && mouse.X < HORIZONTALCAMERACHANGERANGE) cameraX -= 10;
            if (mouse.X > HORIZONTALTILECOUNT * Tile.TILESIZE - HORIZONTALCAMERACHANGERANGE && mouse.X < HORIZONTALTILECOUNT * Tile.TILESIZE) cameraX += 10;
            if (mouse.Y > 0 && mouse.Y < VERTICALCAMERACHANGERANGE) cameraY -= 10;
            if (mouse.Y > VERTICALTILECOUNT * Tile.TILESIZE - VERTICALCAMERACHANGERANGE && mouse.Y < VERTICALTILECOUNT * Tile.TILESIZE) cameraY += 10;

            RenderGameArea(data.tileMap, data.entities);

            Money = data.money;
        }

        private void Model_GameOver(object? sender, bool playerWin)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Game area click handler
        public void ClickPlayArea(object? sender, Point p)
        {
            int gameX = cameraX + (int)p.X;
            int gameY = cameraY + (int)p.Y;

            if (CAction == ClickAction.SELECT)
            {
                selectedTile = model.GetTileFromCoords(gameX, gameY);
                selectedEntityID = model.GetEntityIDFromCoords(gameX, gameY);
            }
            if(CAction == ClickAction.BUY)
            {
                model.BuyEntity(SelectedShopName, gameX, gameY);
            }

            if (CAction == ClickAction.SELL)
            {
                selectedEntityID = model.GetEntityIDFromCoords(gameX, gameY);
                if(selectedEntityID != -1)
                {
                    model.SellEntity(selectedEntityID);
                }
            }
        }
        #endregion

        #region Private methods
        private void RenderGameArea(Tile[,] tileMap, List<Entity> entities)
        {

            //render tiles

            if (cameraX < 0) cameraX = 0;
            if (cameraY < 0) cameraY = 0;

            if (cameraX > (Model.MAPSIZE - HORIZONTALTILECOUNT) * Tile.TILESIZE) cameraX = (Model.MAPSIZE - HORIZONTALTILECOUNT) * Tile.TILESIZE;
            if (cameraY > (Model.MAPSIZE - VERTICALTILECOUNT) * Tile.TILESIZE) cameraY = (Model.MAPSIZE - VERTICALTILECOUNT) * Tile.TILESIZE;


            int cameraXLeft = cameraX - Tile.TILESIZE;
            int cameraYUp = cameraY - Tile.TILESIZE;

            int tileMapLeft = cameraXLeft / Tile.TILESIZE;
            int tileMapTop = cameraYUp / Tile.TILESIZE;


            if (tileMapLeft < 0) tileMapLeft = 0;
            if (tileMapTop < 0) tileMapTop = 0;

            RenderedTiles.Clear();

            for (int j = tileMapTop; j < Math.Min(tileMapTop + VERTICALTILECOUNT + 3, Model.MAPSIZE); j++)
            {
                for (int i = tileMapLeft; i < Math.Min(tileMapLeft + HORIZONTALTILECOUNT + 2, Model.MAPSIZE); i++)
                {
                    Tile t = tileMap[i, j];

                    int tileScaledX = t.I * Tile.TILESIZE;
                    int tileScaledY = t.J * Tile.TILESIZE;

                    //Convert to real coordinates and add to render list
                    int realX = tileScaledX - cameraXLeft - Tile.TILESIZE;
                    int realY = tileScaledY - cameraYUp - Tile.TILESIZE;

                    Brush? b = null;

                    //Get type of tile
                    b = tileBrushes[t.Type];

                    /* Set currently selected tile's color to yellow
                    if((i,j) == selectedTile) b = new SolidColorBrush(Color.FromRgb(252, 240, 3));
                    */

                    TileRender tile = new TileRender(realX, realY, b!);

                    RenderedTiles.Add(tile);
                }
            }

            //render entities

            RenderedEntities.Clear();

            foreach (Entity e in entities)
            {
                if (e.X >= cameraXLeft && e.X <= cameraXLeft + ((HORIZONTALTILECOUNT + 1) * Tile.TILESIZE) && e.Y >= cameraYUp && e.Y <= cameraYUp + ((VERTICALTILECOUNT + 2) * Tile.TILESIZE))
                {
                    /* Set currently selected entity's color to blue
                    if(e.ID == selectedEntityID) RenderedEntities.Add(new EntityRender(e.X - cameraX, e.Y - cameraY, new SolidColorBrush(Color.FromRgb(30,30,255)), e.EntitySize));
                    else RenderedEntities.Add(new EntityRender(e.X - cameraX, e.Y - cameraY, entityBrushes[e.GetType()], e.EntitySize));
                    */

                    RenderedEntities.Add(new EntityRender(e.X - cameraX, e.Y - cameraY, entityBrushes[e.GetType()], e.EntitySize));
                }
            }

            FinishedRender();
        }

        private void OnGameTimerTick(object? sender, EventArgs e)
        {
            model.UpdatePerTick();
        }

        private void FinishedRender()
        {
            FinishedRendering?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}

