﻿using APIService.Model;
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
	public class BankAPIController : ControllerBase
	{
		[HttpGet("/api/quotation/{time}")]
		public async Task<IResult> Get(string time, IHttpClientFactory httpClientFactory, UserContext userContext)
		{
			if (DateTime.TryParseExact(time, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
			{
				if (Request.Headers.TryGetValue("API-KEY", out StringValues ApiKey))
				{
					var val = ApiKey.FirstOrDefault();
					if (val != null && userContext.Users.Any(item => item.API_KEY == val))
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
							return Results.Text(jsonContent, "application/json");
						}
					}
				}
				return Results.Unauthorized();
			}
			return Results.StatusCode(422);
		}
	}
}