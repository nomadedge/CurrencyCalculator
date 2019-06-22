using CurrencyCalculator.Models;
using CurrencyCalculator.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace CurrencyCalculator.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {
        private const int _numbersCount = 9;
        private const int _historySize = 10;
        private const string _placeholder = "100";

        private readonly IPageService _pageService;
        private readonly ICurrencyRateGetter _currencyRateGetter;
        private readonly CurrencyRateService _currencyRateService;
        private string _amount;
        private string _convertedAmount;
        private DateTime _rateDate;
        private string _currencyFrom;
        private string _currencyTo;
        private bool _isRatesLoaded;
        private SQLiteAsyncConnection _historyDb;
        private List<Conversion> _history;
        private bool _isHistoryLoaded;
        private bool _isAllLoaded;
        private bool _isSaved;
        private bool _isPlaceholder;

        public string Amount
        {
            get { return _amount; }
            set
            {
                SetValue(ref _amount, value);
                ConvertedAmount = _currencyRateService.Convert
                    (Amount, CurrencyFrom, CurrencyTo);
            }
        }
        public string ConvertedAmount
        {
            get { return _convertedAmount; }
            set { SetValue(ref _convertedAmount, value); }
        }
        public DateTime RateDate
        {
            get { return _rateDate; }
            set
            {
                SetValue(ref _rateDate, value);
                GetRatesOnDate();
            }
        }
        public List<string> CurrenciesNames { get; private set; }
        public string CurrencyFrom
        {
            get { return _currencyFrom; }
            set
            {
                if (value == CurrencyTo)
                    SetValue(ref _currencyTo, _currencyFrom, "CurrencyTo");

                SetValue(ref _currencyFrom, value);

                if (string.IsNullOrWhiteSpace(Amount) && IsRatesLoaded)
                {
                    Amount = _placeholder;
                    IsPlaceholder = true;
                }

                ConvertedAmount = _currencyRateService.Convert
                    (Amount, CurrencyFrom, CurrencyTo);
            }
        }
        public string CurrencyTo
        {
            get { return _currencyTo; }
            set
            {
                if (value == CurrencyFrom)
                    SetValue(ref _currencyFrom, _currencyTo, "CurrencyFrom");

                SetValue(ref _currencyTo, value);

                if (string.IsNullOrWhiteSpace(Amount) && IsRatesLoaded)
                {
                    Amount = _placeholder;
                    IsPlaceholder = true;
                }

                ConvertedAmount = _currencyRateService.Convert
                    (Amount, CurrencyFrom, CurrencyTo);
            }
        }
        public bool IsRatesLoaded
        {
            get { return _isRatesLoaded; }
            set
            {
                SetValue(ref _isRatesLoaded, value);

                if (value == false)
                    IsAllLoaded = false;
                else if (IsRatesLoaded && IsHistoryLoaded && IsSaved)
                    IsAllLoaded = true;
            }
        }
        public bool IsHistoryLoaded
        {
            get { return _isHistoryLoaded; }
            set
            {
                SetValue(ref _isHistoryLoaded, value);

                if (value == false)
                    IsAllLoaded = false;
                else if (IsRatesLoaded && IsHistoryLoaded && IsSaved)
                    IsAllLoaded = true;

            }
        }
        public bool IsAllLoaded
        {
            get { return _isAllLoaded; }
            set { SetValue(ref _isAllLoaded, value); }
        }
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                SetValue(ref _isSaved, value);

                if (value == false)
                    IsAllLoaded = false;
                else if (IsRatesLoaded && IsHistoryLoaded && IsSaved)
                    IsAllLoaded = true;

            }
        }
        public bool IsPlaceholder
        {
            get { return _isPlaceholder; }
            set
            {
                if (_isPlaceholder != value)
                {
                    _isPlaceholder = value;
                    OnPlaceholderSet();
                }
            }
        }

        public ICommand AddNumberCommand { get; private set; }
        public ICommand AddZeroCommand { get; private set; }
        public ICommand AddDotCommand { get; private set; }
        public ICommand RemoveSymbolCommand { get; private set; }
        public ICommand RemoveAllCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public event EventHandler ConversionSaved;
        public event EventHandler<bool> PlaceholderSet;

        public CalculatorViewModel(IPageService pageService, ICurrencyRateGetter currencyRateGetter)
        {
            IsSaved = true;
            IsRatesLoaded = false;
            IsHistoryLoaded = false;
            IsAllLoaded = false;
            _historyDb = DependencyService.Get<ISQLiteDb>().GetConnection();
            GetHistoryFromDb();
            _currencyRateGetter = currencyRateGetter;
            _currencyRateService = new CurrencyRateService();
            _pageService = pageService;
            Amount = "";
            SetCurrenciesNames();
            IsPlaceholder = true;
            AddNumberCommand = new Command<string>(AddNumber);
            AddZeroCommand = new Command(AddZero);
            AddDotCommand = new Command(AddDot);
            RemoveSymbolCommand = new Command(RemoveSymbol);
            RemoveAllCommand = new Command(RemoveAll);
            SaveCommand = new Command(Save);
        }

        private void SetCurrenciesNames()
        {
            CurrenciesNames = new List<string>
            {
                "Euro",
                "Japanese Yen",
                "Russian ruble",
                "US Dollar"
            };
            CurrencyFrom = CurrenciesNames[0];
            CurrencyTo = CurrenciesNames[1];
        }

        private async void GetHistoryFromDb()
        {
            IsHistoryLoaded = false;
            await _historyDb.CreateTableAsync<Conversion>();
            _history = await _historyDb.Table<Conversion>().ToListAsync();
            IsHistoryLoaded = true;
        }

        public async void GetRatesOnDate()
        {
            IsRatesLoaded = false;
            try
            {
                _currencyRateService.Currencies = await _currencyRateGetter.GetRates(RateDate);
                if (string.IsNullOrWhiteSpace(Amount))
                {
                    Amount = _placeholder;
                    IsPlaceholder = true;
                }
                IsRatesLoaded = true;
                ConvertedAmount = _currencyRateService.Convert
                        (Amount, CurrencyFrom, CurrencyTo);
            }
            catch (Exception)
            {
                await _pageService.DisplayAlert(
                    "No internet connection",
                    "Do you want to try again?",
                    "Try again");

                GetRatesOnDate();
            }
        }

        private void AddNumber(string numberText)
        {
            if (IsPlaceholder)
            {
                Amount = numberText;
                IsPlaceholder = false;
            }
            else if (Amount == "0")
            {
                Amount = numberText;
            }
            else if (Amount.Contains("."))
            {
                var parts = Amount.Split('.');
                if (parts[1].Length < 2)
                    Amount += numberText;
            }
            else
            {
                int numbersCount = Amount.Length;
                if (numbersCount < _numbersCount)
                    Amount += numberText;
            }
        }

        private void AddZero()
        {
            if (Amount.Contains("."))
            {
                var parts = Amount.Split('.');
                if (parts[1].Length < 2)
                    Amount += "0";
            }
            else if (Amount != "0" && !IsPlaceholder)
            {
                int numbersCount = Amount.Length;
                if (numbersCount < _numbersCount)
                    Amount += "0";
            }
            else
            {
                Amount = "0";
                IsPlaceholder = false;
            }
        }

        private void AddDot()
        {
            if (IsPlaceholder)
            {
                Amount = "0.";
                IsPlaceholder = false;
            }
            if (!Amount.Contains("."))
            {
                int numbersCount = Amount.Length;
                if (numbersCount < _numbersCount)
                    Amount += ".";
            }
        }

        private void RemoveSymbol()
        {
            if (!IsPlaceholder)
            {
                var buffer = Amount.Remove(Amount.Length - 1);
                if (string.IsNullOrWhiteSpace(buffer))
                {
                    Amount = _placeholder;
                    IsPlaceholder = true;
                }

                else
                    Amount = buffer;
            }
        }

        private void RemoveAll()
        {
            if (!IsPlaceholder)
            {
                Amount = _placeholder;
                IsPlaceholder = true;
            }
        }

        private async void Save()
        {
            if (Amount == "0")
            {
                await _pageService.DisplayAlert(
                    "Invalid amount",
                    "You can't save conversion with zero amount.",
                    "OK");
                return;
            }

            IsHistoryLoaded = false;

            var newConversion = new Conversion
            {
                RateDate = RateDate,
                CurrencyFrom = CurrencyFrom,
                CurrencyTo = CurrencyTo,
                FromValue = decimal.Parse(Amount),
                ToValue = decimal.Parse(ConvertedAmount),
                ConversionDateTime = DateTime.Now
            };

            if (_history.Count < _historySize)
            {
                await _historyDb.InsertAsync(newConversion);
                _history.Add(newConversion);
            }
            else
            {
                var oldestConversion = _history.Aggregate
                    ((curOld, i) => (curOld == null || i.ConversionDateTime <
                    curOld.ConversionDateTime ? i : curOld));

                await _historyDb.DeleteAsync(oldestConversion);
                _history.Remove(oldestConversion);
                await _historyDb.InsertAsync(newConversion);
                _history.Add(newConversion);
            }

            IsHistoryLoaded = true;

            OnConversionSaved();
        }

        protected virtual void OnConversionSaved()
        {
            ConversionSaved?.Invoke(this, new EventArgs());
        }
        protected virtual void OnPlaceholderSet()
        {
            PlaceholderSet?.Invoke(this, IsPlaceholder);
        }
    }
}
