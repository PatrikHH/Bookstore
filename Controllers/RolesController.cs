﻿using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<AppUser> _userManager;
        public RolesController(BaseService baseService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : base(baseService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrors(result);
            }
            return View(name);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonmembers = new List<AppUser>();
            foreach (AppUser user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                RoleMembers = members,
                RoleNonMembers = nonmembers
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(RoleModification roleModifications)
        {
            AppUser user;
            IdentityResult result;
            foreach (var id in roleModifications.AddIds ?? new string[] { })
            {
                user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    result = await _userManager.AddToRoleAsync(user, roleModifications.RoleName);
                    if (result != IdentityResult.Success)
                        AddErrors(result);
                }
            }
            foreach (var id in roleModifications.DeleteIds ?? new string[] { })
            {
                user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    result = await _userManager.RemoveFromRoleAsync(user, roleModifications.RoleName);
                    if (result != IdentityResult.Success)
                        AddErrors(result);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(roleToDelete);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrors(result);
            }
            ModelState.AddModelError("", "No role found");
            return View("Index", _roleManager.Roles);
        }
        private void AddErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
