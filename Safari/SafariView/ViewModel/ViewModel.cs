using SafariModel.Model;
using SafariModel.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region Properties
        public int Money { get { return money; } set {  money = value; OnPropertyChanged(); } }
        public GameSpeed Gamespeed { get { return gameSpeed; } set { gameSpeed = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public DelegateCommand SaveGameCommand;
        public DelegateCommand LoadGameCommand;
        public DelegateCommand ClickedCanvas;
        public DelegateCommand ClickedShopIcon;
        public DelegateCommand ChangedGameSpeed;
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

            //Subscribe to model's events
            model.TickPassed += new EventHandler<GameData>(Model_TickPassed);
            model.GameOver += new EventHandler<bool>(Model_GameOver);
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

