using SafariModel.Model;
using SafariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace SafariView.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        #region Private fields
        private ObservableCollection<TileRender> renderedTiles;
        private ObservableCollection<EntityRender> renderedEntities;
        private int money;
        private GameSpeed gameSpeed;
        private int cameraX;
        private int cameraY;
        private DispatcherTimer tickTimer;
        private string? indexPage;
        private string? newGamePage;
        private string? creditsPage;
        private string? loadGamePage;
        private string? optionName;

        private Model model;
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
        #endregion

        #region Properties
        public int Money { get { return money; } set {  money = value; OnPropertyChanged(); } }
        public GameSpeed Gamespeed { get { return gameSpeed; } set { gameSpeed = value; OnPropertyChanged(); } }
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
        public DelegateCommand ClickedCanvas;
        public DelegateCommand ClickedShopIcon;
        public DelegateCommand ChangedGameSpeed;
        #endregion

        #region EventHandlers
        public event EventHandler? ExitGame;
        public event EventHandler? StartGame;
        #endregion

        #region Constructor
        public ViewModel(Model model)
        {
            this.model = model;
            renderedEntities = new ObservableCollection<EntityRender>();
            renderedTiles = new ObservableCollection<TileRender>();
            tickTimer = new DispatcherTimer();

            //Initialize commands
            SaveGameCommand = new DelegateCommand((param) => SaveGame());
            LoadGameCommand = new DelegateCommand((param) => LoadGame());
            ClickedCanvas = new DelegateCommand((param) => ClickPlayArea());
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
        }

        private void Model_NewGameStarted(object? sender, EventArgs e)
        {
            //update renderers
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

        private void ClickPlayArea()
        {
            throw new NotImplementedException();
        }

        private void ClickShop(object? clickedObject) 
        {
            throw new NotImplementedException();
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
            model.NewGame();
            StartGame?.Invoke(this, EventArgs.Empty);

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
            throw new NotImplementedException();
        }

        private void Model_GameOver(object? sender, bool playerWin)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

