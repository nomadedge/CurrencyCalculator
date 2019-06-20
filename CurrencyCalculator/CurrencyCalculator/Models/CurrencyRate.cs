namespace CurrencyCalculator.Models
{
    public class CurrencyRate
    {
        public string Name { get; private set; }
        public decimal Rate { get; private set; }

        public CurrencyRate(string name, decimal rate)
        {
            Name = name;
            Rate = rate;
        }
    }
}
