using System.Threading.Tasks;
using Xamarin.Forms;

namespace CurrencyCalculator.ViewModels
{
    public class PageService : IPageService
    {
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
