using BudgetAppApi.Data;
using BudgetAppApi.Dtos;
using BudgetAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BudgetAppApiContext _context;

        public TransactionsController(BudgetAppApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.User.FindAsync(int.Parse(id));
            var transaction = await _context.Transaction.Where(t => t.User == user).Select
                (t => new
                {
                    t.TransactionName,
                    t.TransactionPrice,
                    t.TransactionDate,
                    t.Category.CategoryName,
                    t.Id
                }).ToListAsync();
            return Ok(transaction);
        }

        [HttpPost("createtransaction")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transaction)
        {

            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (id == null) return Unauthorized();
            var user = await _context.User.FindAsync(int.Parse(id));

            if (user == null) return NotFound();
            var category = await _context.TransactionCategory.FirstOrDefaultAsync(c => c.CategoryName == transaction.TransactionCategory);
            if (category == null)
            {
                var newcategory = new TransactionCategory { CategoryName = transaction.TransactionCategory, User = user };
                category = newcategory;
            }
            var newtransaction = new Transaction
            {
                TransactionName = transaction.TransactionName,
                TransactionPrice = transaction.TransactionPrice,
                TransactionDate = transaction.TransactionDate,
                Category = category,
                User = user


            };
            _context.Transaction.Add(newtransaction);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Transaction Created" });
        }
        [HttpPost("deletetransaction")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteTransaction([FromBody] int id_to_delete)
        {

            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (id == null) return Unauthorized();
            var user = await _context.User.FindAsync(int.Parse(id));
            if (user == null) return NotFound();

            var action = await _context.Transaction.FirstOrDefaultAsync(t => t.Id == id_to_delete);
            _context.Transaction.Remove(action);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Transaction Deleted" });
        }
    }
}
