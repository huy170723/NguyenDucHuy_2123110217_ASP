using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using NguyenDucHuy_2123110217_ASP.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.Orders) // Include đơn hàng nếu cần
                                     .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return user;
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Validate bắt buộc
            if (string.IsNullOrWhiteSpace(user.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Password is required.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("Email is required.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
                return BadRequest();

            // Validate
            if (string.IsNullOrWhiteSpace(user.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Password is required.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("Email is required.");

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.Orders)
                                     .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            if (user.Orders.Any())
                return BadRequest("Cannot delete user linked to orders.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // ===== AUTH =====
        public class RegisterDto
        {
            public string Name { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string Email { get; set; } = null!;
        }

        public class LoginDto
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginResultDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = null!;
            public string Name { get; set; } = null!;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Username, password and email are required.");

            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Name = dto.Name,
                Username = dto.Username,
                Password = HashPassword(dto.Password),
                Email = dto.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResultDto>> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            var hashed = HashPassword(dto.Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == hashed);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            return new LoginResultDto { UserId = user.UserId, Username = user.Username, Name = user.Name };
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}