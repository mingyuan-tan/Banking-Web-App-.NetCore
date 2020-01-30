using Microsoft.AspNetCore.Mvc;
using WDT_Assignment2.Data;


namespace WDT_Assignment2.Controllers
{
    public class PayeesController : Controller
    {
        private readonly NwbaContext _context;
        public PayeesController(NwbaContext context)
        {
            _context = context;
        }

    }
}
