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
            var bankUser = _context.AspNetUsers.Where(i => i.Id == currentId).First();

            try
            {

                if (ModelState.IsValid)
                {
                    bankUser.Balance += Input.Amount;
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
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
            int a = 0;
            int b = 0;
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

                            if (Input.Amount >= 500)
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

                                    ModelState.AddModelError("", "You do not have enough funds " +
                            "");
                                }
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



