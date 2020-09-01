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
using WDT_Assignment2.BusinessObjects;

namespace WDT_Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class CustomersController : Controller
    {
        // Connection to database 
        private readonly NwbaContext _context;

        // Session string
        private const string AccountSessionKey = "_AccountSessionKey";

        private ATMMethods ATMMethods = new ATMMethods();
        private CustomerMethods CustomerMethods = new CustomerMethods();
        private BillPayMethods billPayMethods = new BillPayMethods();
        
        // Get logged in Customer ID and UserID from Session variable 
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
        private string UserID => HttpContext.Session.GetString("UserID");

        public CustomersController(NwbaContext context)
        {
            _context = context;
        }


        // Return home page 
        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(customer);
        }


        // Opens initial deposit page
        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));


        // Deposit method logic 
        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);

            // If anything is wrong with deposit
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            ATMMethods.Deposit(account, id, amount, comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Opens initial withdrawal page
        public async Task<IActionResult> Withdrawal(int id) => View(await _context.Accounts.FindAsync(id));


        // Withdrawal method logic
        [HttpPost]
        public async Task<IActionResult> Withdrawal(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal withdrawalFee = 0.1m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" &&  x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;

            if (transactionsMade >= 4)
            {
                totalAmount = amount + withdrawalFee;
            }

            // if anything is wrong with withdrawal
            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid Amount");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            ATMMethods.Withdrawal(account, id, amount, comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Opens transfer page to choose transfer to own or transfer to third-party
        public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));


        // Opens initial transfer to own page
        public async Task<IActionResult> Transfer_Own(int id) => View(await _context.Accounts.FindAsync(id));


        // Transfer to own account method logic
        [HttpPost]
        public async Task<IActionResult> Transfer_Own(int id, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" && x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;
            int selectedID;

            if (transactionsMade >= 4)
            {
                totalAmount = amount + transferFee;
            }

            if (id % 2 == 0)
            {
                selectedID = id + 1;
            }
            else
            {
                selectedID = id - 1;
            }
            var destAccount = await _context.Accounts.FindAsync(selectedID);

            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid amount.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            ATMMethods.Transfer_Own(account, destAccount, id, selectedID, amount, comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Opens initial transfer to third-party page
        public async Task<IActionResult> Transfer_ThirdParty(int id) => View(await _context.Accounts.FindAsync(id));


        // Transfer to third-party method logic
        [HttpPost]
        public async Task<IActionResult> Transfer_ThirdParty(int id, int destID, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(id);
            const decimal transferFee = 0.2m;
            var totalAmount = amount;
            var notServiceTransactions = account.Transactions.Count(x => x.TransactionType != "S");
            var transfersToAccount = account.Transactions.Count(x => x.TransactionType == "T" && x.DestinationAccountNumber.Equals(null));
            var transactionsMade = notServiceTransactions - transfersToAccount;
            var destAccount = await _context.Accounts.FindAsync(destID);

            if (transactionsMade >= 4)
            {
                totalAmount = amount + transferFee;
            }

            if (destID.ToString().Length != 4 || destAccount == null)
                ModelState.AddModelError(nameof(destID), "Invalid account number.");
            if (amount <= 0 || amount.HasMoreThanTwoDecimalPlaces() || totalAmount > account.Balance)
                ModelState.AddModelError(nameof(amount), "Invalid Amount.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            ATMMethods.Transfer_ThirdParty(account, destAccount, id, destID, amount, comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Opens account selection page before viewing statements
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


        // Opens statement page, using paging list
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


        // Opens profile page
        public async Task<IActionResult> MyProfile()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(customer);
        }


        // Opens page to change password
        [Route("Customers/ChangePasswordView")]
        public async Task<IActionResult> ChangePassword()
        {
            var login = await _context.Logins.FindAsync(UserID);

            return View(login);
        }


        // Opens edit customer page
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


        // logic for edit customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,CustomerName,TFN,Address,City,State,PostCode,Phone,Status")] Customer customer)
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


        // logic for changing password
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

            CustomerMethods.ChangePassword(login, password);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Opens initial page for creating a billpay
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

        // Creating a billpay logic
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
            }

            _context.BillPays.Add(
                new BillPay
                {
                    AccountNumber = accountNumber,
                    PayeeID = payeeID,
                    Amount = amount,
                    ScheduleDate = scheduleDate,
                    Period = period,
                    Status = "Active"
                });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllScheduledPayments));
        }


        // Opens the page for all the scheduled payments by the customer
        public async Task<IActionResult> AllScheduledPayments()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            return View(billPayMethods.AllScheduledPayments(customer));
        }

        // Opens initial page for modifying billpay
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


        // Logic for modifying billpays
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

            billPayMethods.ModifyBillPay(billPay, accountNumber, payeeID, amount, scheduleDate, period);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllScheduledPayments));
        }


        // Logic for deleting billpay
        public async Task<IActionResult> DeleteBillPay(int id)
        {
            var billPay = await _context.BillPays.FindAsync(id);
            _context.BillPays.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllScheduledPayments));
        }


        private bool BillPayExists(int id)
        {
            return _context.BillPays.Any(e => e.BillPayID == id);
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}
