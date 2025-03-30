using Bookstore.Models;
using Bookstore.Services;
using Bookstore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private UserManager<AppUser> _userManager;
        private IPasswordHasher<AppUser> _passwordHasher;
        private IPasswordValidator<AppUser> _passwordValidator;
        public UsersController(BaseService baseService, UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) : base(baseService)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser newUser = new AppUser()
                {
                    UserName = userViewModel.Name,
                    Email = userViewModel.Email
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, userViewModel.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrors(result);
            }
            return View(userViewModel);
        }
        public async Task<IActionResult> Edit(string id)
        {
            AppUser? userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit == null)
            {
                return View("NotFound");
            }
            return View(userToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, string email, string password)
        {
            AppUser? userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit != null)
            {
                IdentityResult validPass;
                if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
                {
                    userToEdit.Email = email;
                    validPass = await _passwordValidator.ValidateAsync(_userManager, userToEdit, password);
                    if (validPass.Succeeded)
                    {
                        userToEdit.PasswordHash = _passwordHasher.HashPassword(userToEdit, password);
                        IdentityResult identityResult = await _userManager.UpdateAsync(userToEdit);
                        if (identityResult.Succeeded)
                            return RedirectToAction("Index");
                        else
                            AddErrors(identityResult);
                    }
                    else
                        AddErrors(validPass);
                }
            }
            else
                ModelState.AddModelError("", "User not found");
            return View(userToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser? userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null)
            {
                var result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrors(result);
            }
            else
                ModelState.AddModelError("", "User not found");
            return View("Index", _userManager.Users);
        }
        private void AddErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
