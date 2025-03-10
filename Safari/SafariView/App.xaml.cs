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
        private MainWindow window;
        private ViewModel.ViewModel viewModel;
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
            viewModel = new ViewModel.ViewModel();

            window = new MainWindow();
            window.DataContext = viewModel;

            window.Show();
        }
        #endregion
    }

}
