using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WDT_Assignment2.Models
{

    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required, Range(1000, 9999)]
        public int TransactionID { get; set; }

        [Required]
        [RegularExpression("^(D|W|T|S|B)$", ErrorMessage = "Invalid Transaction Type. Enter 'D' for Deposit, 'W' for Withdrawal," +
            " 'T' for Transfer, 'S' for Service Charge, and 'B' for BillPay")]
        public string TransactionType { get; set; }

        [Required]
        public int AccountNumber { get; set; }
        [JsonIgnore]
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }

        [JsonIgnore]
        public virtual Account DestinationAccount { get; set; }

        [Column(TypeName = "Money")]
        [DataType(DataType.Currency)]
        [Range(0.0001, int.MaxValue, ErrorMessage = "Amount cannot be 0")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime ModifyDate { get; set; }

    }
}
