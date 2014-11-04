using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Squirrel;

namespace StandUpTimer
{
    public partial class MainWindow : IBringToForeground
    {
        private readonly StandUpViewModel standUpViewModel;
        private UpdateManager updateManager;

        public MainWindow()
        {
            SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v =>
                    {
                        using (updateManager = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45))
                            updateManager.CreateShortcutForThisExe();
                    },
                    onAppUpdate: v =>
                    {
                        using (updateManager = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45))
                            updateManager.CreateShortcutForThisExe();

                        Close();
                    },
                    onAppUninstall: v =>
                    {
                        using (updateManager = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45))
                            updateManager.RemoveShortcutForThisExe();

                        Close();
                    });

            standUpViewModel = new StandUpViewModel(this);

            DataContext = standUpViewModel;

            this.Left = Properties.Settings.Default.Left;
            this.Top = Properties.Settings.Default.Top;

            InitializeComponent();

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveWindowPosition();
        }

        private void SaveWindowPosition()
        {
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;

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

        private async void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            using (updateManager = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45))
            {
                var updateInfo = await updateManager.CheckForUpdate();

                if (updateInfo.ReleasesToApply.Count > 0)
                    await updateManager.UpdateApp();
            }

            Close();
        }

        public void Now()
        {
            Activate();

            Topmost = true;
            Topmost = false;
        }
    }
}