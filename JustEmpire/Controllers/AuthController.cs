using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace JustEmpire.Controllers;

public class AuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly UserRepository _userRepository;
    private readonly RankRepository _rankRepository;
    private readonly UserAccessor _userAccessor;

    public AuthController(IConfiguration configuration, UserRepository userRepository, RankRepository rankRepository,
        UserAccessor userAccessor)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _rankRepository = rankRepository;
        _userAccessor = userAccessor;
    }
    
    /// <summary>
    /// Login endpoint
    /// </summary>
    /// <param name="userData">User model</param>
    /// <returns>JWT Token</returns>
    [HttpPost]
    [LogAction]
    public async Task<ActionResult> Login([FromBody]UserModel userData)
    {
        var user = _userRepository.GetByUsername(userData?.Username) ?? null;
        // If user is not found
        if (user is null) return Unauthorized();
        // If user password is correct
        if (!_userRepository.IsPasswordCorrect(userData.Username, userData.Password)) return Problem();

        var userRole = _rankRepository.GetById(user.RankId);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("Id", user.Id.ToString()),
            new Claim("Name", userData.Username),
            new Claim("RankId", user.RankId.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole.Name)
        };
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));
        
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        
        Log.Information($"IP: {HttpContext.Connection.RemoteIpAddress}; Logged into {user.Username}");
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });
        return Ok(new { token });
    }

    [HttpGet]
    [Authorize]
    [LogAction]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok();
    }

    [HttpGet]
    [Authorize]
    [LogAction]
    public async Task<ActionResult> User()
    {
        string id = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id").Value;
        string username = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Name").Value;
        var rankId = int.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "RankId").Value);
        string rank = _rankRepository.GetById(rankId).Name;

        return Ok(new { id, username, rank });
    }

    [HttpGet]
    [Authorize]
    [LogAction]
    public async Task<ActionResult<Rank>> CurrentRank()
    {
        var rankId = int.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "RankId").Value);
        Rank rank = _rankRepository.GetById(rankId);

        return Ok(rank);
    }
}