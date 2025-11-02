using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BudgetAppApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public string TransactionName { get; set; }
        [DataType(DataType.Currency)]
        public decimal TransactionPrice { get; set; }

        public User? User { get; set; }

        public int UserId { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? TransactionDate { get; set; }

        public int CategoryId { get; set; }

        public TransactionCategory? Category { get; set; }

    }
}
