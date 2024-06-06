using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Jwttest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		IConfiguration configuration;

		public AuthController(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Auth([FromBody] User user)
		{
			IActionResult response = Unauthorized();

			if(user != null)
			{
				var issuer = configuration["Jwt:Issuer"];
				var audience = configuration["Jwt:Audience"];
				var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
				var signingCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha512Signature
					);

				var subject = new ClaimsIdentity(new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Uid),
					new Claim(JwtRegisteredClaimNames.Email, user.Pass),
				});

				var expires = DateTime.UtcNow.AddMinutes(10);

				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = subject,
					Expires = expires,
					Issuer = issuer,
					Audience = audience,
					SigningCredentials = signingCredentials
				};

				var tokenHandler = new JwtSecurityTokenHandler();
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var jwtToken = tokenHandler.WriteToken(token);

				return Ok(jwtToken);
			}
			return response;
		}
		

	}
}
