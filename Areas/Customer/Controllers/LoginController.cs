using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Helpers;
using WebBanGiay.Models;
using WebBanGiay.ViewModels;

namespace WebBanGiay.Areas.Customer.Controllers;

[Area("Customer")]
public class LoginController : Controller
{
    private readonly WebBanGiayContext _context;

    public LoginController(WebBanGiayContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null, string? denied = null)
    {
        ViewBag.Denied = denied;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (taiKhoan == null || !PasswordHelper.Verify(model.MatKhau, taiKhoan.MatKhau))
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(taiKhoan.TokenXacThucEmail))
        {
            ModelState.AddModelError(string.Empty, "Email chưa xác thực. Vui lòng đăng ký lại để nhận email mới.");
            return View(model);
        }

        if (string.Equals(taiKhoan.TrangThai, "Khoa", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Tài khoản đang bị khóa.");
            return View(model);
        }

        var role = string.IsNullOrWhiteSpace(taiKhoan.VaiTro) ? "KhachHang" : taiKhoan.VaiTro;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan),
            new Claim(ClaimTypes.Name, taiKhoan.HoTen ?? taiKhoan.Email),
            new Claim(ClaimTypes.Email, taiKhoan.Email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
            Url.IsLocalUrl(model.ReturnUrl) &&
            !model.ReturnUrl.StartsWith("/Customer/Login", StringComparison.OrdinalIgnoreCase) &&
            !model.ReturnUrl.StartsWith("/Customer/Register", StringComparison.OrdinalIgnoreCase))
        {
            return Redirect(model.ReturnUrl);
        }

        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        return RedirectToAction("Index", "TrangChu", new { area = "Customer" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "TrangChu", new { area = "Customer" });
    }
}
