using System.Collections.Generic;

namespace Wallet.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, decimal> Wallet { get; set; }
    }
}