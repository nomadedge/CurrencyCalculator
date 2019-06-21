using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Models
{
    class TestCurrencyRateGetter : ICurrencyRateGetter
    {
        public async Task<List<CurrencyRate>> GetRates(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException("date");

            var rates = new List<CurrencyRate>();
            rates.Add(new CurrencyRate("Russian ruble", 1m));
            rates.Add(new CurrencyRate("US Dollar", 60m));
            rates.Add(new CurrencyRate("Euro", 70m));
            rates.Add(new CurrencyRate("Japanese Yen", 0.5m));

            return rates;
        }
    }
}
