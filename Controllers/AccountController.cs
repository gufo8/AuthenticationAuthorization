﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAuthenticationAuthorization.Models;
using TaskAuthenticationAuthorization.ViewModels;

namespace TaskAuthenticationAuthorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShoppingContext _context;

        public AccountController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Account  
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            User user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                await Authenticate(user);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Incorrect login and(or) password");

            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.Buyer);
                    user = new User 
                    { 
                        Email = model.Email, 
                        Password = model.Password,
                        RoleId = role.Id,
                        Role = role,
                        Type = BuyerType.Regular
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    return View("SuccessfullRegister", user.Email);
                }
                ModelState.AddModelError("", "User with such email already exists");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name.ToString()),
                new Claim("BuyerType", user.Type.ToString())
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie"
                , ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
