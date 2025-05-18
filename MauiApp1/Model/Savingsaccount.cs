using System;

namespace MauiApp1
{
    public class Savingsaccount : BankAccount
    {
        public double InterestRate { get; set; } 

        public Savingsaccount(string accountNumber, double initialBalance, double interestRate)
            : base(accountNumber, initialBalance)
        {
            if (interestRate < 0)
                throw new ArgumentException("Interest rate cannot be negative.");

            InterestRate = interestRate;
        }

        public string DisplayInfo => $"Savings #{GetAccountNumber()} - Balance: {GetBalance():C}";

        public override string GetAccountDetails()
        {
            return $"Savings Account {GetAccountNumber()}: Balance = {GetBalance():C}, Interest Rate = {InterestRate}%";
        }

        public override void Deposit(double amount)
        {
            base.Deposit(amount);

            double interest = GetBalance() * (InterestRate / 100);
            base.Deposit(interest);
        }
    }
}
