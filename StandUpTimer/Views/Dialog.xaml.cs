using System.Windows;
using StandUpTimer.ViewModels;

namespace StandUpTimer.Views
{
    internal partial class Dialog
    {
        public Dialog(IDialogViewModel dataContext)
        {
            WeakEventManager<IDialogViewModel, RequestCloseEventArgs>.AddHandler(dataContext, "RequestClose",
                (sender, args) =>
                {
                    DialogResult = args.DialogResult;
                    Close();
                });

            DataContext = dataContext;

            InitializeComponent();
        }
    }
}