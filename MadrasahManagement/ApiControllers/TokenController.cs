using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MadrasahManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public TokenController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // =======================
        // REGISTER
        // =======================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
                return BadRequest("Email already registered");

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email ?? model.UserName,
                FullName = model.FullName,
                IsStudent = true,   // default role flags
                UserType = "Student"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Ensure Student role exists
            string defaultRole = "Student";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
                await _roleManager.CreateAsync(new AppRole(defaultRole));

            await _userManager.AddToRoleAsync(user, defaultRole);

            return Ok(new { message = "Registration successful" });
        }

        // =======================
        // LOGIN
        // =======================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user, roles);

            return Ok(new
            {
                token,
                userName = user.UserName,
                fullName = user.FullName,
                email = user.Email,
                userType = user.UserType,
                profilePicture = user.ProfilePicture,
                roles,
            });
        }

        // =======================
        // HELPER: Generate JWT
        // =======================
        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id),
                new Claim("fullName", user.FullName ?? ""),
                new Claim("userType", user.UserType ?? "Student")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
