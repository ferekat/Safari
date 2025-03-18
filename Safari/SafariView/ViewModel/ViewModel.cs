using SafariModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SafariView.ViewModel
{
    public class ViewModel
    {
        private Model _model = null!;

        public DelegateCommand ExitGameCommand { get; private set; }
        public event EventHandler? ExitGame;

        public ViewModel(Model model)
        {
            _model = model;

            ExitGameCommand = new DelegateCommand(param => OnGameExit());
        }

        private void OnGameExit()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
