using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyCalculator.Models
{
    public class CurrencyRateService
    {
        public List<CurrencyRate> Currencies { get; set; }

        public CurrencyRateService()
        {
            Currencies = new List<CurrencyRate>();
        }

        public string Convert(string amount, string currencyFrom, string currencyTo)
        {
            if (string.IsNullOrWhiteSpace(amount))
                return "";

            if (amount == "0")
                return "0";

            if (string.IsNullOrWhiteSpace(currencyFrom))
                throw new ArgumentNullException("currencyFrom");
            if (string.IsNullOrWhiteSpace(currencyTo))
                throw new ArgumentNullException("currencyTo");

            var amountM = decimal.Parse(amount);
            var fromRate = GetRate(currencyFrom);
            var toRate = GetRate(currencyTo);
            var convertedAmountM = amountM * fromRate / toRate;
            return (convertedAmountM.ToString("0.00"));
        }

        private decimal GetRate(string currencyName)
        {
            return Currencies.Single(c => c.Name == currencyName).Rate;
        }
    }
}
