using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WDT_Assignment2.Data;
using WDT_Assignment2.Models;
using WDT_Assignment2.BusinessObjects;

namespace WDT_Assignment2.Controllers
{
    public class BillPaysController : Controller
    {
        private readonly NwbaContext _context;
        private BillPayMethods billPayMethods = new BillPayMethods();

        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public BillPaysController(NwbaContext context)
        {
            _context = context;
        }
    }
}
