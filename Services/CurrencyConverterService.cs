using System;
using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Wallet.Services
{
    public class CurrencyConverterService
    {
        private  readonly IConfiguration _configuration;

        public CurrencyConverterService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public decimal GetCurrencyRateInEuro(string currency)
        {
            if (currency.ToUpper() == "")
                throw new ArgumentException("Invalid Argument! currency parameter cannot be empty!");
            if (currency.ToUpper() == "EUR")
                throw new ArgumentException("Invalid Argument! Cannot get exchange rate from EURO to EURO");

            try
            {
                // Create & Load New Xml Document
                var doc = new XmlDocument();

                doc.Load(_configuration.GetSection("PublicApi").GetValue<string>("ExchangeRate"));

                // Get list of daily currency exchange rate between selected "currency" and the EURO
                var nodeList = doc.SelectNodes("//*[@currency]");

                // Loop Through all XMLNODES with daily exchange rates
                if (nodeList != null)
                    foreach (XmlNode node in nodeList)
                        if (node.Attributes != null && node.Attributes["currency"].Value == currency.ToUpper())
                        {
                            var ci = (CultureInfo) CultureInfo.CurrentCulture.Clone();
                            ci.NumberFormat.CurrencyDecimalSeparator = ".";

                            try
                            {
                                // Get currency exchange rate with EURO from XMLNODE
                                var exchangeRate = decimal.Parse(node.Attributes["rate"].Value, NumberStyles.Any, ci);

                                return exchangeRate;
                            }
                            catch
                            {
                            }
                        }

                // currency not parsed
                // return default value
                return 0;
            }
            catch
            {
                // currency not parsed
                // return default value
                return 0;
            }
        }

        public decimal GetExchangeRate(string from, string to, decimal amount = 1)
        {
            // If currency's are empty abort
            if (from == null || to == null)
                return 0;

            // Convert Euro to Euro
            if (from.ToUpper() == "EUR" && to.ToUpper() == "EUR")
                return amount;

            try
            {
                // First Get the exchange rate of both currencies in euro
                var toRate = GetCurrencyRateInEuro(to);
                var fromRate = GetCurrencyRateInEuro(from);

                // Convert Between Euro to Other Currency
                if (from.ToUpper() == "EUR")
                    return amount * toRate;
                if (to.ToUpper() == "EUR")
                    return amount / fromRate;
                return amount * toRate / fromRate;
            }
            catch
            {
                return 0;
            }
        }
    }
}