using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using yamvc.Core.Extensions;
using yamvc.Core.Service;
using yamvc.Models;

namespace yamvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IList<ExtendedUserModel> _usersProvider;

        public AccountController(IOptions<List<ExtendedUserModel>> usersProvider)
        {
            _usersProvider = usersProvider.Value;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromForm]UserModel model)
        {
            if (ModelState.IsValid)
            {
                var principal = ValidateLogin(model);

                if (principal != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                }
            }

            return new RedirectToActionResult("Welcome", "Service", null);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();

            UserManager.Instance.RemoveUser(HttpContext.User.Claims.GetUserLogin());

            return new RedirectToActionResult("Login", "Account", null);
        }

        public ClaimsPrincipal ValidateLogin(UserModel model)
        {
            var user = _usersProvider.FirstOrDefault(x => x.Login.Equals(model.Login, StringComparison.InvariantCultureIgnoreCase) && x.Password.Equals(model.Password));

            if (user == null)
                return null;

            UserManager.Instance.AddUser(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }
    }
}