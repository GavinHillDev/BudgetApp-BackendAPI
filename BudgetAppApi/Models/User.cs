using System.ComponentModel.DataAnnotations;

namespace BudgetAppApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public required string Username { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }

        public ICollection<TransactionCategory>? TransactionCategories { get; set; }
    }
}
