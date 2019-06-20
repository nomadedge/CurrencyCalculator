using CurrencyCalculator.Persistence;
using CurrencyCalculator.UWP.Persistence;
using SQLite;
using System.IO;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDb))]

namespace CurrencyCalculator.UWP.Persistence
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = ApplicationData.Current.LocalFolder.Path;
            var path = Path.Combine(documentsPath, "LastOperations.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}