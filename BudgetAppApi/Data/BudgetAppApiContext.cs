using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BudgetAppApi.Models;

namespace BudgetAppApi.Data
{
    public class BudgetAppApiContext : DbContext
    {
        public BudgetAppApiContext (DbContextOptions<BudgetAppApiContext> options)
            : base(options)
        {
        }

        public DbSet<BudgetAppApi.Models.Transaction> Transaction { get; set; } = default!;
        public DbSet<BudgetAppApi.Models.TransactionCategory> TransactionCategory { get; set; } = default!;
        public DbSet<BudgetAppApi.Models.User> User { get; set; } = default!;
    }
}
