using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BudgetAppApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public required string TransactionName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [DataType(DataType.Currency)]
        public required decimal TransactionPrice { get; set; }

        public User? User { get; set; }

        public int UserId { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? TransactionDate { get; set; }

        public int CategoryId { get; set; }

        public TransactionCategory? Category { get; set; }

    }
}
