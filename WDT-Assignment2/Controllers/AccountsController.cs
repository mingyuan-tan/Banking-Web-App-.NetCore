using Microsoft.AspNetCore.Mvc;
using WDT_Assignment2.Data;

namespace WDT_Assignment2.Controllers
{
    public class AccountsController : Controller
    {
        private readonly NwbaContext _context;

        public AccountsController(NwbaContext context)
        {
            _context = context;
        }
    }
}
