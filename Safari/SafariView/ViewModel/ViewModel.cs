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
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.CodeDom;
using SafariModel.Model.InstanceEntity;
using System.Diagnostics.Eventing.Reader;
using System.Data.SqlTypes;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Diagnostics.Contracts;
using SafariModel.Model.EventArgsClasses;

namespace SafariView.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        #region Private fields
        private List<TileRender> RenderedTiles;
        public ObservableCollection<EntityRender> RenderedEntities { get; private set; }
        public ObservableCollection<FloatingText> FloatingTexts { get; private set; }
        private int money;
        private int hour;
        private int day;
        private int week;
        private int month;
        private GameSpeed gameSpeed;
        private int cameraX;
        private int cameraY;

        private Guard? selectedGuard;
        private (int, int) selectedTile;
        private int selectedEntityID;
        private ClickAction cAction;
        private string selectedShopName;
        private GameData? cachedGameData;
        //Mennyi tile lesz látható a képernyőn
        private int HorizontalTileCount;
        private int VerticalTileCount;
        private int HorizontalCameraAdjustment;
        private int VerticalCameraAdjustment;

        private readonly int HORIZONTALCAMERACHANGERANGE = 150;
        private readonly int VERTICALCAMERACHANGERANGE = 150;
        private readonly int CAMERASPEED = 10;
        private bool force_render_next_frame;
        private bool redrawMinimap;

        private int camchange_x = 0;
        private int camchange_y = 0;

        private Thickness minimapPosition;
        private WriteableBitmap minimapBitmap;

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
            {TileType.WATER, new SolidColorBrush(Color.FromRgb(55, 55, 255))},
            { TileType.GROUND, new SolidColorBrush(Color.FromRgb(153, 76, 0))},
            { TileType.EMPTY,new SolidColorBrush(Color.FromRgb(0, 0, 0))},
            { TileType.FENCE,new SolidColorBrush(Color.FromRgb(30, 30, 30))},
           // { TileType.HILL,new SolidColorBrush(Color.FromRgb(0, 102, 0))},
            { TileType.ENTRANCE,new SolidColorBrush(Color.FromRgb(255, 0, 0))},
            { TileType.EXIT,new SolidColorBrush(Color.FromRgb(0, 255, 0))}
        };

        private static Dictionary<TileType, byte[]> minimaptileBrushes = new Dictionary<TileType, byte[]>()
        {
            {TileType.WATER, new byte[] {55,55,255} },
            { TileType.GROUND, new byte[] {153,76,0}},
            { TileType.EMPTY,new byte[] {0,0,0}},
            { TileType.FENCE,new byte[] {30,30,30}},
           // { TileType.HILL,new SolidColorBrush(Color.FromRgb(0, 102, 0))},
            { TileType.ENTRANCE,new byte[] {255,0,0}},
            { TileType.EXIT,new byte[] {0,255,0}}
        };

        private static Dictionary<TilePlaceable, byte[]> minimapConditionBrushes = new Dictionary<TilePlaceable, byte[]>()
        {
            {TilePlaceable.EMPTY,new byte[] {0,0,0} },
            {TilePlaceable.IS_ROAD,new byte[] {235,125,52}},
            {TilePlaceable.IS_LARGE_BRIDGE,new byte[] {125,37,37} },
            {TilePlaceable.IS_SMALL_BRIDGE,new byte[] {140,136,136}}
        };

        private static Dictionary<TilePlaceable, Brush> conditionBrushes = new Dictionary<TilePlaceable, Brush>()
        {
            {TilePlaceable.EMPTY,new SolidColorBrush(Color.FromRgb(0,0,0)) },
            {TilePlaceable.IS_ROAD,new SolidColorBrush(Color.FromRgb(235, 125, 52) )},
            {TilePlaceable.IS_LARGE_BRIDGE,new SolidColorBrush(Color.FromRgb(125, 37, 37)) },
            {TilePlaceable.IS_SMALL_BRIDGE,new SolidColorBrush(Color.FromRgb(140, 136, 136) )}
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
            {typeof(Hunter),new SolidColorBrush(Color.FromRgb(150,150,150)) },
        };
        private static Brush HillBrush(Tile hill)
        {
            return new SolidColorBrush(Color.FromRgb(0, (byte)(102 + hill.Z), 0));
        }

        private static byte[] HillBrushMinimap(Tile hill)
        {
            return new byte[] { 0, (byte)(102 + hill.Z), 0 };
        }

        #endregion

        #region ClickAction enum
        public enum ClickAction
        {
            NOTHING,
            BUY,
            SELL,
            SELECT,
            TARGET
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

        public int MINIMAPSIZE { get { return 300; } }
        public int MINIMAPBORDERTHICKNESS { get { return 20; } }
        public double PlayerMarkerWidth { get { return (((MINIMAPSIZE - (2 * MINIMAPBORDERTHICKNESS)) / (double)Model.MAPSIZE) * HorizontalTileCount); } }
        public double PlayerMarkerHeight { get { return (((MINIMAPSIZE - (2 * MINIMAPBORDERTHICKNESS)) / (double)Model.MAPSIZE) * VerticalTileCount); } }
        public Thickness MinimapPosition { get { return minimapPosition; } private set { minimapPosition.Left = value.Left; minimapPosition.Top = value.Top; OnPropertyChanged(); } }
        public WriteableBitmap MinimapBitmap { get { return minimapBitmap; } private set { OnPropertyChanged(); } }



        public ClickAction CAction { get { return cAction; } private set { cAction = value; OnPropertyChanged(); } }

        public string MoneyString { get { return moneyString; } private set { moneyString = value; OnPropertyChanged(); } }

        public string SelectedShopName { get { return selectedShopName; } private set { selectedShopName = value; OnPropertyChanged(); } }

        #endregion

        #region Properties
        public float Mid { get { return mid; } set { mid = value; OnPropertyChanged(); } }
        public int Money { get { return money; } private set { money = value; MoneyString = $"Money : {money}$"; } }
        public string? Hour
        {
            get { return hour.ToString("D2"); }
            set
            {
                if (int.TryParse(value, out int parsedHour))
                {
                    hour = parsedHour;
                    OnPropertyChanged();
                }
            }
        }
        public string? Day
        {
            get { return day.ToString("D2"); }
            set
            {
                if (int.TryParse(value, out int parsedDay))
                {
                    day = parsedDay;
                    OnPropertyChanged();
                }
            }
        }
        public string? Week
        {
            get { return week.ToString("D2"); }
            set
            {
                if (int.TryParse(value, out int parsedWeek))
                {
                    week = parsedWeek;
                    OnPropertyChanged();
                }
            }
        }
        public string? Month
        {
            get { return $"{month}/12"; }
            set
            {
                if (int.TryParse(value!.Split('/')[0], out int parsedMonth))
                {
                    month = parsedMonth;
                    OnPropertyChanged();
                }
            }
        }
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
        public DelegateCommand ChangedGameSpeed { get; private set; }
        #endregion

        #region EventHandlers
        public event EventHandler? ExitGame;
        public event EventHandler? StartGame;
        public event EventHandler? FinishedRenderingTileMap;
        public event EventHandler<(int, int)>? RequestCameraChange;
        #endregion

        #region Constructor
        public ViewModel(Model model, List<TileRender> renderedTiles)
        {
            this.model = model;
            RenderedEntities = new ObservableCollection<EntityRender>();
            FloatingTexts = new ObservableCollection<FloatingText>();
            this.RenderedTiles = renderedTiles;
            minimapBitmap = new WriteableBitmap(Model.MAPSIZE, Model.MAPSIZE,96,96, PixelFormats.Rgb24, null);
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
            model.TileMapUpdated += new EventHandler<(int,int)>(Model_TileMapUpdated);
            model.NewMessage += OnMessage;

            //Set window bindings
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            LoadGamePage = "Hidden";
            CreditsPage = "Hidden";
            OptionName = "SAFARI";
            CAction = ClickAction.NOTHING;
            Gamespeed = GameSpeed.Slow;
            Hour = "0";
            Day = "1";
            Week = "1";
            Month = "0/12";

            TopRowHeightRelative = 0.08F;
            BottomRowHeightRelative = 0.15F;
            Mid = 1 - TopRowHeightRelative - BottomRowHeightRelative;
            HorizontalTileCount = 1;
            VerticalTileCount = 1;
            HorizontalCameraAdjustment = 0;
            VerticalCameraAdjustment = 0;

            selectedTile = (-1, -1);
            selectedEntityID = -1;

            force_render_next_frame = true;
            redrawMinimap = true;
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
            if (speedValue is string speed)
            {
                switch (speed)
                {
                    case "Slow":
                        Gamespeed = GameSpeed.Slow;
                        model.GameSpeed = GameSpeed.Slow;
                        break;
                    case "Medium":
                        Gamespeed = GameSpeed.Medium;
                        model.GameSpeed = GameSpeed.Medium;
                        break;
                    case "Fast":
                        Gamespeed = GameSpeed.Fast;
                        model.GameSpeed = GameSpeed.Fast;
                        break;
                }
            }
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
            if(Money != data.money)
            {
                Money = data.money;
            }
            if(hour != data.hour)
            {
                Hour = data.hour.ToString();
            }
            if (day != data.day)
            {
                Day = data.day.ToString();
            }
            if (week != data.week)
            {
                Week = data.week.ToString();
            }
            if (Month != $"{data.month}/12")
            {
                Month = $"{data.month}/12";
            }
        }

        private void Model_GameOver(object? sender, bool playerWin)
        {
            throw new NotImplementedException();
        }

        private void Model_TileMapUpdated(object? sender, (int,int) updatedTile)
        {
            if (cachedGameData == null) return;
            force_render_next_frame = true;
            UpdateMinimap(updatedTile.Item1, updatedTile.Item2, cachedGameData!.tileMap);
        }

        private void Model_NewGameStarted(object? sender, EventArgs e)
        {
            CreditsPage = "Hidden";
            LoadGamePage = "Hidden";
            OptionName = "SAFARI";
            StartGame?.Invoke(this, EventArgs.Empty);

            redrawMinimap = true;
        }
        #endregion

        #region Click handlers
        public void ClickPlayArea(object? sender, Point p)
        {
            int gameX = cameraX + (int)p.X;
            int gameY = cameraY + (int)p.Y;

            if (CAction == ClickAction.NOTHING)
            {
                selectedTile = model.GetTileFromCoords(gameX, gameY);
                selectedEntityID = model.GetEntityIDFromCoords(gameX, gameY);
                Entity? e = model.GetEntityByID(selectedEntityID);
                if (e is Guard g)
                {
                    selectedGuard = g;
                    CAction = ClickAction.TARGET;
                }
            }

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
            if (CAction == ClickAction.TARGET)
            {
                selectedEntityID = model.GetEntityIDFromCoords(gameX, gameY);
                if (selectedEntityID != -1)
                {
                    if (model.GetEntityByID(selectedEntityID) is Carnivore c)
                    {
                        selectedGuard!.TargetAnimal = c;
                        CAction = ClickAction.NOTHING;
                        selectedGuard = null;
                    }
                }
            }
        }

        public void ClickMinimap(object? sender, Point p)
        {
            int canvasSize = MINIMAPSIZE - 2 * MINIMAPBORDERTHICKNESS;
            double xPercent = p.X/ canvasSize;
            double yPercent = p.Y/ canvasSize;

            int mapSizeinPixels = Model.MAPSIZE * Tile.TILESIZE;

            int clickedXPos = (int)(mapSizeinPixels * xPercent);
            int clickedYPos = (int)(mapSizeinPixels * yPercent);

            cameraX = clickedXPos - (HorizontalTileCount / 2) * Tile.TILESIZE;
            cameraY = clickedYPos - (VerticalTileCount / 2) * Tile.TILESIZE;

            force_render_next_frame = true;
        }
        #endregion

        #region Tile canvas resize event handler
        public void TileCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            double sizeX = e.NewSize.Width;
            double sizeY = e.NewSize.Height;

            HorizontalTileCount = (int)(sizeX / Tile.TILESIZE);
            VerticalTileCount = (int)(sizeY / Tile.TILESIZE);

            HorizontalCameraAdjustment = (int)(sizeX - HorizontalTileCount * Tile.TILESIZE);
            VerticalCameraAdjustment = (int)(sizeY - VerticalTileCount * Tile.TILESIZE);

            OnPropertyChanged(nameof(PlayerMarkerWidth));
            OnPropertyChanged(nameof(PlayerMarkerHeight));
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

            if (cameraX > ((Model.MAPSIZE - HorizontalTileCount) * Tile.TILESIZE) - HorizontalCameraAdjustment) 
                cameraX = ((Model.MAPSIZE - HorizontalTileCount) * Tile.TILESIZE) - HorizontalCameraAdjustment;
            if (cameraY > ((Model.MAPSIZE - VerticalTileCount) * Tile.TILESIZE) - VerticalCameraAdjustment) 
                cameraY = ((Model.MAPSIZE - VerticalTileCount) * Tile.TILESIZE) - VerticalCameraAdjustment;

            int cameraXLeft = cameraX - Tile.TILESIZE;
            int cameraYUp = cameraY - Tile.TILESIZE;

            UpdateMinimapMarker(cameraX, cameraY);
            if (redrawMinimap) ReDrawMinimap(tileMap);

            if (camchange_x != 0 || camchange_y != 0 || force_render_next_frame)
            {

                force_render_next_frame = false;
                //render tiles

                int tileMapLeft = cameraXLeft / Tile.TILESIZE;
                int tileMapTop = cameraYUp / Tile.TILESIZE;

                if (tileMapLeft < 0) tileMapLeft = 0;
                if (tileMapTop < 0) tileMapTop = 0;

                RenderedTiles.Clear();

                for (int j = tileMapTop; j < Math.Min(tileMapTop + VerticalTileCount + 3, Model.MAPSIZE); j++)
                {
                    for (int i = tileMapLeft; i < Math.Min(tileMapLeft + HorizontalTileCount + 3, Model.MAPSIZE); i++)
                    {
                        Tile t = tileMap[i, j];

                        int tileScaledX = t.I * Tile.TILESIZE;
                        int tileScaledY = t.J * Tile.TILESIZE;

                        //Convert to real coordinates and add to render list
                        int realX = tileScaledX - cameraXLeft - Tile.TILESIZE;
                        int realY = tileScaledY - cameraYUp - Tile.TILESIZE;

                        Brush? b = null;

                        //Get type of tile
                        if (t.HasPlaceable())
                        {
                            b = conditionBrushes[t.Placeable];
                        }
                        else
                        {
                            if (t.Type == TileType.HILL)
                            {
                                b = HillBrush(t);
                            }
                            else
                            {
                                b = tileBrushes[t.Type];

                            }
                        }

                        TileRender tile = new TileRender(realX, realY,Tile.TILESIZE, b!);

                        RenderedTiles.Add(tile);
                    }
                }

                FinishedTileMapRender();
            }
            //render entities

            RenderedEntities.Clear();

            foreach (Entity e in entities)
            {
                if (e.X >= cameraXLeft && e.X <= cameraXLeft + ((HorizontalTileCount + 1) * Tile.TILESIZE) && e.Y >= cameraYUp && e.Y <= cameraYUp + ((VerticalTileCount + 2) * Tile.TILESIZE))
                {
                    RenderedEntities.Add(new EntityRender(e.X - cameraX, e.Y - cameraY, entityBrushes[e.GetType()], e.EntitySize));
                }
            }

            
        }

        private void ReDrawMinimap(Tile[,] tileMap)
        {
            redrawMinimap = false;

            for(int i = 0; i < Model.MAPSIZE; i++)
            {
                for(int j = 0; j < Model.MAPSIZE; j++)
                {

                    Tile t = tileMap[i, j];

                    byte[]? b = null;

                    //Get type of tile
                    if (t.HasPlaceable())
                    {
                        b = minimapConditionBrushes[t.Placeable];
                    }
                    else
                    {
                        
                        if (t.Type == TileType.HILL)
                        {
                            b = HillBrushMinimap(t);
                        }
                        else
                        {
                        
                        b = minimaptileBrushes[t.Type];
                        }
                    }
                    Int32Rect rect = new Int32Rect(i, j, 1, 1);
                    minimapBitmap.WritePixels(rect, b, 3, 0);
                }

                MinimapBitmap = minimapBitmap;
            }
        }

        private void UpdateMinimap(int tileX, int tileY, Tile[,] tileMap)
        {
            Tile t = tileMap[tileX, tileY];

            byte[]? b = null;

            //Get type of tile
            if (t.HasPlaceable())
            {
                b = minimapConditionBrushes[t.Placeable];
            }
            else
            {

                if (t.Type == TileType.HILL)
                {
                    b = HillBrushMinimap(t);
                }
                else
                {

                    b = minimaptileBrushes[t.Type];
                }
            }
            Int32Rect rect = new Int32Rect(tileX, tileY, 1, 1);
            minimapBitmap.WritePixels(rect, b, 3, 0);
            MinimapBitmap = minimapBitmap;
        }

        private void UpdateMinimapMarker(int camX, int camY)
        {
            double mapSizeinPixels = Model.MAPSIZE * Tile.TILESIZE;
            
            double xPercent = camX / mapSizeinPixels;
            double yPercent = camY / mapSizeinPixels;

            MinimapPosition = new Thickness(xPercent * (MINIMAPSIZE-(2*MINIMAPBORDERTHICKNESS)), yPercent * (MINIMAPSIZE-(2 * MINIMAPBORDERTHICKNESS)), 0,0);
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

        private void FinishedTileMapRender()
        {
            FinishedRenderingTileMap?.Invoke(this, EventArgs.Empty);
        }

        private void OnMessage(object? sender, EventArgs e)
        {
            if (e is MessageEventArgs messageEvent)
            {
                var screenX = messageEvent.X - cameraX;
                var screenY = messageEvent.Y - cameraY;
                var text = new FloatingText($"{messageEvent.Message}", screenX, screenY);
                FloatingTexts.Add(text);

                Task.Run(async () =>
                {
                    await Task.Delay(600);
                    App.Current.Dispatcher.Invoke(() => FloatingTexts.Remove(text));
                });
            }
        }
        #endregion
    }
}

