using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WDT_Assignment2.ViewModels
{
    public class StateViewModel
    {
        public List<SelectListItem> State { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "VIC", Text = "VIC" },
            new SelectListItem { Value = "NSW", Text = "NSW" },
            new SelectListItem { Value = "QLD", Text = "QLD"  },
            new SelectListItem { Value = "WA", Text = "WA"  },
            new SelectListItem { Value = "SA", Text = "SA"  },
            new SelectListItem { Value = "TAS", Text = "TAS"  },

        };
    }
}
