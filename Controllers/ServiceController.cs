using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yamvc.Core;
using yamvc.Models;

namespace yamvc.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ServiceController : Controller
    {
        public IActionResult Welcome()
        {
            var model = new WelcomeModel
            {
                IsAdmin = HttpContext.User.IsInRole(UserRole.Admin),
                Login = HttpContext.User.Identity.Name
            };

            return View(model);
        }
    }
}