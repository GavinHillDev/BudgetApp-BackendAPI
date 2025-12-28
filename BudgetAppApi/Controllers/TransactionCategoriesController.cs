using BudgetAppApi.Data;
using BudgetAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionCategoriesController : ControllerBase
    {
        private readonly BudgetAppApiContext _context;

        public TransactionCategoriesController(BudgetAppApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetCategories()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.User.FindAsync(int.Parse(id));


            var categories = await _context.TransactionCategory.Where(c => c.User == user).Select
                (c => new
                {
                    c.CategoryName
                }).ToListAsync();
            return Ok(categories);
        }
       
    }
}
