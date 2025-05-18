using System;

namespace MauiApp1
{
    public class Loanaccount : BankAccount
    {
        public double LoanAmount { get; private set; }
        public double InterestRate { get; set; }
        public double MaxLoanAmount { get; } = 10000;

        public Loanaccount(string accountNumber, double initialBalance, double interestRate, double loanAmount)
            : base(accountNumber, initialBalance)
        {
            if (interestRate < 0)
                throw new ArgumentException("Interest rate cannot be negative.");

            if (loanAmount < 0)
                throw new ArgumentException("Loan amount cannot be negative.");

            InterestRate = interestRate;
            LoanAmount = Math.Min(loanAmount, MaxLoanAmount);
        }

        public override string GetAccountDetails()
        {
            return $"Loan Account {GetAccountNumber()}: Balance = {GetBalance():C}, Interest Rate = {InterestRate}%, Available Credit = {LoanAmount:C} / {MaxLoanAmount:C}";
        }

        public void ApplyInterest()
        {
            double interest = LoanAmount * (InterestRate / 100);
            LoanAmount = Math.Min(MaxLoanAmount, LoanAmount + interest);
        }

        public override bool Withdraw(double amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");

            if (amount <= GetBalance())
            {
                Balance -= amount;
                return true;
            }
            else
            {
                double remainder = amount - GetBalance();

                if (remainder <= LoanAmount)
                {
                    Balance = 0;
                    LoanAmount -= remainder;
                    return true;
                }
            }

            return false;
        }

        public override void Deposit(double amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");

            double spaceToRepay = MaxLoanAmount - LoanAmount;

            if (amount <= spaceToRepay)
            {
                LoanAmount += amount;
            }
            else
            {
                LoanAmount = MaxLoanAmount;
                double extra = amount - spaceToRepay;
                base.Deposit(extra); 
            }
        }
    }
}
