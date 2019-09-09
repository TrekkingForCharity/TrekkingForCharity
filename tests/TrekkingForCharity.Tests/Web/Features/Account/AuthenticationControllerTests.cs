using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TrekkingForCharity.Web.Features.Account;
using Xunit;

namespace TrekkingForCharity.Tests.Web.Features.Account
{
    public class AuthenticationControllerTests
    {
        [Fact]
        public async Task Login_GivenDefinedRoute_ExpectChallengeWithPath()
        {
            var controller = new AuthenticationController { ControllerContext = new ControllerContext() };

            var redirectUrl = string.Empty;

            var auth = new Mock<IAuthenticationService>();
            auth.Setup(x =>
                    x.ChallengeAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Callback((HttpContext context, string scheme, AuthenticationProperties props) =>
                {
                    redirectUrl = props.RedirectUri;
                });
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.RequestServices)
                .Returns(CreateServices().AddSingleton(auth.Object).BuildServiceProvider());

            controller.ControllerContext.HttpContext = httpContext.Object;

            var routeData = new RouteData();
            routeData.Routers.Add(Mock.Of<IRouter>());

            await controller.Login("/some-route");

            Assert.Equal("/some-route", redirectUrl);
        }

        [Fact]
        public async Task Login_GivenNoRoute_ExpectChallengeWithDefault()
        {
            var controller = new AuthenticationController { ControllerContext = new ControllerContext() };

            var redirectUrl = string.Empty;

            var auth = new Mock<IAuthenticationService>();
            auth.Setup(x =>
                    x.ChallengeAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Callback((HttpContext context, string scheme, AuthenticationProperties props) =>
                {
                    redirectUrl = props.RedirectUri;
                });
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.RequestServices)
                .Returns(CreateServices().AddSingleton(auth.Object).BuildServiceProvider());

            controller.ControllerContext.HttpContext = httpContext.Object;

            var routeData = new RouteData();
            routeData.Routers.Add(Mock.Of<IRouter>());

            await controller.Login();

            Assert.Equal("/", redirectUrl);
        }

        [Fact]
        public async Task Logout()
        {
            var controller = new AuthenticationController { ControllerContext = new ControllerContext() };

            var callbackCount = 0;

            var auth = new Mock<IAuthenticationService>();
            auth.Setup(x =>
                    x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Callback((HttpContext context, string scheme, AuthenticationProperties props) =>
                {
                    callbackCount++;
                });
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.RequestServices)
                .Returns(CreateServices().AddSingleton(auth.Object).BuildServiceProvider());

            controller.ControllerContext.HttpContext = httpContext.Object;
            var urlHelper = new Mock<IUrlHelper>();
            controller.Url = urlHelper.Object;

            var routeData = new RouteData();
            routeData.Routers.Add(Mock.Of<IRouter>());

            await controller.Logout();

            Assert.Equal(2, callbackCount);
        }

        private static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            services.AddAuthenticationCore();
            return services;
        }
    }
}