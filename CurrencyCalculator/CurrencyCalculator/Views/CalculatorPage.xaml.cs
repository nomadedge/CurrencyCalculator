using CurrencyCalculator.Models;
using CurrencyCalculator.ViewModels;
using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CurrencyCalculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculatorPage : ContentPage
    {
        private const int _changedImageTime = 1000;
        private readonly Color _placeholderColor = Color.Gray;

        private string _saveButtonImage;
        private Color _amountsColor;

        public string SaveButtonImage
        {
            get { return _saveButtonImage; }
            set
            {
                if (_saveButtonImage != value)
                {
                    _saveButtonImage = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color AmountsColor
        {
            get { return _amountsColor; }
            set
            {
                if (_amountsColor != value)
                {
                    _amountsColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private CalculatorViewModel ViewModel
        {
            get { return BindingContext as CalculatorViewModel; }
            set { BindingContext = value; }
        }

        public CalculatorPage()
        {
            InitializeComponent();
            AmountsColor = _placeholderColor;

            ViewModel = new CalculatorViewModel(new PageService(), new CBRCurrencyRateGetter());
            ViewModel.ConversionSaved += OnConversionSaved;
            ViewModel.PlaceholderSet += OnPlaceholderSet;

            SaveButtonImage = GetSaveButtonImageDefault();
        }

        private static string GetSaveButtonImageDefault()
        {
            string saveButtonImage;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    saveButtonImage = "Resources/Images/save_50px.png";
                    break;
                default:
                    saveButtonImage = "save.png";
                    break;
            }

            return saveButtonImage;
        }

        private static string GetSaveButtonImageAfterSave()
        {
            string saveButtonImage;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    saveButtonImage = "Resources/Images/checked_2_50px.png";
                    break;
                default:
                    saveButtonImage = "checked_2.png";
                    break;
            }

            return saveButtonImage;
        }

        public void OnConversionSaved(object source, EventArgs e)
        {
            var changedImage = new Timer();

            ViewModel.IsSaved = false;
            SaveButtonImage = GetSaveButtonImageAfterSave();
            changedImage.Elapsed += (object sender, ElapsedEventArgs elapsedEventArgs) =>
                {
                    ViewModel.IsSaved = true;
                    SaveButtonImage = GetSaveButtonImageDefault();
                };
            changedImage.Interval = _changedImageTime;
            changedImage.AutoReset = false;
            changedImage.Start();

            MessagingCenter.Send(this, "saved");
        }

        public void OnPlaceholderSet(object source, bool isPlaceholder)
        {
            if (isPlaceholder)
                AmountsColor = _placeholderColor;
            else
                AmountsColor = Color.Black;
        }
    }
}