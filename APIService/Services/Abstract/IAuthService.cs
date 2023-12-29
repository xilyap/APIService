using APIService.Model;

namespace APIService.Services.Abstract
{
	public interface IAuthService
	{
		public bool Register(string login, string password);

		public bool Login(string login, string password);
	}
}