using APIService.Model;
using APIService.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIService.Controllers
{
	[Route("[controller]/[action]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private IAuthService _authService;
		private IApiKeyService _apiService;

		public AccountController(IAuthService authService, IApiKeyService apiService)
		{
			_authService = authService;
			_apiService = apiService;
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromForm] string login, [FromForm] string password)
		{
			if (_authService.Register(login, password))
			{
				return Ok(new { Status = "Error", Message = "Пользователь уже существует!" });
			}

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
			ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
			return Ok(new { Status = "Success", Message = "Пользователь успешно зарегистрирован!" });
		}

		[HttpGet]
		public async Task Login()
		{
			HttpContext.Response.ContentType = "text/html; charset=utf-8";

			string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Login</label><br />
                <input name='login' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
			await Response.WriteAsync(loginForm);
		}

		[HttpPost]
		public async Task<IActionResult> Login([FromForm] string login, [FromForm] string password)
		{
			if (_authService.Login(login, password))
			{
				return Ok(new { Status = "Error", Message = "Пользователь уже существует!" });
			}

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };

			ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
			return Redirect("/");
		}

		[HttpGet]
		[Authorize]
		public IActionResult GetApiKey()
		{
			string user = HttpContext.User.Identity.Name;
			string? API_KEY = _apiService.GetApiKey(user);
			if (API_KEY == null)
			{
				return NotFound();
			}
			return Ok(API_KEY);
		}

		[HttpGet]
		[Authorize]
		public string ResetApiKey()
		{
			string user = HttpContext.User.Identity.Name;
			string newApiKey = _apiService.ResetApiKey(user);
			return newApiKey;
		}
	}
}