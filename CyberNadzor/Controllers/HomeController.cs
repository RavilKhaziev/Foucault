using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CyberNadzor.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CyberNadzor.Extensions.AuthExtension;
using CyberNadzor.Extensions;
using CyberNadzor.Seed;
using CyberNadzor.Data;
using Microsoft.EntityFrameworkCore;

namespace CyberNadzor.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AnswerSeed _answerSeed;
    private readonly UserSeed _userSeed;
    private readonly SurveySeed _surveySeed;
    private readonly ApplicationDbContext _db;
    public HomeController(ILogger<HomeController> logger, 
        SignInManager<IdentityUser> SignInManager,
        UserManager<IdentityUser> userManager,
        AnswerSeed answerSeed,
        UserSeed userSeed,
        SurveySeed surveySeed,
        ApplicationDbContext db
        )
    {
        _logger = logger;
        _signInManager = SignInManager;
        _userManager = userManager;
        _answerSeed = answerSeed;
        _userSeed = userSeed;
        _surveySeed = surveySeed;
        _db = db;
    }
    [NonAction]
    public IActionResult Index()
    {
        //if (_signInManager.IsSignedIn(User))
        //{
        //    return View();
        //}
        //else
        //{
        //    // Если пользователь не зарегестрирован
        //    return Redirect("/Identity/Account/Login");
        //}
        return View();
    }
    /// <summary>
    ///  ВНИМАНИЕ это тестовая API при её вызове все данные в БД будут перезаписанны на стандартные данные
    /// </summary>
    /// <returns></returns>
    [HttpGet("/test")]
    public async Task<bool> TestPrupose()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        await _userSeed.SeedUsers();
        await _surveySeed.SeedSurvey();
        await _answerSeed.SeedAnswers(new() { "Анкета студента ФРГФ" });
        return true;
    }
     
    /// <summary>
    /// Входим. Если не указывать Email то будем входить за самого первого пользователя
    /// (Только для тестового использованя!!!!)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>

    [HttpPost("/login")]
    public async Task<IActionResult> Test([FromForm]string? email, [FromForm] string? password)
    {
        IdentityUser? user;
        if (email.IsNullOrEmpty())
        {
            user = await _userManager.Users.FirstOrDefaultAsync();
            await _signInManager.SignInAsync(user, isPersistent: true, IdentityConstants.BearerScheme);
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        else
        {
            user = await _userManager.FindByEmailAsync(email);
        }
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            await _signInManager.SignInAsync(user, isPersistent: true, IdentityConstants.BearerScheme);
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            
            var token = GetToken(authClaims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();

    }
    [NonAction]
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthExtension.AuthOptions.KEY));
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            expires: DateTime.Now.AddDays(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        return token;
    }
    [NonAction]
    public IActionResult Privacy()
    {
        return View();
    }
    [NonAction]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
