namespace APIService.Services.Abstract
{
	public interface IApiKeyService
	{
		public string? GetApiKey(string? user);

		public string? ResetApiKey(string? user);
	}
}