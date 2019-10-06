using AcmeFunEvents.Test.Attributes;
using AcmeFunEvents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AcmeFunEvents.Test
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _logger = new Mock<ILogger<HomeController>>();
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController(_logger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public void Index_CanVisit()
        {
            var result = _controller.Index();
            Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [JsonFileData("data.json", "HostData")]
        public void SetLanguage_ReturnsResult(string culture, string returnUrl)
        {
            var result = _controller.SetLanguage(culture, returnUrl);
            Assert.NotNull(result);
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(400)]
        [InlineData(404)]
        [InlineData(0)]
        public void Error_ReturnsResult(int? statusCode)
        {
            var result = _controller.Error(statusCode);
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}