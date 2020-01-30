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
using Newtonsoft.Json;
using X.PagedList;
using SimpleHashing;
using WDT_Assignment2.ViewModels; 

namespace WDT_Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class CustomersController : Controller
    {
        private readonly NwbaContext _context;
        private const string AccountSessionKey = "_AccountSessionKey";

        // ReSharper disable once PossibleInvalidOperationException
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        private string UserID => HttpContext.Session.GetString("UserID");

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
        public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (amount <= 0)
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
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Withdrawal(int id) => View(await _context.Accounts.FindAsync(id));


        [HttpPost]
        public async Task<IActionResult> Withdrawal(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal withdrawalFee = 0.1m;
            var totalAmount = amount;

            if (account.Transactions.Count >= 4)
            {
                totalAmount = amount + withdrawalFee;
            }

            //if (amount <= 0)
            //    ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            //if (amount.HasMoreThanTwoDecimalPlaces())
            //    ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            //if (totalAmount > account.Balance)
            //    ModelState.AddModelError(nameof(amount), "Amount plus withdrawal fee of $0.10 must be less than account balance.");
            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid Amount");
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
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));

        public async Task<IActionResult> Transfer_Own(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Transfer_Own(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount;

            if (account.Transactions.Count >= 4)
            {
                totalAmount = amount + transferFee;
            }

            int selectedID;
            if (id % 2 == 0)
            {
                selectedID = id + 1;
            }
            else
            {
                selectedID = id - 1;
            }
            var destAccount = await _context.Accounts.FindAsync(selectedID);

            //if (amount <= 0)
            //    ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            //if (amount.HasMoreThanTwoDecimalPlaces())
            //    ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            //if (totalAmount > account.Balance)
            //    ModelState.AddModelError(nameof(amount), "Amount plus transfer fee of $ 0.20 must be less than than account balance");
            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid amount.");
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
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = amount,
                    DestinationAccountNumber = id,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer_ThirdParty(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Transfer_ThirdParty(int id, int destID, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount;

            var destAccount = await _context.Accounts.FindAsync(destID);

            if (account.Transactions.Count >= 4)
            {
                totalAmount = amount + transferFee;
            }

            //if (destID.ToString().Length != 4)
            //    ModelState.AddModelError(nameof(destID), "Destination Account No. be be 4 digits long.");
            //if (destAccount == null)
            //    ModelState.AddModelError(nameof(destID), "Destination Account No. must be a valid account number.");
            //if (amount <= 0)
            //    ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            //if (amount.HasMoreThanTwoDecimalPlaces())
            //    ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            //if (totalAmount > account.Balance)
            //    ModelState.AddModelError(nameof(amount), "Amount plus transfer fee of $ 0.20 must be less than than account balance");
            if (destID.ToString().Length != 4 || destAccount == null)
                ModelState.AddModelError(nameof(destID), "Invalid account number.");
            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid Amount.");
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
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            destAccount.Balance += amount;
            destAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = "T",
                    Amount = totalAmount,
                    DestinationAccountNumber = id,
                    Comment = comment,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AccountSelection()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(customer);
        }

        public async Task<IActionResult> IndexToViewStatements(int accountNumber)
        {
            var account = await _context.Accounts.FindAsync(accountNumber);

            if (account == null)
            {
                return NotFound();
            }

            // Store a complex object in the session via JSON serialization. 
            var accountJson = JsonConvert.SerializeObject(account);
            HttpContext.Session.SetString(AccountSessionKey, accountJson);

            return RedirectToAction(nameof(ViewStatements));

        }

        public async Task<IActionResult> ViewStatements(int? page = 1)
        {
            var accountJson = HttpContext.Session.GetString(AccountSessionKey);
            if (accountJson == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var account = JsonConvert.DeserializeObject<Account>(accountJson);
            ViewBag.Account = account;

            const int pageSize = 4;
            var pagedList = _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber);

            var pagedListOrdered = await pagedList.OrderByDescending(x => x.TransactionID).ToPagedListAsync(page, pageSize); 


            return View(pagedListOrdered);
        }

        public async Task<IActionResult> MyProfile()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(customer);
        }

        [Route("Customers/ChangePasswordView")]
        public async Task<IActionResult> ChangePassword()
        {
            var login = await _context.Logins.FindAsync(UserID);

            return View(login);
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

            ViewData["States"] = new StateViewModel().State; 

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
                return RedirectToAction(nameof(MyProfile));
            }
            return View(customer);
        }

        public async Task<IActionResult> ChangePasswordSet(string password)
        {
            var login = await _context.Logins.FindAsync(UserID);
            if (PBKDF2.Verify(login.Password, password))
                ModelState.AddModelError(nameof(password), "Cannot change to the same password");
            if (!ModelState.IsValid)
            {
                ViewBag.Password = password;
                return View("ChangePassword");
            }

            var passwordHash = PBKDF2.Hash(password);
            login.Password = passwordHash;
            login.ModifyDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> CreateBillPay()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            List<Account> accounts = new List<Account>();

            ViewData["AccountNumber"] = new SelectList(customer.Accounts, "AccountNumber", "AccountNumber");
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName");

            foreach (var account in customer.Accounts)
            {
                accounts.Add(account); 
            }

            return View(
                new BillPayViewModel
                {
                    Customer = customer, 
                    Accounts = accounts
                });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBillPay(int accountNumber, int payeeID, decimal amount, DateTime scheduleDate, string period)
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            var account = await _context.Accounts.FindAsync(accountNumber);

            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || amount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid amount");
            if (scheduleDate < DateTime.Today)
                ModelState.AddModelError(nameof(scheduleDate), "Invalid date");
            if (!ModelState.IsValid)
            {
                List<Account> accounts = new List<Account>();

                ViewData["AccountNumber"] = new SelectList(customer.Accounts, "AccountNumber", "AccountNumber");
                ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName");

                foreach (var acc in customer.Accounts)
                {
                    accounts.Add(acc);
                }

                ViewBag.Amount = amount;
                ViewBag.ScheduleDate = scheduleDate;
                return View(new BillPayViewModel
                {
                    Customer = customer,
                    Accounts = accounts
                });
                //return View();
            }

            _context.BillPays.Add(
                new BillPay
                {
                    AccountNumber = accountNumber,
                    PayeeID = payeeID,
                    Amount = amount,
                    ScheduleDate = scheduleDate,
                    Period = period
                });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllScheduledPayments));
        }

        public IActionResult AllScheduledPayments()
        {
            List<BillPay> BillPays = new List<BillPay>();

            var accounts = _context.Accounts.Include(a => a.BillPays);

            foreach (var a in accounts)
            {
                foreach (var b in a.BillPays)
                {
                    BillPays.Add(b);
                }
            }

            return View(BillPays);
        }

        public async Task<IActionResult> ModifyBillPay(int? id)
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

            var customer = await _context.Customers.FindAsync(CustomerID);

            ViewData["Periods"] = new BillPayViewModel().Periods;
            ViewData["AccountNumber"] = new SelectList(customer.Accounts, "AccountNumber", "AccountNumber");
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName", billPay.PayeeID);
            return View(billPay);
        }

        private bool BillPayExists(int id)
        {
            return _context.BillPays.Any(e => e.BillPayID == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyBillPay(int billpayID, int accountNumber, int payeeID, decimal amount, DateTime scheduleDate, string period)
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            var account = await _context.Accounts.FindAsync(accountNumber);
            var billPay = await _context.BillPays.FindAsync(billpayID);

            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || amount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid amount");
            if (scheduleDate < DateTime.Today)
                ModelState.AddModelError(nameof(scheduleDate), "Invalid date");

            ViewData["Periods"] = new BillPayViewModel().Periods;
            ViewData["AccountNumber"] = new SelectList(customer.Accounts, "AccountNumber", "AccountNumber");
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName", billPay.PayeeID);


            if (!ModelState.IsValid)
            {
                List<Account> accounts = new List<Account>();

                ViewData["AccountNumber"] = new SelectList(customer.Accounts, "AccountNumber", "AccountNumber");
                ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "PayeeName");

                foreach (var acc in customer.Accounts)
                {
                    accounts.Add(acc);
                }

                ViewBag.Amount = amount;
                ViewBag.ScheduleDate = scheduleDate;
                return View(billPay);
            }

            billPay.AccountNumber = accountNumber;
            billPay.PayeeID = payeeID;
            billPay.Amount = amount;
            billPay.ScheduleDate = scheduleDate;
            billPay.Period = period;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllScheduledPayments));

        }

        public async Task<IActionResult> DeleteBillPay(int id)
        {
            var billPay = await _context.BillPays.FindAsync(id);
            _context.BillPays.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllScheduledPayments));
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
