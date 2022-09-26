using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MyProfile.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            string UserName;
            string UserIdStr;
            HttpContext.Request.Cookies.TryGetValue("Username", out UserName);
            HttpContext.Request.Cookies.TryGetValue("UserId", out UserIdStr);
            if (UserName != null && UserIdStr != null) {
                return Redirect("/profile");
            }
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
