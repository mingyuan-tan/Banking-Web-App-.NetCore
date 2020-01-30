using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WDT_Assignment2.Models
{
    public class BillPay
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required, Range(1000,9999)]
        public int BillPayID { get; set; }

        [ForeignKey("Account")]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }

        [ForeignKey("Payee")]
        [Display(Name = "Payee")]
        public int PayeeID { get; set; }

        [JsonIgnore]
        public virtual Payee Payee { get; set; }

        [Column(TypeName = "Money")]
        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be below $0")]
        public decimal Amount { get; set; }
        
        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Scheduled Date")]
        public DateTime ScheduleDate { get; set; }

        //[RegularExpression("^(M|Q|Y|S)$", ErrorMessage = "Invalid Period. Please enter 'M' for Monthly, 'Q' for Quarterly, 'Y' for Annually, or 'S' for Once Off")]
        [Display(Name = "Payment Intervals")]
        public string Period { get; set; }
    }
}
