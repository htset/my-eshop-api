using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using my_eshop_api.Helpers;
using my_eshop_api.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
namespace my_eshop_api.Controllers
{
    [Route("api/users")]
    [EnableCors("my_eshop_AllowSpecificOrigins")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ItemContext Context;
        private readonly string Secret = "this is a very long string to be used as secret";

        public UserController(ItemContext context)
        {
            Context = context;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User formParams)
        {
            var user = Context.Users.SingleOrDefault(x => x.Username == formParams.Username);

            if (user == null)
                return BadRequest(new { message = "Log in failed" });

            if (!PasswordHasher.VerifyPassword(formParams.Password, user.Password))
                return BadRequest(new { message = "Log in failed" });

            user.Token = CreateToken(user);
            user.Password = null;

            return Ok(user);
        }

        private string CreateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(120),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
