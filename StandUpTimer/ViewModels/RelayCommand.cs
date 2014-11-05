using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace StandUpTimer.ViewModels
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;        

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Contract.Requires(execute != null);

            this.execute = execute;
            this.canExecute = canExecute;           
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}