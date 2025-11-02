using System.ComponentModel.DataAnnotations;

namespace BudgetAppApi.Models
{
    public class TransactionCategory
    {
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public string? CategoryName { get; set; }

        public User? User { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
