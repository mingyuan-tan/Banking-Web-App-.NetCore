using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WDT_Assignment2.Models
{
    public class Login
    {
        [Key]
        [Required]
        public int CustomerID { get; set; }

        // Denotes 1 - 1 relationship between Login and Customer 
        public virtual Customer Customer { get; set; }

        [Required, StringLength(50)]
        public string UserID { get; set; }

        [Required, StringLength(64)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        [Required, StringLength(8)]
        public DateTime ModifyDate { get; set; }
    }
}