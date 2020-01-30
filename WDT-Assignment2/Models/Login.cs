using System;
using System.ComponentModel.DataAnnotations;

namespace WDT_Assignment2.Models
{
    public class Login
    {
        [Required]
        public int CustomerID { get; set; }

        // Denotes 1 - 1 relationship between Login and Customer 
        public virtual Customer Customer { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "User ID")]
        [Key]
        public string UserID { get; set; }

        [Required, StringLength(64)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [Required, StringLength(8)]
        public DateTime ModifyDate { get; set; }
    }
}