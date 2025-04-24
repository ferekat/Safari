using SafariModel.Model;
using SafariModel.Model.AbstractEntity;
using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using SafariModel.Persistence;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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
        private GameData? cachedGameData;
        //Mennyi tile lesz látható a képernyőn
        private readonly int HORIZONTALTILECOUNT = 38;
        private readonly int VERTICALTILECOUNT = 16;

        private readonly int HORIZONTALCAMERACHANGERANGE = 150;
        private readonly int VERTICALCAMERACHANGERANGE = 150;
        private readonly int CAMERASPEED = 10;
        private bool force_render_next_frame;

        private int camchange_x = 0;
        private int camchange_y = 0;

        private DispatcherTimer tickTimer;
        private DispatcherTimer renderTimer;
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
            {TileType.DEEP_WATER, new SolidColorBrush(Color.FromRgb(0, 45, 179))},
            {TileType.SHALLOW_WATER,new SolidColorBrush(Color.FromRgb(51, 204, 255))},
            { TileType.GROUND, new SolidColorBrush(Color.FromRgb(0, 204, 0))},
             { TileType.GROUND_SMALL, new SolidColorBrush(Color.FromRgb(119, 255, 51))},
            { TileType.SMALL_HILL, new SolidColorBrush(Color.FromRgb(230, 230, 0))},
             { TileType.SMALL_MEDIUM, new SolidColorBrush(Color.FromRgb(255, 153, 102))},
                { TileType.MEDIUM_HILL, new SolidColorBrush(Color.FromRgb(255, 51, 0))},
                 { TileType.MEDIUM_HIGH, new SolidColorBrush(Color.FromRgb(255, 102, 153))},
            { TileType.HIGH_HILL,new SolidColorBrush(Color.FromRgb(204, 0, 204))},
            { TileType.FENCE,new SolidColorBrush(Color.FromRgb(30, 30, 30))},
            { TileType.ENTRANCE,new SolidColorBrush(Color.FromRgb(255, 0, 0))},
            { TileType.EXIT,new SolidColorBrush(Color.FromRgb(0, 255, 0))},
           
        };

        private static Dictionary<PathTileType, Brush> pathBrushes = new Dictionary<PathTileType, Brush>()
        {
            {PathTileType.EMPTY,new SolidColorBrush(Color.FromRgb(0,0,0)) },
            {PathTileType.ROAD,new SolidColorBrush(Color.FromRgb(235, 125, 52) )},
            {PathTileType.LARGE_BRIDGE,new SolidColorBrush(Color.FromRgb(125, 37, 37)) },
            {PathTileType.SMALL_BRIDGE,new SolidColorBrush(Color.FromRgb(140, 136, 136) )},
            {PathTileType.NODE,new SolidColorBrush(Color.FromRgb(100,100,0))},
        };
        #endregion

        #region Entity brushes
        private static Dictionary<Type, Brush> entityBrushes = new Dictionary<Type, Brush>()
        {
            {typeof(Lion),new SolidColorBrush(Color.FromRgb(204,204,0)) },
            {typeof(Leopard),new SolidColorBrush(Color.FromRgb(212,170,33)) },
            {typeof(Gazelle),new SolidColorBrush(Color.FromRgb(189,168,98)) },
            { typeof(Giraffe),new SolidColorBrush(Color.FromRgb(243,226,69))},
            {typeof(Cactus),new SolidColorBrush(Color.FromRgb(107,168,50)) },
            {typeof(Greasewood),new SolidColorBrush(Color.FromRgb(143,168,50)) },
            {typeof(PalmTree),new SolidColorBrush(Color.FromRgb(62,168,50)) },
            {typeof(Guard),new SolidColorBrush(Color.FromRgb(0,0,0)) },
            { typeof(Jeep),new SolidColorBrush(Color.FromRgb(226, 105, 240))}
        };
        private static Brush HillBrush(Tile hill)
        {
            return new SolidColorBrush(Color.FromRgb(0, (byte)(102 + hill.H), 0));
        }

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
        private float TopRowHeightRelative { get { return topRowHeightRelative!; } set { topRowHeightRelative = value; TopRowHeightString = topRowHeightRelative.ToString(CultureInfo.CreateSpecificCulture("C")) + "*"; } }
        private float BottomRowHeightRelative { get { return bottomRowHeightRelative!; } set { bottomRowHeightRelative = value; BottomRowHeightString = bottomRowHeightRelative.ToString(CultureInfo.CreateSpecificCulture("C")) + "*"; } }
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
        public event EventHandler<(int, int)>? RequestCameraChange;
        #endregion

        #region Constructor
        public ViewModel(Model model, List<TileRender> renderedTiles)
        {
            this.model = model;
            RenderedEntities = new ObservableCollection<EntityRender>();
            this.RenderedTiles = renderedTiles;
            tickTimer = new DispatcherTimer(DispatcherPriority.Normal);
            tickTimer.Tick += new EventHandler(OnGameTimerTick);
            tickTimer.Interval = TimeSpan.FromSeconds(1 / 120.0);

            renderTimer = new DispatcherTimer(DispatcherPriority.Render);
            renderTimer.Tick += new EventHandler(OnRenderTick);
            renderTimer.Interval = TimeSpan.FromSeconds((1 / 120.0));

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
            model.TileMapUpdated += new EventHandler(Model_TileMapUpdated);

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

            force_render_next_frame = true;
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
                if (shopString == "Jeep")
                {
                    Jeep dummy = new Jeep(0, 0);
                    System.Drawing.Point p = model.TileMap.Entrance.TileCenterPoint(dummy); //a jeepet rárakjuk a bejárat tile közepére
                    model.BuyItem("Jeep",p.X,p.Y);
                  
                    return;
                }
                if (CAction != ClickAction.BUY)
                {
                    CAction = ClickAction.BUY;
                    SelectedShopName = shopString;
                }
                else if (selectedShopName.Equals(shopString))
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
            renderTimer.Start();

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
            cachedGameData = data;
            Money = data.money;
        }

        private void Model_GameOver(object? sender, bool playerWin)
        {
            throw new NotImplementedException();
        }

        private void Model_TileMapUpdated(object? sender, EventArgs e)
        {
            force_render_next_frame = true;
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
            if (CAction == ClickAction.BUY)
            {
               
                model.BuyItem(SelectedShopName, gameX, gameY);
            }

            if (CAction == ClickAction.SELL)
            {
                selectedEntityID = model.GetEntityIDFromCoords(gameX, gameY);
                if (selectedEntityID != -1)
                {
                    model.SellEntity(selectedEntityID);
                }
            }
        }
        #endregion

        #region Camera movement
        public void MainWindow_CameraMovement(object? sender, (int, int) change)
        {
            camchange_x = change.Item1;
            camchange_y = change.Item2;
        }
        #endregion

        #region Private methods
        private void RenderGameArea()
        {

            if (cachedGameData == null) return;

            Tile[,] tileMap = cachedGameData.tileMap;
            List<Entity> entities = cachedGameData.entities;

            cameraX += CAMERASPEED * camchange_x;
            cameraY += CAMERASPEED * camchange_y;

            if (cameraX < 0) cameraX = 0;
            if (cameraY < 0) cameraY = 0;

            if (cameraX > (TileMap.MAPSIZE - HORIZONTALTILECOUNT) * Tile.TILESIZE) cameraX = (TileMap.MAPSIZE - HORIZONTALTILECOUNT) * Tile.TILESIZE;
            if (cameraY > (TileMap.MAPSIZE - VERTICALTILECOUNT) * Tile.TILESIZE) cameraY = (TileMap.MAPSIZE - VERTICALTILECOUNT) * Tile.TILESIZE;

            int cameraXLeft = cameraX - Tile.TILESIZE;
            int cameraYUp = cameraY - Tile.TILESIZE;

            if (camchange_x != 0 || camchange_y != 0 || force_render_next_frame)
            {

                force_render_next_frame = false;
                //render tiles

                int tileMapLeft = cameraXLeft / Tile.TILESIZE;
                int tileMapTop = cameraYUp / Tile.TILESIZE;

                if (tileMapLeft < 0) tileMapLeft = 0;
                if (tileMapTop < 0) tileMapTop = 0;

                RenderedTiles.Clear();

                for (int j = tileMapTop; j < Math.Min(tileMapTop + VERTICALTILECOUNT + 3, TileMap.MAPSIZE); j++)
                {
                    for (int i = tileMapLeft; i < Math.Min(tileMapLeft + HORIZONTALTILECOUNT + 2, TileMap.MAPSIZE); i++)
                    {
                        Tile t = tileMap[i, j];

                        int tileScaledX = t.I * Tile.TILESIZE;
                        int tileScaledY = t.J * Tile.TILESIZE;

                        //Convert to real coordinates and add to render list
                        int realX = tileScaledX - cameraXLeft - Tile.TILESIZE;
                        int realY = tileScaledY - cameraYUp - Tile.TILESIZE;

                        Brush? b = null;

                        //Get type of tile
                        if (t is PathTile p && p.HasPlaceable())
                        {
                            b = pathBrushes[p.PathType];
                        }
                        else
                        {
                           
                           
                                b = tileBrushes[t.Type];

                           
                        }

                        TileRender tile = new TileRender(realX, realY, b!);

                        RenderedTiles.Add(tile);
                    }
                }

                FinishedRender();
            }
            //render entities

            RenderedEntities.Clear();

            foreach (Entity e in entities)
            {
                if (e.X >= cameraXLeft && e.X <= cameraXLeft + ((HORIZONTALTILECOUNT + 1) * Tile.TILESIZE) && e.Y >= cameraYUp && e.Y <= cameraYUp + ((VERTICALTILECOUNT + 2) * Tile.TILESIZE))
                {
                    RenderedEntities.Add(new EntityRender(e.X - cameraX, e.Y - cameraY, entityBrushes[e.GetType()], e.EntitySize));
                }
            }

            
        }

        private void OnCameraChangeRequest()
        {
            RequestCameraChange?.Invoke(this, (HORIZONTALCAMERACHANGERANGE, VERTICALCAMERACHANGERANGE));
        }

        private void OnRenderTick(object? sender, EventArgs e)
        {
            OnCameraChangeRequest();
            RenderGameArea();
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

