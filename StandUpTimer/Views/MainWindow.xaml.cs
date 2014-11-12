using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shell;
using StandUpTimer.Models;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Views
{
    public partial class MainWindow : IBringToForeground
    {
        private readonly Updater updater;
        private readonly StandUpViewModel standUpViewModel;

        public MainWindow()
        {
            updater = new Updater(Close);
            standUpViewModel = new StandUpViewModel(new StandUpModel(new DispatcherTimerWrapper()), this);

            DataContext = standUpViewModel;

            Left = Properties.Settings.Default.Left;
            Top = Properties.Settings.Default.Top;

            InitializeComponent();

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveWindowPosition();
        }

        private void SaveWindowPosition()
        {
            Properties.Settings.Default.Left = Left;
            Properties.Settings.Default.Top = Top;

            Properties.Settings.Default.Save();
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            standUpViewModel.ExitButtonVisibility = Visibility.Visible;
        }

        private void MainWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            standUpViewModel.ExitButtonVisibility = Visibility.Hidden;
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        public void Now()
        {
            Activate();

            Topmost = true;
            Topmost = false;
        }

        private void CreativeCommons_OnMouseMove(object sender, MouseEventArgs e)
        {
            standUpViewModel.CreativeCommonsVisibility = Visibility.Visible;
        }

        private void CreativeCommons_OnMouseLeave(object sender, MouseEventArgs e)
        {
            standUpViewModel.CreativeCommonsVisibility = Visibility.Hidden;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PrintScreen)
                File.WriteAllBytes("screenshot.png", this.GetJpgImage(1.0, 80));
        }
    }
}