using APIService.Model;
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
		[HttpPost]
		public async Task<IActionResult> Register(UserContext db)
		{
			var form = HttpContext.Request.Form;
			if (!form.ContainsKey("login") && !form.ContainsKey("password"))
			{
				return BadRequest();
			}

			string login = form["login"];
			string password = form["password"];

			User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
			if (user != null)
			{
				return Ok(new { Status = "Error", Message = "Пользователь уже зарегистрирован!" });
			}

			db.Users.Add(new User { Login = login, Password = password });
			db.SaveChanges();

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
		public async Task<IActionResult> Login(UserContext db)
		{
			var form = HttpContext.Request.Form;
			if (!form.ContainsKey("login") && !form.ContainsKey("password"))
			{
				return BadRequest("Логин или пароль не установлены");
			}

			string login = form["login"];
			string password = form["password"];

			User? user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
			if (user == null)
			{
				return Unauthorized();
			}

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };

			ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
			return Redirect("/");
		}

		[HttpGet]
		[Authorize]
		public IActionResult GetApiKey(UserContext db)
		{
			string user = HttpContext.User.Identity.Name;
			string? API_KEY = db.Users.FirstOrDefault(u => u.Login == user).API_KEY;
			if (API_KEY == null)
			{
				return NotFound();
			}
			return Ok(API_KEY);
		}

		[HttpGet]
		[Authorize]
		public string ResetApiKey(UserContext db)
		{
			string user = HttpContext.User.Identity.Name;
			string newApiKey = GenerateApiKey(32);
			db.Users.First(u => u.Login == user).API_KEY = newApiKey;
			db.SaveChangesAsync();
			return newApiKey;
		}

		private string GenerateApiKey(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			string result = new string(Enumerable.Repeat(chars, length)
		.Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
			return result;
		}
	}
}