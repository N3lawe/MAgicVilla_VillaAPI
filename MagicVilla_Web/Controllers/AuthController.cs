using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers;
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginRequestDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDTO obj)
    {
        if (!ModelState.IsValid)
            return View(obj);

        APIResponse response = await _authService.LoginAsync<APIResponse>(obj);

        if (response != null && response.IsSuccess)
        {
            var modelJson = Convert.ToString(response.Result);
            if (string.IsNullOrEmpty(modelJson))
            {
                ModelState.AddModelError("CustomError", "Login failed: Empty token.");
                return View(obj);
            }

            LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(modelJson);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "name");
            var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "role");

            if (nameClaim != null)
                identity.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value));

            if (roleClaim != null)
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString(SD.SessionToken, model.Token);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            var errorMsg = response?.ErrorMessages?.FirstOrDefault() ?? "Login failed.";
            ModelState.AddModelError("CustomError", errorMsg);
            return View(obj);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterationRequestDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterationRequestDTO obj)
    {
        if (!ModelState.IsValid)
            return View(obj);

        APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);

        if (result != null && result.IsSuccess)
        {
            return RedirectToAction("Login");
        }
        else
        {
            var errorMsg = result?.ErrorMessages?.FirstOrDefault() ?? "Registration failed.";
            ModelState.AddModelError("CustomError", errorMsg);
            return View(obj);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.Remove(SD.SessionToken);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
