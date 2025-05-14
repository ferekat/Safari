using SafariModel.Model;
using SafariModel.Persistence;
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
            model = new Model(new FileDataAccess());
            List<RenderObject> renderedTiles = new List<RenderObject>();
            List<RenderObject> renderedEntities = new List<RenderObject>();

            viewModel = new ViewModel.ViewModel(model,renderedTiles,renderedEntities);
            viewModel.ExitGame += new EventHandler(ViewModel_GameExit);
            viewModel.StartGame += new EventHandler(ViewModel_GameStart);
            viewModel.EndGame += new EventHandler(ViewModel_GameEnd);
            viewModel.FinishedRenderingTileMap += new EventHandler(ViewModel_FinishedTileMapRendering);
            viewModel.FinishedRenderingEntities += new EventHandler(ViewModel_FinishedEntityRendering);


            
            mainWindow = new MainWindow(renderedTiles,renderedEntities);
            mainWindow.DataContext = lobbyWindow.DataContext = viewModel;
            mainWindow.TileCanvasClick += new EventHandler<Point>(viewModel.ClickPlayArea);
            mainWindow.MinimapCanvasClick += new EventHandler<Point>(viewModel.ClickMinimap);

            viewModel.RequestCameraChange += new EventHandler<(int, int)>(mainWindow.ViewModel_CameraChangeRequest);
            mainWindow.CameraChange += new EventHandler<(int, int)>(viewModel.MainWindow_CameraMovement);

            mainWindow.tileCanvas.SizeChanged += new SizeChangedEventHandler(viewModel.TileCanvas_SizeChanged);

            lobbyWindow.Show();
        }

        private void ViewModel_FinishedTileMapRendering(object? sender, EventArgs e)
        {
            mainWindow!.ShowTileMapRender();
        }

        private void ViewModel_FinishedEntityRendering(object? sender, EventArgs e)
        {
            mainWindow!.ShowEntityRender();
        }

        private void ViewModel_GameExit(object? sender, System.EventArgs e)
        {
            Shutdown();
        }
        private void ViewModel_GameStart(object? sender, System.EventArgs e)
        {
            lobbyWindow!.Hide();
            mainWindow!.Show();
        }

        private void ViewModel_GameEnd(object? sender, System.EventArgs e)
        {
            lobbyWindow!.Show();
            mainWindow!.Hide();
        }
        #endregion
    }

}
