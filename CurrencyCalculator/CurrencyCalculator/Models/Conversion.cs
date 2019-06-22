using SQLite;
using System;

namespace CurrencyCalculator.Models
{
    public class Conversion
    {
        [PrimaryKey]
        public DateTime ConversionDateTime { get; set; }
        public DateTime RateDate { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
    }
}
