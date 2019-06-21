using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Models
{
    public interface ICurrencyRateGetter
    {
        Task<List<CurrencyRate>> GetRates(DateTime date);
    }
}
