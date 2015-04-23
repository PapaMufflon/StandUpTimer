using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using System.Windows.Input;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Views
{
    internal partial class Dialog
    {
        public Dialog(IDialogViewModel dataContext)
        {
            Contract.Requires(dataContext != null);

            WeakEventManager<IDialogViewModel, RequestCloseEventArgs>.AddHandler(dataContext, "RequestClose", OnRequestClose);

            DataContext = dataContext;

            InitializeComponent();
        }

        private void OnRequestClose(object sender, RequestCloseEventArgs e)
        {
            DialogResult = e.DialogResult;
            Close();

            WeakEventManager<IDialogViewModel, RequestCloseEventArgs>.RemoveHandler(DataContext as IDialogViewModel, "RequestClose", OnRequestClose);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PrintScreen)
                File.WriteAllBytes("screenshot.png", this.GetJpgImage(1.0));
        }
    }
}