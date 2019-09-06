using Microsoft.AspNetCore.Mvc;

namespace TrekkingForCharity.Web.Features.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}