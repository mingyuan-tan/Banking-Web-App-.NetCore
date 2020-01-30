using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WDT_Assignment2.Models
{
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        [Required, Range(1000,9999)] // ensures 4 digits 
        public int AccountNumber { get; set; }

        [Required, StringLength(1, MinimumLength = 1)]
        [RegularExpression("^(S|C|)$", ErrorMessage = "Invalid Account Type. Enter 'S' for Savings Account and 'C' for Checking Account" )]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        public int CustomerID { get; set; }

        public virtual Customer Customer { get; set; }

        [Column(TypeName = "Money")]
        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue, ErrorMessage = "Balance cannot be below $0")]
        public decimal Balance { get; set; }

        // Navigation Property - Account has many Transactions 
        public virtual List<Transaction> Transactions { get; set; }
        // Navigation Property - Account has many BillPays 
        public virtual List<BillPay> BillPays { get; set; }
    }
}
