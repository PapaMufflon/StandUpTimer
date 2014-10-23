using System;
using System.Windows.Input;

namespace StandUpTimer
{
    internal class SkipCommand : ICommand
    {
        private readonly ICanSkip skip;
        private readonly ICommand okCommand;

        public SkipCommand(ICanSkip skip, ICommand okCommand)
        {
            this.skip = skip;
            this.okCommand = okCommand;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            skip.Skip();
            okCommand.Execute(null);
        }

        public event EventHandler CanExecuteChanged;
    }
}