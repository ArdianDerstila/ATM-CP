using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankAtmMVC.Data;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BankAtmMVC.Controllers.TransactionController;

namespace BankAtmMVC.Controllers
{  
    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministratorController(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit1([Bind("Amount")] Transaction transaction)
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



        public async Task<IActionResult> Index(string searchValue)
        {
            var users = from m in _context.AspNetUsers select m;

            if (!String.IsNullOrEmpty(searchValue))
            {
                users = users.Where(s => s.LastName.Contains(searchValue) 
                                      || s.FirstName.Contains(searchValue)
                                      || s.Email.Contains(searchValue)
                                      || s.Id.Contains(searchValue));
            }


            
            return View(await users.ToListAsync());
        }
    }
}