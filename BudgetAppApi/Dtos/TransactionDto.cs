using BudgetAppApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetAppApi.Dtos
{
    public class TransactionDto
    {
        
        public string TransactionName { get; set; }
        
        public decimal TransactionPrice { get; set; }

        public DateOnly TransactionDate { get; set; }

        public string TransactionCategory { get; set; }
    }
}
