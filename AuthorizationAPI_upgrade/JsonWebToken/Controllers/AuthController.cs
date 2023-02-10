using JsonWebToken.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JsonWebToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AuthController : Controller
    {
        private readonly AppDBContext appDbContext;
    

        public AuthController(AppDBContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        // GET api/values  
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(1000);
            List<User> users = appDbContext.GetUsers();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("login/{Aadhar}")]
        public IActionResult GetToken([FromRoute] string Aadhar)
        {  
            List<User> users = appDbContext.GetUsers();
            User user = users.Find(u => u.Aadhar == Aadhar);
            if (user == null)
                return Unauthorized();
            else
            {
                var token = GenerateJSONWebToken(user.Aadhar, user.Name);
                return Ok(token);
            }

        }
        private string GenerateJSONWebToken(string Aadhar, string Name)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysuperdupersecret"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Name),
                new Claim("Aadhar", Aadhar.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "mySystem",
                audience: "myUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
              );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
