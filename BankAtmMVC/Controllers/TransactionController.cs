using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankAtmMVC.Data;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Data.SqlClient;

namespace BankAtmMVC.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;


        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }



        //GET: Transaction/Deposit
        public IActionResult Deposit()
        {
            return View();
        }

        //POST: Transaction/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit([Bind("Amount")] Transaction transaction)
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            transaction.BankUserID = currentId;
            transaction.Date = DateTime.UtcNow;
            transaction.Type = TransactionType.Deposit;
           var total= transaction.Amount;
           
            var bankUser = _context.AspNetUsers.Where(i => i.Id == currentId).First();
            
            try
            {
                //SqlConnection conn = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=Atm1;Integrated Security=true ;MultipleActiveResultSets=true");
                //conn.Open();
                //SqlCommand comm = new SqlCommand("select COUNT(Kart_1000) from[Atm1].[dbo].[Transactions] as test where Kart_1000 = 1000.00", conn);
                //Int32 count = (Int32)comm.ExecuteScalar();
                
                var count1 = _context.Transactions.Count(p => p.Kart_500 == 500);
                var count2 = _context.Transactions.Count(p => p.Kart_1000 == 1000);
                var count3 = _context.Transactions.Count(p => p.Kart_2000 == 2000);
                var count4 = _context.Transactions.Count(p => p.Kart_5000 == 5000);
                int d1 = count1;
                if (Input.Amount == 500) 
                { 
                    Input.Kart_500 = transaction.Kart_500 =total ;
                    Input.Kart_1000 = 0;
                    Input.Kart_2000 = 0;
                    Input.Kart_5000 = 0;
                    await _context.SaveChangesAsync();
                }
                else if(Input.Amount == 1000) 
                {
                    Input.Kart_500 = 0;
                    Input.Kart_1000 = transaction.Kart_1000=total;
                    Input.Kart_2000 = 0;
                    Input.Kart_5000 = 0;
                    await _context.SaveChangesAsync();
                }
                else if (Input.Amount == 2000)
                {
                    Input.Kart_500 =0;
                    Input.Kart_1000 = 0;
                    Input.Kart_2000 = transaction.Kart_2000=total;
                    Input.Kart_5000 = 0;
                    await _context.SaveChangesAsync();
                }
                else if (Input.Amount == 5000)
                {
                    Input.Kart_500 = 0;
                    Input.Kart_1000 = 0;
                    Input.Kart_2000 = 0;
                    Input.Kart_5000 = transaction.Kart_5000=total;
                    await _context.SaveChangesAsync();
                }

                




                
                    //Input.Kart_500 = Input.Amount;
                    //await _context.SaveChangesAsync();
                    //_context.Transactions.Add(transaction);

                    if (ModelState.IsValid)
                    {
                        bankUser.Balance += Input.Amount;
                        _context.Transactions.Add(transaction);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                
                else
                {
                    if (ModelState.IsValid)
                    {
                        bankUser.Balance += Input.Amount;
                        _context.Transactions.Add(transaction);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                }

               
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to complete the transaction. " +
                "Try again, and if the problem persists " +
               "see your system administrator.");
            }

            return View(transaction);
        }

        //GET: Transaction/Withdraw
        public IActionResult Withdraw()
        {
            return View();
        }


        private bool isMultipleof5(int n)
        {
            while (n > 0)
                n = n - 500;

            if (n == 0)
                return true;

            return false;
        }



        //POST: Transaction/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw([Bind("Amount")] Transaction transaction)
        {
            int n;
            int min = 500;
            int max = 50000;
            bool t = true;
            bool f = false;
            bool isMultiple;

            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            transaction.BankUserID = currentId;
            transaction.Date = DateTime.UtcNow;
            transaction.Type = TransactionType.Withdraw;
            var bankUser = _context.AspNetUsers.Where(i => i.Id == currentId).First();

            try
            {
                n = (int)Input.Amount;

                if (bankUser.Balance == 0)
                {
                    bankUser.Balance = bankUser.Balance;

                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", "You do not have enough funds " +
                        "");
                }
                else
                {

                    if (isMultipleof5(n) == true)
                    {
                        if (bankUser.Balance >= Input.Amount)
                        {

                            if (Input.Amount >= min )
                            {
                                if (Input.Amount <= max)
                                {
                                    //if (Input.Amount==500 || Input.Amount == 1000 || Input.Amount == 2000|| Input.Amount == 5000) { }
                                    if (ModelState.IsValid && (bankUser.Balance - Input.Amount) >= 0)
                                    {

                                        bankUser.Balance -= Input.Amount;

                                        _context.Transactions.Add(transaction);
                                        await _context.SaveChangesAsync();
                                        ModelState.AddModelError("", "Sucsessfully transaction. ");
                                        return RedirectToAction("Index", "Home");


                                    }
                                    else
                                    {

                                        ModelState.AddModelError("", "You do not have enough funds " + "");
                                    }
                                }
                                else { ModelState.AddModelError("", "Unable to complete the transaction. " + "The minimum withdrawal is 50000"); }
                                
                            }
                            else
                            {
                                ModelState.AddModelError("", "Unable to complete the transaction. " +
                        "The minimum withdrawal is 500");
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", "Unable to complete the transaction. " +
          "You have less funds in your account than the required value, Try again");


                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to complete the transaction. " +
         "You have to set amounts that are multiples of 500");

                    }
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to complete the transaction. " +
                "Try again, and if the problem persists " +
               "see your system administrator.");
            }

            return View(transaction);
        }



        //GET: Transaction/PersonalTransactions
        public async Task<IActionResult> PersonalTransactions()
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userTransactions = await _context.AspNetUsers
                                .Include(s => s.Transactions)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == currentId);

            UtcToLocalDate(userTransactions);

            return View(userTransactions);
        }

        //maybe should be a Post call so id doesn't show up in URL
        //GET: Transaction/PersonalTransactions/id
        [Route("{id}")]
        public async Task<IActionResult> PersonalTransactions(string id)
        {
            var currentId = id;

            var userTransactions = await _context.AspNetUsers
                                .Include(s => s.Transactions)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == currentId);

            UtcToLocalDate(userTransactions);

            return View(userTransactions);
        }

        public class InputModel : Transaction { }

        public void UtcToLocalDate(BankUser user)
        {
            DateTime utcToLocal;

            foreach (var transaction in user.Transactions)
            {
                utcToLocal = transaction.Date.ToLocalTime();
                transaction.Date = utcToLocal;

            }
        }

    }
}



