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
    public class BillPaysController : Controller
    {
        private readonly NwbaContext _context;

        public BillPaysController(NwbaContext context)
        {
            _context = context;
        }

        // GET: BillPays
        public async Task<IActionResult> Index()
        {
            var nwbaContext = _context.BillPays.Include(b => b.Account).Include(b => b.Payee);
            return View(await nwbaContext.ToListAsync());
        }

        // GET: BillPays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPays
                .Include(b => b.Account)
                .Include(b => b.Payee)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            if (billPay == null)
            {
                return NotFound();
            }

            return View(billPay);
        }

        // GET: BillPays/Create
        public IActionResult Create()
        {
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType");
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName");
            return View();
        }

        // POST: BillPays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleDate,Period")] BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName", billPay.PayeeID);
            return View(billPay);
        }

        // GET: BillPays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPays.FindAsync(id);
            if (billPay == null)
            {
                return NotFound();
            }
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName", billPay.PayeeID);
            return View(billPay);
        }

        // POST: BillPays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleDate,Period")] BillPay billPay)
        {
            if (id != billPay.BillPayID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billPay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillPayExists(billPay.BillPayID))
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
            ViewData["AccountNumber"] = new SelectList(_context.Accounts, "AccountNumber", "AccountType", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName", billPay.PayeeID);
            return View(billPay);
        }

        // GET: BillPays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billPay = await _context.BillPays
                .Include(b => b.Account)
                .Include(b => b.Payee)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            if (billPay == null)
            {
                return NotFound();
            }

            return View(billPay);
        }

        // POST: BillPays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billPay = await _context.BillPays.FindAsync(id);
            _context.BillPays.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillPayExists(int id)
        {
            return _context.BillPays.Any(e => e.BillPayID == id);
        }
    }
}
