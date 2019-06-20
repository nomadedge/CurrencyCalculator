using CurrencyCalculator.Models;
using CurrencyCalculator.Persistence;
using SQLite;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace CurrencyCalculator.ViewModels
{
    class HistoryViewModel : BaseViewModel
    {
        private SQLiteAsyncConnection _historyDb;
        private ObservableCollection<Conversion> _history;

        public ObservableCollection<Conversion> History
        {
            get { return _history; }
            set { SetValue(ref _history, value); }
        }

        public HistoryViewModel()
        {
            _historyDb = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        public async void GetHistoryFromDb()
        {
            await _historyDb.CreateTableAsync<Conversion>();
            var history = await _historyDb.Table<Conversion>().ToListAsync();
            history = history.OrderBy(x => x.ConversionDateTime).Reverse().ToList();
            History = new ObservableCollection<Conversion>(history);
        }
    }
}
