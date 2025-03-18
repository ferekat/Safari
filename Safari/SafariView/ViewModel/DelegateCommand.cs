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
        private readonly Action<Object?> _execute;
        private readonly Predicate<Object?>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public DelegateCommand(Action<Object?> execute) : this(null, execute) { }
        public DelegateCommand(Predicate<Object?>? canExecute, Action<Object?> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = execute;
            _canExecute = canExecute;
        }
        public Boolean CanExecute(Object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public void Execute(Object? parameter)
        {
            _execute(parameter);
        }

    }
}
