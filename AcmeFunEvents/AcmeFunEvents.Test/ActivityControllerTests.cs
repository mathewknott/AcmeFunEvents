using AcmeFunEvents.Web.Controllers;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Configuration;
using AcmeFunEvents.Web.Models.Page;
using Microsoft.AspNetCore.Hosting;
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
    public class ActivityControllerTests
    {
        private readonly Mock<IOptions<AppOptions>> _optionsAccessor = new Mock<IOptions<AppOptions>>();
        private readonly Mock<IMemoryCache> _memoryCache = new Mock<IMemoryCache>();
        private readonly Mock<IEmailSender> _emailSender = new Mock<IEmailSender>();
        private readonly Mock<IHostingEnvironment> _hosting = new Mock<IHostingEnvironment>();
        private readonly Mock<ILogger<ActivityController>> _logger = new Mock<ILogger<ActivityController>>();
        private readonly Mock<IRegistrationService> _registrationService = new Mock<IRegistrationService>();
        private readonly Mock<IActivityService> _activityService = new Mock<IActivityService>();
        private readonly ActivityController _controller;
        
        public ActivityControllerTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Scratch");

            _controller = new ActivityController(
                _hosting.Object,
                _memoryCache.Object,
                _emailSender.Object,
                new ApplicationDbContext(optionsBuilder.Options),
                _optionsAccessor.Object,
                _logger.Object,
                _activityService.Object,
                _registrationService.Object)
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