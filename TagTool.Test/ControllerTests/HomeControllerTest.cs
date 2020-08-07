using TagTool.Web.Controllers;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TagTool.Test
{
    public class HomeControllerTest
    {
        private readonly HomeController _Controller;
        private readonly Mock<ILogger<HomeController>> logger;

        public HomeControllerTest(){
            logger = new Mock<ILogger<HomeController>>();
            _Controller = new HomeController(logger.Object);
        }

        [Fact]
        public void HomeController_ActionsShouldReturnViews()
        {
            // Given

            // When
            var About = _Controller.About();
            var Index = _Controller.Index();
            var AdminMenu = _Controller.AdminHome();

            // Then
            Assert.IsType<ViewResult>(About);
            Assert.IsType<ViewResult>(Index);
            Assert.IsType<ViewResult>(AdminMenu);
            
        }
    }
}