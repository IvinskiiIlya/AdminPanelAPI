using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Application.DTO.User;
using Microsoft.AspNetCore.Authorization;
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

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.UserName),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.NameIdentifier, user.Id)
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(_configuration.GetValue<double>("Jwt:ExpiryInMinutes")),
            signingCredentials: creds
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = token.ValidTo
        };
        Response.Cookies.Append("jwtToken", tokenString, cookieOptions);

        return Ok(new { message = "Аутентификация прошла успешно" });
    }
    
    /// <summary>
    /// Разлогирование, удаление токена
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (Request.Cookies.ContainsKey("jwtToken"))
        {
            Response.Cookies.Delete("jwtToken");
        }
        return Ok(new { message = "Выход выполнен" });
    }
    
    /// <summary>
    /// Получение информации о пользователе
    /// </summary>
    [HttpGet("userinfo")]
    [Authorize]
    public IActionResult UserInfo()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        var userName = User.Identity?.Name ?? "";
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new { id = userId, name = userName, roles });
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
