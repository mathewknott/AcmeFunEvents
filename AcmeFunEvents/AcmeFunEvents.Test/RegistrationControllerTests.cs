using AcmeFunEvents.Web.Controllers;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Configuration;
using AcmeFunEvents.Web.Models.Page;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AcmeFunEvents.Test
{
    public class RegistrationControllerTests
    {
        private readonly Mock<IOptions<AppOptions>> _optionsAccessor = new Mock<IOptions<AppOptions>>();
        private readonly Mock<IMemoryCache> _memoryCache = new Mock<IMemoryCache>();
        private readonly Mock<ILogger<RegistrationController>> _logger = new Mock<ILogger<RegistrationController>>();
        private readonly Mock<IRegistrationService> _registrationService = new Mock<IRegistrationService>();
        private readonly Mock<IActivityService> _activityService = new Mock<IActivityService>();
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();

        private readonly RegistrationController _controller;
        
        public RegistrationControllerTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Scratch");

            _controller = new RegistrationController(
                _memoryCache.Object,
                new ApplicationDbContext(optionsBuilder.Options),
                _optionsAccessor.Object,
                _logger.Object,
                _activityService.Object,
                _registrationService.Object,
                _userService.Object)
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
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewName);
            Assert.NotNull(viewResult.ViewData);
            Assert.IsType<BasePageViewModel>(viewResult.ViewData.Model);
        }
    }
}