using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDT_Assignment2.Data;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly NwbaContext _context;

        public TransactionsController(NwbaContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var nwbaContext = _context.Transactions.Include(t => t.Account).Include(t => t.DestinationAccount);
            return View(await nwbaContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.DestinationAccount)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType");
            ViewData["DestinationAccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionID,TransactionType,AccountNumber,DestinationAccountNumber,Amount,Comment,ModifyDate")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.AccountNumber);
            ViewData["DestinationAccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.DestinationAccountNumber);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.AccountNumber);
            ViewData["DestinationAccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.DestinationAccountNumber);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,TransactionType,AccountNumber,DestinationAccountNumber,Amount,Comment,ModifyDate")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
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
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.AccountNumber);
            ViewData["DestinationAccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", transaction.DestinationAccountNumber);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.DestinationAccount)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionID == id);
        }
    }
}
