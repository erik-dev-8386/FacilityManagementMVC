using CFB.Entities.ThanhDT.Models;
using CFB.Services.ThanhDT;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CFB.MVCWebApp.ThanhDT.Controllers
{
    public class SystemUserAccountsController : Controller
    {
        private readonly SystemUserAccountService _userService;

        public SystemUserAccountsController(SystemUserAccountService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new SystemUserAccount());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SystemUserAccount model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu.";
                return View(model);
            }

            var user = await _userService.GetUserAccount(model.UserName, model.Password);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()), // Dùng RoleId để phân quyền sau này
                new Claim("FullName", user.FullName),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());

            return RedirectToAction("Index", "FacilityThanhDts");
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa Cookie Authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Xóa Session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            // KHÔNG CÓ HttpContext.SignOutAsync() ở đây. Người dùng vẫn đang đăng nhập.

            // Lưu thông báo vào TempData. (Sử dụng tên key khác để tránh xung đột)
            TempData["DeniedActionMessage"] = "Bạn không có đủ quyền (yêu cầu Role 1) để truy cập trang này hoặc thực hiện hành động xóa.";

            return RedirectToAction("Index", "FacilityThanhDts");
        }
    }
}
