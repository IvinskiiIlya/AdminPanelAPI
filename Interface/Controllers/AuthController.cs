using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Application.DTO.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="loginDto">Данные для входа</param>
    /// <returns>JWT-токен</returns>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Аутентификация пользователя",
        Description = "Возвращает JWT-токен для авторизованного доступа к API"
    )]
    [SwaggerResponse(200, "Успешная аутентификация", typeof(JwtTokenResponse))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Неверные учетные данные")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(_configuration.GetValue<double>("Jwt:ExpiryInMinutes")),
            signingCredentials: creds
        );

        return Ok(new JwtTokenResponse 
        { 
            Token = new JwtSecurityTokenHandler().WriteToken(token) 
        });
    }

    /// <summary>
    /// Модель ответа с JWT-токеном
    /// </summary>
    public class JwtTokenResponse
    {
        /// <summary>
        /// Сгенерированный JWT-токен
        /// </summary>
        public string Token { get; set; }
    }
}
