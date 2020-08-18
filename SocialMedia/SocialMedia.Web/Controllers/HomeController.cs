namespace SocialMedia.Web.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SocialMedia.Web.Models;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoggerInformation(Request.GetDisplayUrl());

            //It is requered becuase the browser cache last records
            TempData.Clear();
            return View();
        }
        
        public IActionResult Privacy()
        {
            LoggerInformation(Request.GetDisplayUrl());
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void LoggerInformation(string url)
        {
            this._logger.LogInformation(url);
        }
    }
}
