using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.RateLimiting;

namespace GerenciadorUsuarios.Api.Controller;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{      
    private readonly string Secret;

    public LoginController(IConfiguration configuration)
    {
        Secret = configuration.GetValue<string>("ChaveAutenticacao");
    }

    /// <summary>
    /// Endpoint responsável por gerar o Token do usuário no modelo JWT
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [EnableRateLimiting("janela-fixa")]
    public IActionResult GerarToken()
    {

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Definicao das claims
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, "usuario@gmail.com"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("ler-dados-por-id", "true")
        };

        var token = new JwtSecurityToken(
            issuer: "usuarios-api", // quem eh o emissor
            audience: "usuarios-api", // quem vai validar o token
            claims: claims, 
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(tokenString);
    } 
}
