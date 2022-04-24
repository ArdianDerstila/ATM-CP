using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAtmMVC.Models
{
    public enum TransactionType {Deposit, Withdraw};

    public class Transaction
    {
        public int ID { get; set; } 

        public TransactionType Type { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }


        public string BankUserID { get; set; }
        public BankUser AspNetUsers { get; set; }
    }
}
