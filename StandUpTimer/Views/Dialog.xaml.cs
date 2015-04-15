using StandUpTimer.ViewModels;

namespace StandUpTimer.Views
{
    internal partial class Dialog
    {
        public Dialog(IDialogViewModel dataContext)
        {
            dataContext.RequestClose += (sender, args) =>
            {
                DialogResult = args.DialogResult;
                Close();
            };

            DataContext = dataContext;

            InitializeComponent();
        }
    }
}