using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using my_eshop_api.Helpers;
using my_eshop_api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/users")]
    [EnableCors("my_eshop_AllowSpecificOrigins")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ItemContext Context;
        private readonly AppSettings AppSettings;

        public UserController(ItemContext context, IOptions<AppSettings> appSettings)
        {
            Context = context;
            AppSettings = appSettings.Value;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User formParams)
        {
            var user = await Context.Users.SingleOrDefaultAsync(x => x.Username == formParams.Username);

            if (user == null)
                return BadRequest(new { message = "Log in failed" });

            if (!PasswordHasher.VerifyPassword(formParams.Password, user.Password))
                return BadRequest(new { message = "Log in failed" });

            user.Token = CreateToken(user);
            user.RefreshToken = CreateRefreshToken();
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);
            Context.SaveChanges();

            user.Password = null;

            return Ok(user);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] User data)
        {
            var user = await Context.Users.SingleOrDefaultAsync(u => (u.RefreshToken == data.RefreshToken) && (u.Token == data.Token));

            if (user == null || DateTime.Now > user.RefreshTokenExpiry)
                return BadRequest(new { message = "Invalid token" });

            user.Token = CreateToken(user);
            user.RefreshToken = CreateRefreshToken();
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);
            Context.SaveChanges();

            user.Password = null;

            return Ok(user);
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] User data)
        {
            var user = await Context.Users.SingleOrDefaultAsync(u => (u.RefreshToken == data.RefreshToken));

            if (user == null || DateTime.Now > user.RefreshTokenExpiry)
                return BadRequest(new { message = "Invalid token" });

            user.Token = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            Context.SaveChanges();

            user.Password = null;

            return Ok(user);
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return await Context.Users
                .Select(x => new User()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Username = x.Username,
                    Password = null,
                    Role = x.Role,
                    Email = x.Email,
                    Token = null,
                    RefreshToken = null,
                    RefreshTokenExpiry = DateTime.Now
                })
                .ToListAsync();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            //TODO: return users without passwords????
            return await Context.Users.FindAsync(id);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (await Context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists");
            }

            user.Role = "customer";
            user.Password = PasswordHasher.HashPassword(user.Password);

            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        private string CreateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, user.Role)
                });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        private string CreateRefreshToken()
        {
            var randomNum = new byte[64];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNum);
                return Convert.ToBase64String(randomNum);
            }
        }

    }
}
