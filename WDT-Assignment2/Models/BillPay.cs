using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WDT_Assignment2.Models
{

    public class BillPay
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required, Range(1000,9999)]
        public int BillPayID { get; set; }

        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("Payee")]
        public int PayeeID { get; set; }

        public virtual Payee Payee { get; set; }

        [Column(TypeName = "Money")]
        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be below $0")]
        public decimal Amount { get; set; }
        
        [DataType(DataType.Date)]
        [Required, StringLength(15)]
        public DateTime ScheduleDate { get; set; }

        [RegularExpression("^(M|Q|Y|S)$", ErrorMessage = "Invalid Period. Please enter 'M' for Monthly, 'Q' for Quarterly, 'Y' for Annually, or 'S' for Once Off")]
        public string Period { get; set; }

    }
}
