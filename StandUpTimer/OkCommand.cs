using System;
using System.Windows;
using System.Windows.Input;

namespace StandUpTimer
{
    public class OkCommand : ICommand
    {
        private readonly StandUpViewModel standUpViewModel;

        public OkCommand(StandUpViewModel standUpViewModel)
        {
            this.standUpViewModel = standUpViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            standUpViewModel.Shake = false;
            standUpViewModel.OkButtonVisibility = Visibility.Collapsed;
            standUpViewModel.StartTimer();
        }

        public event EventHandler CanExecuteChanged;
    }
}