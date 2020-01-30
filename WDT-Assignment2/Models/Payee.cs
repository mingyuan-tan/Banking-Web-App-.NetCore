using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WDT_Assignment2.Models
{
    public class Payee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        [Required, Range(1000, 9999)]
        public int PayeeID { get; set; }

        [Required(ErrorMessage = "Please enter name"), StringLength(50)]
        public string PayeeName { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        // Spec says 3 lettered AU States but there are WA and SA (2 letters). Cannot set exact 3.
        [StringLength(3)]
        [RegularExpression("^(VIC|NSW|WA|SA|TAS|QLD)$", ErrorMessage = "Invalid State. Please choose between VIC, NSW, WA, SA, TAS or ACT")]
        public string State { get; set; }

        [StringLength(4, MinimumLength = 4)]
        [RegularExpression("^(0[289][0-9]{2})|([1345689][0-9]{3})|(2[0-8][0-9]{2})|(290[0-9])|(291[0-4])|(7[0-4][0-9]{2})|(7[8-9][0-9]{2})$", ErrorMessage = "Please enter a valid post code")]
        public string PostCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(^1300\d{6}$)|(^1800|1900|1902\d{6}$)|(^0[2|3|7|8]{1}[0-9]{8}$)|(^13\d{4}$)|(^04\d{2,3}\d{6}$)",
            ErrorMessage = "Please enter a valid phone number")]
        [Required]
        public string Phone { get; set; }

        // Navigation Property - Payee has many BillPays 
        public virtual List <BillPay> BillPays { get; set; }
    }
}
