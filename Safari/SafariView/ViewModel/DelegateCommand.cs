using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafariView.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private Action<object?> execute;
        private Predicate<object?>? canExecute;

        public DelegateCommand(Action<object>? action, Predicate<object>? predicate = null)
        {
            if (action == null) throw new ArgumentNullException("Action cannot be null");

            execute = action!;
            canExecute = predicate!;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) throw new InvalidOperationException("Unable to execute");
            execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
