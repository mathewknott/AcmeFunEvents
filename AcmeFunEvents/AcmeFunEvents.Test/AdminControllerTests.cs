using AcmeFunEvents.Web.Controllers;
using AcmeFunEvents.Web.Data;
using AcmeFunEvents.Web.Interfaces;
using AcmeFunEvents.Web.Models.Page;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace AcmeFunEvents.Test
{
    public class AdminControllerTests 
    {
        private readonly Mock<IMemoryCache> _memoryCache = new Mock<IMemoryCache>();
        private readonly Mock<IUrlHelper> _urlHelper = new Mock<IUrlHelper>();
        private readonly Mock<IRegistrationService> _registrationService = new Mock<IRegistrationService>();
        private readonly Mock<IActivityService> _activityService = new Mock<IActivityService>();
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();

        private readonly AdminController _controller;
        
        public AdminControllerTests()
        {
           
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Scratch");
            
            _controller = new AdminController(
                _memoryCache.Object,
                _activityService.Object,
                _registrationService.Object,
                _userService.Object,
                new ApplicationDbContext(optionsBuilder.Options)
                )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            _urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost");
            _controller.Url = _urlHelper.Object;
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

        [Fact]
        public void Index_CanSeed()
        {
            var result = _controller.Seed();
            var viewResult = Assert.IsType<ContentResult>(result);
            Assert.NotNull(viewResult.Content);
        }
    }
}