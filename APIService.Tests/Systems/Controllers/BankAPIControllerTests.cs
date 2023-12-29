using APIService.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIService.Tests.Systems.Controllers
{
	public class BankAPIControllerTests
	{
		public BankAPIControllerTests()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		[Fact]
		public async void Get_OnSuccess_ReturnsJSONCode200()
		{
			//Arrange
			HttpClient client = new HttpClient();
			var clientFactory = new Mock<IHttpClientFactory>();
			clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

			BankAPIController controller = new BankAPIController();
			//Act
			var result = await controller.Get("23-10-1999", clientFactory.Object) as JsonResult;

			//Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async void Get_OnBadRequest_ReturnsCode400()
		{
			//Arrange
			HttpClient client = new HttpClient();
			var clientFactory = new Mock<IHttpClientFactory>();
			clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

			BankAPIController controller = new BankAPIController();
			//Act
			var result = await controller.Get("23 ete-10-2400", clientFactory.Object) as BadRequestResult;

			//Assert
			Assert.NotNull(result);
		}
	}
}