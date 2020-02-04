using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WDT_Assignment2.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required, Range(1000, 9999)]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Please enter name"), StringLength(50)]
        [Display(Name = "Name")]
        public string CustomerName { get; set; }

        [StringLength(11, MinimumLength = 11)]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(3)]

        [RegularExpression("^(VIC|NSW|WA|SA|TAS|QLD)$", ErrorMessage = "Invalid State. Please choose between VIC, NSW, WA, SA, TAS or ACT")]
        public string State { get; set; }

        [Display(Name = "Post Code")]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression("^(0[289][0-9]{2})|([1345689][0-9]{3})|(2[0-8][0-9]{2})|(290[0-9])|(291[0-4])|(7[0-4][0-9]{2})|(7[8-9][0-9]{2})$", ErrorMessage = "Please enter a valid post code")]
        public string PostCode { get; set; }

        // All Australian phone numbers (landlines only - area code required) (61)XXXX XXXX as per spec sheet 
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(^1300\d{6}$)|(^1800|1900|1902\d{6}$)|(^0[2|3|7|8]{1}[0-9]{8}$)|(^13\d{4}$)|(^04\d{2,3}\d{6}$)",
            ErrorMessage = "Please enter a valid phone number")]
        [Required]
        public string Phone { get; set; }

        [Required]
        [RegularExpression("^(Active|Blocked)$")]
        public string Status { get; set; }

        // Navigation Property - Customer has many accounts 
        [JsonIgnore]
        public virtual List<Account> Accounts { get; set;}

        // Denotes 1 - 1 relationship between Customer and Login 
        [JsonIgnore]
        public virtual Login Login { get; set; }
    }
}
