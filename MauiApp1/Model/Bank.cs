using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauiApp1
{
    using System.Collections.Generic;

    public class Bank
    {
        public List<BankAccount> accounts = new List<BankAccount>();



        public BankAccount BankAccount
        {
            get => default;
            set
            {
            }
        }

        public void AddAccount(BankAccount account)
        {
            accounts.Add(account);
        }
        public void RemoveAccount(BankAccount account)
        {
            accounts.Remove(account);
        }

        public List<BankAccount> GetAccounts()
        {
            return accounts;
        }
    }
}
