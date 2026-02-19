// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Constants;
using Discounts.Infrastructure.Identity;
using Discounts.MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Your account has been blocked.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var role = roles.FirstOrDefault();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return role switch
                {
                    Roles.Admin => RedirectToAction("Index", "Dashboard", new { area = Roles.Admin }),
                    Roles.Merchant => RedirectToAction("Index", "Dashboard", new { area = Roles.Merchant }),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role).ConfigureAwait(false);
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);

                return model.Role switch
                {
                    Roles.Merchant => RedirectToAction("Index", "Dashboard", new { area = Roles.Merchant }),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
