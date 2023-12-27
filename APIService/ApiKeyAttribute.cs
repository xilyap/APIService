using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIService
{
	public class ApiKeyAttribute : ServiceFilterAttribute
	{
		public ApiKeyAttribute() : base(typeof(ApiKeyAuthFilter))
		{
		}
	}
}