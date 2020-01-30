using System;
using System.Linq;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.BusinessObjects
{
    public class ATMMethods
    {
        // Deposit method logic 
        public void Deposit(Account account, int id, decimal amount, string comment)
        {
            account.Balance += amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "D",
                    Amount = amount,
                    DestinationAccountNumber = id,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });
        }

        // Withdrawal method logic
        public void Withdrawal(Account account, int id, decimal amount, string comment)
        {
            const decimal withdrawalFee = 0.1m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" && x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;

            if (transactionsMade >= 4)
            {
                totalAmount = amount + withdrawalFee;
            }

            account.Balance -= amount;

            // Creates new transaction for withdrawal
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "W",
                    Amount = amount,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            // Creates new transaction for service charge
            if (transactionsMade >= 4)
            {
                account.Balance -= withdrawalFee;

                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "S",
                    Amount = withdrawalFee,
                    Comment = "Service charge for withdrawal",
                    ModifyDate = DateTime.UtcNow
                });
            }
        }

        // Transfer to own method logic
        public void Transfer_Own(Account account, Account destAccount, int id, int selectedID, decimal amount, string comment)
        {
            const decimal transferFee = 0.2m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" && x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;

            // Creates new transaction for transfer
            account.Balance -= amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    DestinationAccountNumber = selectedID,
                    Comment = "To account no. " + selectedID + ": " + comment,
                    ModifyDate = DateTime.UtcNow
                });

            // Creates new transaction for service charge
            if (transactionsMade >= 4)
            {
                account.Balance -= transferFee;

                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "S",
                    Amount = transferFee,
                    Comment = "Service charge for transfer",
                    ModifyDate = DateTime.UtcNow
                });
            }

            // Createse new transaction in destination account
            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    Comment = "From account no. " + id + ": " + comment,
                    ModifyDate = DateTime.UtcNow
                });
        }

        // Transfer to third-party method logic
        public void Transfer_ThirdParty(Account account, Account destAccount, int id, int destID, decimal amount, string comment)
        {
            const decimal transferFee = 0.2m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" && x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;

            account.Balance -= amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    DestinationAccountNumber = destID,
                    Comment = "To account no. " + destID + ": " + comment,
                    ModifyDate = DateTime.UtcNow
                });

            // Creates new transaction for service charge
            if (transactionsMade >= 4)
            {
                account.Balance -= transferFee;

                account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "S",
                    Amount = transferFee,
                    Comment = "Service charge for transfer",
                    ModifyDate = DateTime.UtcNow
                });
            }

            // Createse new transaction in destination account
            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    Comment = "From account no. " + id + ": " + comment,
                    ModifyDate = DateTime.UtcNow
                });
        }
    }
}
