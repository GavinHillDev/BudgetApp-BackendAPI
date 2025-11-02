using System.ComponentModel.DataAnnotations;

namespace BudgetAppApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public ICollection<Transaction>? Transaction { get; set; }

        public ICollection<TransactionCategory>? TransactionCategories { get; set; }
    }
}
