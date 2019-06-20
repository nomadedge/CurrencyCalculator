using System.Threading.Tasks;

namespace CurrencyCalculator.ViewModels
{
    public interface IPageService
    {
        Task DisplayAlert(string title, string message, string cancel);
    }
}
