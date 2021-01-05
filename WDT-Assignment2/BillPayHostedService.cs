 using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using WDT_Assignment2.Data;
using WDT_Assignment2.Models;
using Microsoft.EntityFrameworkCore;

namespace WDT_Assignment2
{
    public class BillPayHostedService : IHostedService
    {
        private int executionCount = 0;
        private readonly ILogger<BillPayHostedService> _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopedFactory;
        private Task task;

        public BillPayHostedService(ILogger<BillPayHostedService> logger, IServiceScopeFactory scopedFactory)
        {
            _logger = logger;
            _scopedFactory = scopedFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BillPay Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            task = PayBillPays();
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("BillPay Hosted Service is working. Count: {Count}", count);
        }

        public async Task PayBillPays()
        {
            using (var scope = _scopedFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NwbaContext>();

                var bills = await dbContext.BillPays.Where(x => x.ScheduleDate < DateTime.UtcNow).ToListAsync();

                foreach (var billPay in bills)
                {
                    var account = await dbContext.Accounts.FindAsync(billPay.AccountNumber);

                    if (billPay.ScheduleDate <= DateTime.Today && billPay.Status != "Blocked")
                    {
                        if (account.Balance >= billPay.Amount)
                        {
                            account.Balance -= billPay.Amount;
                            account.Transactions.Add(
                                new Transaction
                                {
                                    TransactionType = "B",
                                    Amount = billPay.Amount,
                                    //DestinationAccountNumber = billPay.PayeeID,
                                    Comment = "Scheduled payment to " + billPay.PayeeID,
                                    ModifyDate = DateTime.UtcNow
                                });

                            if (billPay.Period == "S")
                            {
                                dbContext.BillPays.Remove(billPay);
                            }

                            if (billPay.Period == "M")
                            {
                                //billPay.ScheduleDate.AddMonths(1);
                                billPay.ScheduleDate = billPay.ScheduleDate.AddMonths(1);
                            }
                            else if (billPay.Period == "Q")
                            {
                                billPay.ScheduleDate.AddMonths(4);
                            }
                            else if (billPay.Period == "Y")
                            {
                                billPay.ScheduleDate.AddYears(1);
                            }
                        }
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BillPay Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
