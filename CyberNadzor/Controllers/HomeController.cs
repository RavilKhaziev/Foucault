using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CyberNadzor.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CyberNadzor.Controllers;


[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;

    public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> SignInManager)
    {
        _logger = logger;
        _signInManager = SignInManager;
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
        //    // ≈сли пользователь не зарегестрирован
        //    return Redirect("/Identity/Account/Login");
        //}
        return View();
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
