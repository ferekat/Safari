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
        public string IndexPage { get; private set; }
        public string NewGamePage { get; private set; }
        public string CreditsPage { get; private set; }
        public string LoadGamePage { get; private set; }
        public string OptionName { get; private set; }
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
        public DelegateCommand CreditsCommand { get; private set; }
        public DelegateCommand ClickedCanvas;
        public DelegateCommand ClickedShopIcon;
        public DelegateCommand ChangedGameSpeed;
        #endregion

        #region EventHandlers
        public event EventHandler? ExitGame;
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
            NewGamePageCommand = new DelegateCommand((param) => OnNewGame());
            LoadGamePageCommand = new DelegateCommand((param) => OnLoadPageClicked());
            BackCommand = new DelegateCommand((param) => OnBackClicked());
            CreditsCommand = new DelegateCommand((param) => OnCreditsClicked());

            //Subscribe to model's events
            model.TickPassed += new EventHandler<GameData>(Model_TickPassed);
            model.GameOver += new EventHandler<bool>(Model_GameOver);

            //Set window bindings
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            LoadGamePage = "Hidden";
            CreditsPage = "Hidden";
            OptionName = "SAFARI";
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

        private void OnNewGame()
        {
            IndexPage = "Hidden";
            NewGamePage = "Visible";
            OptionName = "New Game";
            OnPropertyChanged(nameof(IndexPage));
            OnPropertyChanged(nameof(NewGamePage));
            OnPropertyChanged(nameof(OptionName));
        }
        private void OnBackClicked()
        {
            IndexPage = "Visible";
            NewGamePage = "Hidden";
            CreditsPage = "Hidden";
            LoadGamePage = "Hidden";
            OptionName = "SAFARI";
            OnPropertyChanged(nameof(IndexPage));
            OnPropertyChanged(nameof(NewGamePage));
            OnPropertyChanged(nameof(CreditsPage));
            OnPropertyChanged(nameof(OptionName));
            OnPropertyChanged(nameof(LoadGamePage));
        }
        private void OnCreditsClicked()
        {
            IndexPage = "Hidden";
            CreditsPage = "Visible";
            OptionName = "Credits";
            OnPropertyChanged(nameof(IndexPage));
            OnPropertyChanged(nameof(CreditsPage));
            OnPropertyChanged(nameof(OptionName));
        }
        private void OnLoadPageClicked()
        {
            IndexPage = "Hidden";
            LoadGamePage = "Visible";
            OptionName = "Load Game";
            OnPropertyChanged(nameof(IndexPage));
            OnPropertyChanged(nameof(LoadGamePage));
            OnPropertyChanged(nameof(OptionName));
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

