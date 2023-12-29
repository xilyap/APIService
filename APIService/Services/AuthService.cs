using APIService.Model;
using APIService.Services.Abstract;
using System.Security.Cryptography;

namespace APIService.Services
{
	public class AuthService : IAuthService, IApiKeyService
	{
		private UserContext _db;

		public AuthService(UserContext db)
		{
			_db = db;
		}

		public bool Register(string login, string password)
		{
			User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

			if (user == null)
			{
				return false;
			}

			_db.Users.Add(new User { Login = login, Password = password });
			_db.SaveChanges();

			return true;
		}

		public bool Login(string login, string password)
		{
			User? user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

			if (user == null)
			{
				return false;
			}
			return true;
		}

		public string? GetApiKey(string? user)
		{
			string result = _db.Users.FirstOrDefault(u => u.Login == user).API_KEY;
			return result;
		}

		public string? ResetApiKey(string? user)
		{
			string newApiKey = GenerateApiKey(32);
			_db.Users.First(u => u.Login == user).API_KEY = newApiKey;
			_db.SaveChangesAsync();
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