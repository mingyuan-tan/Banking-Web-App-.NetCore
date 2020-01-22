using Microsoft.EntityFrameworkCore;
using WDT_Assignment2.Models;
using WDT_Assignment2.Data;

namespace WDT_Assignment2.Data
{
    public class NwbaContext : DbContext
    {
        public NwbaContext(DbContextOptions<NwbaContext> options) : base(options)
        { }


        // Write Fluent API here 
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<Payee> Payees { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Login>().HasCheckConstraint("CH_Login_UserID", "len(UserID) = 8").
                HasCheckConstraint("CH_Login_Password", "len(Password) = 64");

            // Set the check for the balance and account type char 
            builder.Entity<Account>().HasCheckConstraint("CH_Account_Balance", "Balance >= 0")
                .HasCheckConstraint("CH_Account_AccountType", "(AccountType in ('C', 'S'))");

            // Set up the one to many relationship between Account and Transaction && Set up the Foreign Key == AccountNumber 
            builder.Entity<Transaction>().
                HasOne(x => x.Account).WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountNumber);

            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0")
                .HasCheckConstraint(" CH_Transaction_TransactionType", "(TransactionType in ('D', 'W', 'T', 'S', 'B'))");

            // Set validation for Period char
            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_Period", "(Period in ('M', 'Q', 'Y', 'S'))");

            builder.Entity<Payee>().HasCheckConstraint("CH_Payee_State", "(State in ('VIC', 'NSW', 'ACT', 'WA', 'QLD', 'SA'))");
        }

    }
}
