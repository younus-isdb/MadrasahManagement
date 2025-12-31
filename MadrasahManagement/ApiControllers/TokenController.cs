using MadrasahManagement.Dto;
using MadrasahManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MadrasahManagement.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public TokenController(UserManager<AppUser> userManager,
                               SignInManager<AppUser> signInManager,
                               IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // ------------------ REGISTER ------------------
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
        {
            if (userDto == null) return BadRequest("Invalid user data");

            var appUser = new AppUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email ?? userDto.UserName,
                FullName = userDto.FullName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(appUser, userDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully" });
        }

        // ------------------ LOGIN ------------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] UserDto userDto)
        {
            if (userDto == null) return BadRequest("Invalid credentials");

            var user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user == null) return Unauthorized("User not found");

            var validPassword = await _userManager.CheckPasswordAsync(user, userDto.Password);
            if (!validPassword) return Unauthorized("Invalid password");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        // ------------------ GET LOGGED IN USER INFO ------------------
        [Authorize]
        [HttpGet("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            var user = HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return Unauthorized();

            var email = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? "";
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var name = user.Identity.Name ?? "";

            return Ok(new
            {
                Name = name,
                Email = email,
                Roles = roles
            });
        }
    }
}
