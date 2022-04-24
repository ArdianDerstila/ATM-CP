using System;
using System.Collections.Generic;
using System.Text;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankAtmMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<BankUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
          
        public DbSet<BankUser> AspNetUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
