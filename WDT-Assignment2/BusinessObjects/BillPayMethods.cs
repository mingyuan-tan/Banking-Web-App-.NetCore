using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WDT_Assignment2.Models;

namespace WDT_Assignment2.BusinessObjects
{
    public class BillPayMethods
    {
        // Opens the page for all the scheduled payments by the customer
        public List<BillPay> AllScheduledPayments(Customer customer)
        {
            List<BillPay> BillPays = new List<BillPay>();

            foreach (var a in customer.Accounts)
            {
                foreach (var b in a.BillPays)
                {
                    BillPays.Add(b);
                }
            }

            return BillPays;
        }

        // Logic for modifying billpays
        public void ModifyBillPay(BillPay billPay, int accountNumber, int payeeID, decimal amount, DateTime scheduleDate, string period)
        {
            billPay.AccountNumber = accountNumber;
            billPay.PayeeID = payeeID;
            billPay.Amount = amount;
            billPay.ScheduleDate = scheduleDate;
            billPay.Period = period;
        }
    }
}
