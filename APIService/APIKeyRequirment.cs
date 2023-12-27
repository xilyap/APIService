using Microsoft.AspNetCore.Authorization;

namespace APIService
{
	public class APIKeyRequirment : AuthorizationHandler<APIKeyRequirment>, IAuthorizationRequirement
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, APIKeyRequirment requirement)
		{
			throw new NotImplementedException();
		}
	}
}