using Microsoft.AspNetCore.Mvc;
using WDT_Assignment2.Data;

namespace WDT_Assignment2.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly NwbaContext _context;

        public TransactionsController(NwbaContext context)
        {
            _context = context;
        }
    }
}
