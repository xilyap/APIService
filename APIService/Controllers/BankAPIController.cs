using APIService.Model;
using APIService.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Xml;

namespace APIService.Controllers
{
	[ApiController]
	[ApiKey]
	public class BankAPIController : ControllerBase
	{
		[HttpGet("/api/quotation/{time}")]
		public async Task<IActionResult> Get(string time, IHttpClientFactory httpClientFactory)
		{
			if (DateTime.TryParseExact(time, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
			{
				string link = $@"http://www.cbr.ru/scripts/XML_daily.asp?date_req={result.ToString("dd/MM/yyyy")}";
				var client = httpClientFactory.CreateClient();
				var httpResponseMessage = await client.GetAsync(link);

				if (httpResponseMessage.IsSuccessStatusCode)
				{
					var content = await httpResponseMessage.Content.ReadAsStringAsync();
					var xml = new XmlDocument();
					xml.LoadXml(content);
					var jsonContent = JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.Indented);
					return new JsonResult(jsonContent);
				}

			}
			return BadRequest();
		}
	}
}