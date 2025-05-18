using System;

public abstract class BankAccount
{
    protected string AccountNumber { get; private set; }

    private double balance;
    protected double Balance
    {
        get => balance;
        set
        {
            if (value < 0)
                throw new ArgumentException("Balance cannot be negative.");
            balance = value;
        }
    }

    public BankAccount(string accountNumber, double initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.");

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.");

        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public string GetAccountNumber()
    {
        return AccountNumber;
    }

    public double GetBalance()
    {
        return Balance;
    }

    public abstract string GetAccountDetails();

    public virtual void Deposit(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        Balance += amount;
    }

    public virtual bool Withdraw(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");

        if (amount <= Balance)
        {
            Balance -= amount;
            return true;
        }

        return false;
    }
}
