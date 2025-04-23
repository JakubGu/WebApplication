using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication;
using WebApplication.Models.Entities;
using WebApplication.Models.ViewModels;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private AppUserManager _userManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private IAuthenticationManager _authManager => HttpContext.GetOwinContext().Authentication;

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Translations");
                }
                AddErrors(result);
            }

            return View(model);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Translations") : returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                await SignInAsync(user, model.RememberMe);
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Nieprawidłowy login lub hasło.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            _authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Translations");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }
    }
}