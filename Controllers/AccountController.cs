using Bookstore.Models;
using Bookstore.Services;
using Bookstore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Bookstore.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(BaseService baseService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) :base(baseService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel loginVm = new LoginViewModel();
            loginVm.ReturnUrl = returnUrl;
            return View(loginVm);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVm)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByNameAsync(loginVm.Username);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await
                        _signInManager.PasswordSignInAsync(appUser, loginVm.Password, loginVm.Remember, false); // nechci block pri neuspesnem prihlaseni
                    if (result.Succeeded)
                        return Redirect(loginVm.ReturnUrl ?? "/");
                }
                ModelState.AddModelError(nameof(loginVm.Username), "Login Failed: Invalid Username or password. Shame on you.");
            }
            return View(loginVm);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
