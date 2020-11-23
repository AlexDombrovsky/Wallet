using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wallet.Models;
using Wallet.Services;

namespace Wallet.Controllers
{
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly CurrencyConverterService _currencyConverterService;
        public readonly List<User> Users = new List<User>();

        public WalletController(CurrencyConverterService currencyConverterService)
        {
            Users.Add(new User
            {
                Id = 1,
                Name = "Ivan",
                Wallet = new Dictionary<string, decimal>
                {
                    {"RUB", 10000},
                    {"USD", 500},
                    {"CAD", 10},
                    {"AUD", 150},
                    {"JPY", 0},
                    {"PLN", 0}
                }
            });
            Users.Add(new User
            {
                Id = 2,
                Name = "Alex",
                Wallet = new Dictionary<string, decimal>
                {
                    {"RUB", 5000},
                    {"USD", 1700},
                    {"CAD", 100},
                    {"AUD", 100},
                    {"JPY", 0},
                    {"PLN", 1000}
                }
            });
            Users.Add(new User
            {
                Id = 3,
                Name = "John",
                Wallet = new Dictionary<string, decimal>
                {
                    {"RUB", 0},
                    {"USD", 2000},
                    {"CAD", 800},
                    {"AUD", 700},
                    {"JPY", 30000},
                    {"PLN", 0}
                }
            });

            _currencyConverterService = currencyConverterService;
        }

        [Route("api/Wallet/GetWallet/{id:int}")]
        [HttpGet]
        public User GetWallet(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new NullReferenceException("User was not found");

            return user;
        }

        [Route("api/Wallet/AddMoney/{id:int}/{currencyName}/{amount:int}")]
        [HttpPost]
        public void AddMoney(int id, string currencyName, decimal amount)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new NullReferenceException("User was not found");

            user.Wallet[currencyName.ToUpper()] += amount;
        }

        [Route("api/Wallet/WithdrawMoney/{id:int}/{currencyName}/{amount:int}")]
        [HttpPost]
        public void WithdrawMoney(int id, string currencyName, decimal amount)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new NullReferenceException("User was not found");
            if (user.Wallet[currencyName.ToUpper()] >= amount) user.Wallet[currencyName.ToUpper()] -= amount;
        }

        [Route("api/Wallet/ConvertMoney/{id:int}/{fromCurrency}/{toCurrency}/{amount:int}")]
        [HttpPost]
        public void ConvertMoney(int id, string fromCurrency, string toCurrency, decimal amount)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new NullReferenceException("User was not found");

            try
            {
                if (user.Wallet[fromCurrency.ToUpper()] < amount)
                    throw new NullReferenceException("You don't have enough money!");

                var exchangeRate = _currencyConverterService.GetExchangeRate(fromCurrency, toCurrency, amount);
                user.Wallet[fromCurrency.ToUpper()] -= amount;
                user.Wallet[toCurrency.ToUpper()] += exchangeRate;
            }
            catch
            {
                throw new NullReferenceException("Сonversion failed");
            }
        }
    }
}