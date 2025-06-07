using Microsoft.AspNetCore.Mvc;
using PWS.Models;
using System.Diagnostics;

namespace PWS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        // https://dotnettutorials.net/lesson/handling-non-success-http-status-codes-in-asp-net-core-mvc/
        // https://www.youtube.com/watch?v=Mk-KHFUyfL8
        /// <summary>
        /// Returns a view based on the HTTP status code / error (ex: 404)
        /// </summary>
        /// <param name="code">HTTP Status Code</param>
        /// <returns></returns>
        [Route("/Home/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            switch (code)
            {
                case 404:
                    return View(
                        "NotFound",
                        new ErrorViewModel
                        {
                            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                            StatusCode = code,
                            ErrorMessage = "Page not found!"
                        });
                    break;

                default:
                    break;
            }

            return View(
                "NotFound",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
                );
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
