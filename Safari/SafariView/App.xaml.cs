using SafariModel.Model;
using SafariView.View;
using SafariView.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace SafariView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Private fields
        private LobbyWindow? lobbyWindow;
        private MainWindow? mainWindow;
        private ViewModel.ViewModel? viewModel;
        private Model? model;
        #endregion

        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(AppStartUp);
        }
        #endregion

        #region Startup method
        public void AppStartUp(object? sender, StartupEventArgs e)
        {
            model = new Model();
            List<TileRender> renderedTiles = new List<TileRender>();

            viewModel = new ViewModel.ViewModel(model,renderedTiles);
            viewModel.ExitGame += new EventHandler(ViewModel_GameExit);
            viewModel.StartGame += new EventHandler(ViewModel_GameStart);
            viewModel.FinishedRendering += new EventHandler(ViewModel_FinishedRendering);
            

            lobbyWindow = new LobbyWindow();
            
            mainWindow = new MainWindow(renderedTiles);
            mainWindow.DataContext = lobbyWindow.DataContext = viewModel;
            mainWindow.CanvasClick += new EventHandler<Point>(viewModel.ClickPlayArea);

            viewModel.RequestCameraChange += new EventHandler<(int, int)>(mainWindow.ViewModel_CameraChangeRequest);
            mainWindow.CameraChange += new EventHandler<(int, int)>(viewModel.MainWindow_CameraMovement);
            

            lobbyWindow.Show();
        }

        private void ViewModel_FinishedRendering(object? sender, EventArgs e)
        {
            mainWindow!.ShowRender();
        }

        private void ViewModel_GameExit(object? sender, System.EventArgs e)
        {
            Shutdown();
        }
        private void ViewModel_GameStart(object? sender, System.EventArgs e)
        {
            lobbyWindow!.Close();
            mainWindow!.Show();
        }
        #endregion
    }

}
