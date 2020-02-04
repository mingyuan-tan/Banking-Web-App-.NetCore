using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleHashing;
using WDT_Assignment2.Data;
using WDT_Assignment2.Models;
using System.Threading;

namespace WDT_Assignment2.Controllers
{
    public class LoginsController : Controller
    {
        private readonly NwbaContext _context;

        public LoginsController(NwbaContext context) => _context = context;

        public IActionResult Login() => View(); 

        // Login Method
        [HttpPost]
        public async Task<IActionResult> Login(string userID, string password)
        {
            var login = await _context.Logins.FindAsync(userID);
            var customer = await _context.Customers.FindAsync(login.CustomerID);

            if(login == null || !PBKDF2.Verify(login.Password, password))
            {
                login.LoginAttempts++;
                await _context.SaveChangesAsync();
                ViewBag.Attempts = login.LoginAttempts;
                if(login.LoginAttempts >= 3)
                {
                    //await ResetLoginAttempts(login);
                    customer.Status = "Blocked";
                    await _context.SaveChangesAsync();
                    return RedirectToAction ("AccountLocked", "Logins");
                    
                }
                ModelState.AddModelError("LoginFailed", "Login attempt no. " + login.LoginAttempts + " failed, please try again.");
                return View(new Login { UserID = userID });
            } 

            // Login customer.
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.CustomerName), login.Customer.CustomerName);
            HttpContext.Session.SetString("UserID", login.UserID);
            login.LoginAttempts = 0;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Customers");
        }

        //Logout Method 
        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult AccountLocked()
        {
            return View();
        }

        [HttpPost]
        public async Task ResetLoginAttempts(Login login)
        {
            Thread.Sleep(30000);
            login.Customer.Status = "Active";
            login.LoginAttempts = 0;
            await _context.SaveChangesAsync();
        }

















        //-------------------------------------------- METHODS BELOW ARE AUTO-CREATED ------------------------------------------------//
        // GET: Logins
        public async Task<IActionResult> Index()
        {
            var nwbaContext = _context.Logins.Include(l => l.Customer);
            return View(await nwbaContext.ToListAsync());
        }

        // GET: Logins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // GET: Logins/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName");
            return View();
        }

        // POST: Logins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,UserID,Password,ModifyDate")] Login login)
        {
            if (ModelState.IsValid)
            {
                _context.Add(login);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", login.CustomerID);
            return View(login);
        }

        // GET: Logins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", login.CustomerID);
            return View(login);
        }

        // POST: Logins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,UserID,Password,ModifyDate")] Login login)
        {
            if (id != login.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(login);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginExists(login.CustomerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerName", login.CustomerID);
            return View(login);
        }

        // GET: Logins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var login = await _context.Logins.FindAsync(id);
            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginExists(int id)
        {
            return _context.Logins.Any(e => e.CustomerID == id);
        }
    }
}
