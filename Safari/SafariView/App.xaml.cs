using SafariModel.Model;
using SafariView.View;
using System.Configuration;
using System.Data;
using System.Windows;

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

            viewModel = new ViewModel.ViewModel(model);
            viewModel.ExitGame += new EventHandler(ViewModel_GameExit);
            viewModel.StartGame += new EventHandler(ViewModel_GameStart);

            lobbyWindow = new LobbyWindow();
            mainWindow = new MainWindow();
            mainWindow.DataContext = lobbyWindow.DataContext = viewModel;

            lobbyWindow.Show();
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
