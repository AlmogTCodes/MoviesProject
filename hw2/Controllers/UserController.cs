using hw2.Models;
using hw2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations; // Required for DTO validation attributes

namespace hw2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PasswordHashingService _passwordHashingService; // Re-add service field
        private readonly AuthTokenService _authTokenService; // Add AuthTokenService dependency

        // Update constructor to inject both services
        public UserController(PasswordHashingService passwordHashingService, AuthTokenService authTokenService)
        {
            _passwordHashingService = passwordHashingService;
            _authTokenService = authTokenService; // Assign injected service
        }

        // DTO for Login Request (Copied from AuthController)
        public class LoginRequestDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> GetAllUsers()
        {
            return Models.User.Read();
        }

        // GET api/<UserController>/{id}
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public ActionResult<object> Post([FromBody] User user) // Changed return type to match AuthController for consistency
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Hash the password before attempting to insert
            try
            {
                user.Password = _passwordHashingService.CreatePasswordHash(user.Password);
            }
            catch (ArgumentException ex)
            {
                // Handle cases like empty password from the hashing service
                return BadRequest(new { message = ex.Message });
            }

            // Ensure ID is 0 for insertion logic in User.Insert
            user.Id = 0;

            bool inserted = Models.User.Insert(user);

            if (!inserted)
            {
                
                // Check if the reason was duplicate email
                if (Models.User.UsersList.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return Conflict(new { message = $"Email '{user.Email}' is already registered." });
                }
                // Other potential insertion issues
                return BadRequest(new { message = "User registration failed." });
            }

            // Return the created user details (excluding password hash)
            return Ok(new { user.Id, user.Name, user.Email, user.Active });
        }

        // POST: api/User/Login (Moved from AuthController)
        [HttpPost("login")]
        public ActionResult<object> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by email (case-insensitive comparison is good practice)
            var user = Models.User.UsersList.FirstOrDefault(u => u.Email.Equals(loginRequest.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." }); // User not found
            }

            // Verify the password
            bool isPasswordValid = _passwordHashingService.VerifyPasswordHash(loginRequest.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid email or password." }); // Password incorrect
            }

            // Generate token using the injected service
            string token = _authTokenService.GenerateToken(user);

            // Return the token (and potentially some user info)
            return Ok(new { token = token, userId = user.Id, userName = user.Name, userEmail = user.Email });
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
