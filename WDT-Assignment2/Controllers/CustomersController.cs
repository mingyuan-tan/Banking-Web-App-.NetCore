using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDT_Assignment2.Data;
using WDT_Assignment2.Models;
using WDT_Assignment2.Attributes;
using WDT_Assignment2.Utilities;

namespace WDT_Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class CustomersController : Controller
    {
        private readonly NwbaContext _context;

        // ReSharper disable once PossibleInvalidOperationException
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public CustomersController(NwbaContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(customer);
        }


        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);

            if(amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // Note this code could be moved out of the controller, e.g., into the Model.
            account.Balance += amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "D",
                    Amount = amount,
                    DestinationAccountNumber = id,
                    Comment = "Deposit of $" + amount,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Withdrawal(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Withdrawal (int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal withdrawalFee = 0.1m;
            var totalAmount = amount + withdrawalFee;

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Amount plus withdrawal fee of $0.10 must be less than account balance.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // Note this code could be moved out of the controller, e.g., into the Model.
            account.Balance -= totalAmount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "W",
                    Amount = totalAmount,
                    DestinationAccountNumber = id,
                    Comment = "Withdrawal of $" + amount,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer_a(int id) => View(await _context.Accounts.FindAsync(id));

        public async Task<IActionResult> Transfer_Own(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Transfer_Own(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount + transferFee;

            int selectedID;
            if(id % 2 == 0)
            {
                selectedID = id + 1;
            }
            else
            {
                selectedID = id - 1;
            }
            var destAccount = await _context.Accounts.FindAsync(selectedID);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Amount plus transfer fee of $ 0.20 must be less than than account balance");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            account.Balance -= totalAmount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = totalAmount,
                    DestinationAccountNumber = selectedID,
                    Comment = "Transfer of $" + amount + " to account number " + selectedID,
                    ModifyDate = DateTime.UtcNow
                }); ;

            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    DestinationAccountNumber = id,
                    Comment = "Transfer of $" + amount + " from account number " + id,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer_ThirdParty(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Transfer_ThirdParty(int id, int destID, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount + transferFee;

            var destAccount = await _context.Accounts.FindAsync(destID);

            if (destID.ToString().Length != 4)
                ModelState.AddModelError(nameof(destID), "Destination Account No. be be 4 digits long.");
            if (destAccount == null)
                ModelState.AddModelError(nameof(destID), "Destination Account No. must be a valid account number.");
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Amount plus transfer fee of $ 0.20 must be less than than account balance");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            account.Balance -= totalAmount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = totalAmount,
                    DestinationAccountNumber = destID,
                    Comment = "Transfer of $" + amount + " to account number " + destID,
                    ModifyDate = DateTime.UtcNow
                });

            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = totalAmount,
                    DestinationAccountNumber = id,
                    Comment = "Transfer of $" + amount + " from account number " + id,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }














        //-------------------------------------------- METHODS BELOW ARE AUTO-CREATED ------------------------------------------------//



        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,CustomerName,TFN,Address,City,State,PostCode,Phone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,CustomerName,TFN,Address,City,State,PostCode,Phone")] Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}
