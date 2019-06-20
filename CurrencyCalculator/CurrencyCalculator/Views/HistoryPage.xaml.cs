using CurrencyCalculator.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CurrencyCalculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {

        private HistoryViewModel ViewModel
        {
            get { return BindingContext as HistoryViewModel; }
            set { BindingContext = value; }
        }

        public HistoryPage()
        {
            InitializeComponent();

            ViewModel = new HistoryViewModel();
            ViewModel.GetHistoryFromDb();

            MessagingCenter.Subscribe<CalculatorPage>(this, "saved", (sender) =>
            {
                ViewModel.GetHistoryFromDb();
            });
        }

        private void History_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}