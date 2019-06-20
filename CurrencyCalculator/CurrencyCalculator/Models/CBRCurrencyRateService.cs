﻿using Portable.Text;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CurrencyCalculator.Models
{
    static public class CBRCurrencyRateService
    {
        static private string _url = "http://www.cbr.ru/scripts/XML_daily_eng.asp?date_req=";

        static public async Task<List<CurrencyRate>> GetRates(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException("date");

            var url = _url + date.ToString("dd/MM/yyyy");
            using (var client = new HttpClient())
            {
                var win1251Bytes = await client.GetByteArrayAsync(url);

                var utf8String = Win1251BytesToUtf8String(win1251Bytes);

                var rates = GetRatesFromXml(utf8String);
                rates.Add(new CurrencyRate("Russian ruble", 1m));
                return rates;
            }
        }

        static private string Win1251BytesToUtf8String(byte[] win1251Bytes)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = Encoding.Convert(win1251, utf8, win1251Bytes);
            var utf8String = utf8.GetString(utf8Bytes);
            return utf8String;
        }

        static private List<CurrencyRate> GetRatesFromXml(string utf8String)
        {
            var content = XElement.Parse(utf8String);
            IEnumerable<XElement> valutes = content.Elements();
            List<CurrencyRate> rates = new List<CurrencyRate>();

            foreach (var valute in valutes)
            {
                var name = valute.Element("Name").Value;
                var nominal = decimal.Parse(valute.Element("Nominal").Value.Replace(',', '.'));
                var value = decimal.Parse(valute.Element("Value").Value.Replace(',', '.'));
                var rate = value / nominal;
                rates.Add(new CurrencyRate(name, rate));
            }
            return rates;
        }
    }
}
