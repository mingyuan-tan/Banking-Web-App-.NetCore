using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.ViewModels
{
    public class BillPayViewModel
    {
        public BillPay BillPay { get; set; }
        public Customer Customer { get; set; }
        public List<Account> Accounts { get; set; }

        public int AccountNumber { get; set; }

        public int PayeeID { get; set; }

        public List<SelectListItem> Periods { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "M", Text = "Monthly" },
            new SelectListItem { Value = "Q", Text = "Quarterly" },
            new SelectListItem { Value = "Y", Text = "Annually"  },
            new SelectListItem { Value = "S", Text = "Once Off"  },

        };

    }
}
