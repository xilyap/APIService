using APIService.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIService
{
	public class ApiKeyAuthFilter : IAuthorizationFilter
	{
		private UserContext _userContext;

		public ApiKeyAuthFilter(UserContext context)
		{
			_userContext = context;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			string userApiKey = context.HttpContext.Request.Headers["API-KEY"].ToString();

			if (string.IsNullOrEmpty(userApiKey))
			{
				context.Result = new BadRequestResult();
				return;
			}
			bool isValid = _userContext.Users.Any(item => item.API_KEY == userApiKey);

			if (!isValid)
			{
				context.Result = new UnauthorizedResult();
			}
		}
	}
}