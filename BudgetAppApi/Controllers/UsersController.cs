using BudgetAppApi.Data;
using BudgetAppApi.Dtos;
using BudgetAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BudgetAppApiContext _context;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UsersController(BudgetAppApiContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("check/{email}")]
        public IActionResult CheckEmail(string email)
        {
            var exists = _context.User.Any(u => u.Email == email);
            return Ok(new { exists });

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (await _context.User.AnyAsync(u => u.Email == register.Email))
                return BadRequest(new { message = "Email already registered" });

            // Hash the password
            var user = new User
            {

                Username = register.Username,
                Email = register.Email,
                PasswordHash = _passwordHasher.HashPassword(null, register.Password)
            };
            

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid credentials" });

            // Generate JWT
            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.User.FindAsync(int.Parse(userId));
            if (user == null) return NotFound();

            return Ok(new { user.Id, user.Username, user.Email });
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }

}
