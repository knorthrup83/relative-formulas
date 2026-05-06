using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RelativeFormulas.Application.Services;
using RelativeFormulas.Domain.Entities;

namespace RelativeFormulas.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string displayName, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "All fields are required.";
            return View();
        }

        var success = await _accountService.RegisterAsync(email, displayName, password);
        if (!success)
        {
            ViewBag.Error = "An account with that email already exists.";
            return View();
        }

        var user = (await _accountService.ValidateCredentialsAsync(email, password))!;
        await SignInAsync(user);
        return RedirectToAction("Index", "Recipes");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        var user = await _accountService.ValidateCredentialsAsync(email, password);
        if (user is null)
        {
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        await SignInAsync(user);
        return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/recipes" : returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Recipes");
    }

    private async Task SignInAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new("isAdmin", user.IsAdmin.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
