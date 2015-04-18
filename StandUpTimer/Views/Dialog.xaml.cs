using System.Diagnostics.Contracts;
using System.Windows;
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
    }
}