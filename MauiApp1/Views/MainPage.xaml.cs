using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Timers;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private Bank bank = new Bank();
        private int nextlAccountNumber = 1;
        private int nextsAccountNumber = 1;
        private System.Timers.Timer interestTimer;
        private System.Threading.Timer simpleCountdownTimer;
        private int secondsRemaining = 30;

        public MainPage()
        {
            InitializeComponent();
            DefaultAccounts();
            StartInterestTimer();
            StartSimpleTimer();
        }

        private void DefaultAccounts()
        {
            var savingsAccount = new Savingsaccount(nextsAccountNumber++.ToString(), 1000, 2.5);
            bank.AddAccount(savingsAccount);
            var loanAccount = new Loanaccount(nextlAccountNumber++.ToString(), 5000, 2.5, 10000);
            bank.AddAccount(loanAccount);

            RefreshAccountDisplay();
        }

    
        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "What type of account do you want to create?",
                "Cancel",
                null,
                "Savings Account",
                "Loan Account");

            if (action == "Savings Account")
            {
                var savingsAccount = new Savingsaccount(nextsAccountNumber++.ToString(), 1000, 2.5);
                bank.AddAccount(savingsAccount);
                await DisplayAlert("Success", "Savings Account Created!", "OK");
            }
            else if (action == "Loan Account")
            {
                var loanAccount = new Loanaccount(nextlAccountNumber++.ToString(), 5000, 2.5, 10000); 
                bank.AddAccount(loanAccount);
                await DisplayAlert("Success", "Loan Account Created!", "OK");
            }

            RefreshAccountDisplay();
        }
        private async void OnDeleteAccountClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "Which type of account do you want to delete?",
                "Cancel",
                null,
                "Savings Account",
                "Loan Account");

            if (action == "Savings Account")
            {
                string accountNumber = await DisplayPromptAsync("Delete Savings Account", "Enter the account number:");

                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    var savingsAccount = bank.GetAccounts()
                                             .OfType<Savingsaccount>()
                                             .FirstOrDefault(a => a.GetAccountNumber() == accountNumber);

                    if (savingsAccount != null)
                    {
                        bool confirm = await DisplayAlert("Confirm Delete",
                                                          $"Are you sure you want to delete Savings Account {accountNumber}?",
                                                          "Yes", "No");

                        if (confirm)
                        {
                            bank.RemoveAccount(savingsAccount);
                            await DisplayAlert("Deleted", $"Savings Account {accountNumber} has been removed.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Not Found", $"No savings account with number {accountNumber} found.", "OK");
                    }
                }
            }
            else if (action == "Loan Account")
            {
                string accountNumber = await DisplayPromptAsync("Delete Loan Account", "Enter the account number:");

                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    var loanAccount = bank.GetAccounts()
                                          .OfType<Loanaccount>()
                                          .FirstOrDefault(a => a.GetAccountNumber() == accountNumber);

                    if (loanAccount != null)
                    {
                        bool confirm = await DisplayAlert("Confirm Delete",
                                                          $"Are you sure you want to delete Loan Account {accountNumber}?",
                                                          "Yes", "No");

                        if (confirm)
                        {
                            bank.RemoveAccount(loanAccount);
                            await DisplayAlert("Deleted", $"Loan Account {accountNumber} has been removed.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Not Found", $"No loan account with number {accountNumber} found.", "OK");
                    }
                }
            }
            RefreshAccountDisplay();
        }

        private async void OnDepositClicked(object sender, EventArgs e)
        {
            var accounts = bank.GetAccounts();
            if (!accounts.Any())
            {
                await DisplayAlert("Error", "No accounts available", "OK");
                return;
            }

            // Prompt user to enter deposit amount
            string amountStr = await DisplayPromptAsync("Deposit", "Enter the amount to deposit:");
            if (!double.TryParse(amountStr, out double amount) || amount <= 0)
            {
                await DisplayAlert("Error", "Invalid amount", "OK");
                return;
            }

            string[] accountOptions = accounts.Select(a => a.GetAccountNumber()).ToArray();
            string selectedAccountNumber = await DisplayActionSheet("Select account", "Cancel", null, accountOptions);

            if (selectedAccountNumber == "Cancel" || string.IsNullOrEmpty(selectedAccountNumber))
                return;

            var account = accounts.FirstOrDefault(a => a.GetAccountNumber() == selectedAccountNumber);
            if (account == null)
            {
                await DisplayAlert("Error", "Account not found", "OK");
                return;
            }

            account.Deposit(amount);
            await DisplayAlert("Success", $"Deposited {amount:C} to Account {selectedAccountNumber}", "OK");
            RefreshAccountDisplay();
        }

        private async void OnWithdrawClicked(object sender, EventArgs e)
        {
            var accounts = bank.GetAccounts();
            if (!accounts.Any())
            {
                await DisplayAlert("Error", "No accounts available", "OK");
                return;
            }

            string amountStr = await DisplayPromptAsync("Withdraw", "Enter the amount to withdraw:");
            if (!double.TryParse(amountStr, out double amount) || amount <= 0)
            {
                await DisplayAlert("Error", "Invalid amount", "OK");
                return;
            }

            string[] accountOptions = accounts.Select(a => a.GetAccountNumber()).ToArray();
            string selectedAccountNumber = await DisplayActionSheet("Select account", "Cancel", null, accountOptions);

            if (selectedAccountNumber == "Cancel" || string.IsNullOrEmpty(selectedAccountNumber))
                return;

            var account = accounts.FirstOrDefault(a => a.GetAccountNumber() == selectedAccountNumber);
            if (account == null)
            {
                await DisplayAlert("Error", "Account not found", "OK");
                return;
            }

            bool success = account.Withdraw(amount);
            if (success)
            {
                await DisplayAlert("Success", $"Withdrew {amount:C} from Account {selectedAccountNumber}", "OK");
                RefreshAccountDisplay();
            }
            else
            {
                await DisplayAlert("Error", "Insufficient funds or invalid operation", "OK");
            }
        }

        private void RefreshAccountDisplay()
        {
            var accountDetails = bank.GetAccounts()
                                     .Select(a => a.GetAccountDetails())
                                     .ToList();

            accountsLabel.Text = string.Join("\n\n", accountDetails);
        }
        private void StartInterestTimer()
        {
            interestTimer = new System.Timers.Timer(30000); 
            interestTimer.Elapsed += OnInterestTimerElapsed;
            interestTimer.AutoReset = true;
            interestTimer.Start();
        }
        private void OnInterestTimerElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var account in bank.GetAccounts().OfType<Loanaccount>())
            {
                account.ApplyInterest();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("Interest applied to all loan accounts.");
            });
        }

        private void StartSimpleTimer()
        {
            simpleCountdownTimer = new System.Threading.Timer(_ =>
            {
                secondsRemaining--;

                Dispatcher.Dispatch(() =>
                {
                    TimerLabel.Text = $"New interest rate in: {secondsRemaining}s";
                });

                if (secondsRemaining <= 0)
                {
                    secondsRemaining = 30; 
                }

            }, null, 0, 1000); // Starts it immedietly and then ticks every 1 second
        }
    }
}
