using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new NwbaContext(serviceProvider.GetRequiredService<DbContextOptions<NwbaContext>>());

            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    CustomerName = "Matthew Bolger",
                    Address = "123 Fake Street",
                    City = "Melbourne",
                    PostCode = "3000",
                    Phone = "0422367027"
                },
                new Customer
                {
                    CustomerID = 2200,
                    CustomerName = "Rodney Cocker",
                    Address = "456 Real Road",
                    City = "Melbourne",
                    PostCode = "3005",
                    Phone = "0422367027"
                },
                new Customer
                {
                    CustomerID = 2300,
                    CustomerName = "Shekhar Kalra",
                    Phone = "0422367027"
                });

            context.Logins.AddRange(
                new Login
                {
                    UserID = "12345678",
                    CustomerID = 2100,
                    Password = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2"
                },
                new Login
                {
                    UserID = "38074569",
                    CustomerID = 2200,
                    Password = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04"
                },
                new Login
                {
                    UserID = "17963428",
                    CustomerID = 2300,
                    Password = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE"
                });

            context.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = "S",
                    CustomerID = 2100,
                    Balance = 100
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = "C",
                    CustomerID = 2100,
                    Balance = 500
                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = "S",
                    CustomerID = 2200,
                    Balance = 500.95m
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = "S",
                    CustomerID = 2300,
                    Balance = 1250.50m
                });

            const string openingBalance = "Opening balance";
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            context.Transactions.AddRange(
                new Transaction
                {
                    TransactionType = "D",
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = "D",
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = "D",
                    AccountNumber = 4200,
                    Amount = 500.95m,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = "D",
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = "Opening balance",
                    ModifyDate = DateTime.ParseExact("19/12/2019 10:00:00 PM", format, null)
                });

            context.BillPays.AddRange(
                new BillPay
                {
                    AccountNumber = 4100, 
                    PayeeID = 5000, 
                    Amount = 44.90m, 
                    ScheduleDate = DateTime.ParseExact("30/12/2019 09:00:00 AM", format, null), 
                    Period = "M"
                },
                new BillPay
                {
                    AccountNumber = 4100,
                    PayeeID = 5001,
                    Amount = 299.95m,
                    ScheduleDate = DateTime.ParseExact("30/12/2019 09:00:00 AM", format, null),
                    Period = "M"
                });

            context.Payees.AddRange(
               new Payee
               { 
                   PayeeID = 5000, 
                   PayeeName = "Optus", 
                   Address = "T4/359 Docklands Dr", 
                   City = "Melbourne", 
                   State = "VIC", 
                   PostCode = "3008",
                   Phone = "0421222065"
               },
               new Payee
               {
                   PayeeID = 5001,
                   PayeeName = "Telstra",
                   Address = "242 Exhibition St",
                   City = "Melbourne",
                   State = "VIC",
                   PostCode = "3000",
                   Phone = "0427343061"
               },
               new Payee
               {
                   PayeeID = 5002,
                   PayeeName = "Vodafone",
                   Address = "250 Ingles St",
                   City = "Port Melbourne",
                   State = "VIC",
                   PostCode = "3207",
                   Phone = "0427343777"
               },
                new Payee
                {
                    PayeeID = 5003,
                    PayeeName = "Fitness First",
                    Address = "715 Bondi Junction",
                    City = "Sydney",
                    State = "NSW",
                    PostCode = "1355",
                    Phone = "0421937067"
                },
                new Payee
                {
                    PayeeID = 5004,
                    PayeeName = "RMIT University",
                    Address = "124 La Trobe St",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Phone = "99252000"
                },
                new Payee
                {
                    PayeeID = 5005,
                    PayeeName = "University of Melbourne",
                    Address = "Parkville",
                    City = "Sydney",
                    State = "VIC",
                    PostCode = "1355",
                    Phone = "0421955555"
                });

            context.SaveChanges();
        }
    }
}
