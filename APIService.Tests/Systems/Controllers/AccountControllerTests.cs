using APIService.Controllers;
using APIService.Model;
using APIService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Moq;
using Moq.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace APIService.Tests.Systems.Controllers
{
	public class AccountControllerTests
	{
		private readonly HttpContext _httpContext;
		private readonly AccountController _controller;
		private readonly Mock<UserContext> _mockContext;

		public AccountControllerTests()
		{
			var mockSet = new List<User> {
				new User(){Login="test", Password="test", API_KEY = "aaa"  },
			};

			_mockContext = new Mock<UserContext>();

			_mockContext.Setup(m => m.Users).ReturnsDbSet(mockSet);
			var authService = new AuthService(_mockContext.Object);
			_controller = new AccountController(authService, authService);

			_httpContext = new DefaultHttpContext();

			var authServiceMock = new Mock<IAuthenticationService>();
			authServiceMock
				.Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
				.Returns(Task.FromResult((object)null));

			var services = new ServiceCollection();
			services.AddSingleton<IAuthenticationService>(authServiceMock.Object);
			_httpContext.RequestServices = services.BuildServiceProvider();
			_controller.ControllerContext.HttpContext = _httpContext;

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };

			ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies");

			_httpContext.User = new ClaimsPrincipal(identity);
		}

		[Fact]
		public async void Post_Register_OnSuccess_ReturnsStatusCode200Success()
		{
			// Arrange

			// Act
			var result = await _controller.Register(login: "newTest", password: "test");
			var okResult = result as OkObjectResult;
			var options = new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
			var expectedValue = JsonSerializer.Serialize(new { Status = "Success", Message = "Пользователь успешно зарегистрирован!" }, options);
			var actualValue = JsonSerializer.Serialize(okResult.Value, options);

			// Assert
			Assert.NotNull(okResult);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public async void Post_Register_OnFailure_ReturnsStatusCode200Failure()
		{
			// Arrange
			var result = await _controller.Register(login: "test", password: "test");
			var okResult = result as OkObjectResult;
			var options = new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

			// Act

			var actualValue = JsonSerializer.Serialize(okResult.Value, options);
			var expectedValue = JsonSerializer.Serialize(new { Status = "Error", Message = "Пользователь уже существует!" }, options);

			// Assert
			Assert.NotNull(okResult);
			Assert.Equal(200, okResult.StatusCode);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void Get_GetApiKey_Returns_ApiKey()
		{
			// Arrange
			var expectedAPIKey = _mockContext.Object.Users.First().API_KEY;

			// Act
			var result = _controller.GetApiKey() as OkObjectResult;

			// Assert
			Assert.Equal(expectedAPIKey, result.Value);
		}

		[Fact]
		public void Get_GetApiKey_Returns_NotFoundIfEmpty()
		{
			// Arrange
			_mockContext.Object.Users.First().API_KEY = null;

			// Act
			var result = _controller.GetApiKey() as NotFoundObjectResult;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void Get_ResetApiKey_Returns_NewApiKey()
		{
			// Arrange
			var oldAPIKey = _mockContext.Object.Users.First().API_KEY;

			// Act
			var result = _controller.ResetApiKey();

			// Assert
			Assert.NotEqual(oldAPIKey, result);
		}
	}
}