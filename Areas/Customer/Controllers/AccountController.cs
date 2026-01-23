using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Helpers;
using WebBanGiay.Models;
using WebBanGiay.ViewModels.Customer;

namespace WebBanGiay.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class AccountController : Controller
{
    private readonly WebBanGiayContext _context;

    public AccountController(WebBanGiayContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        var taiKhoan = await _context.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == accountId);

        if (taiKhoan == null)
        {
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        var model = new AccountProfileViewModel
        {
            Email = taiKhoan.Email,
            HoTen = taiKhoan.HoTen ?? string.Empty,
            GioiTinh = taiKhoan.GioiTinh,
            NgaySinh = taiKhoan.NgaySinh.HasValue ? taiKhoan.NgaySinh.Value.ToString("dd/MM/yyyy") : null,
            Sdt = taiKhoan.Sdt
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AccountProfileViewModel model)
    {
        var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountId))
        {
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = await _context.TaiKhoan.FirstOrDefaultAsync(x => x.MaTaiKhoan == accountId);
        if (taiKhoan == null)
        {
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        if (!TryParseNgaySinh(model.NgaySinh, out var ngaySinh, out var birthError))
        {
            ModelState.AddModelError(nameof(AccountProfileViewModel.NgaySinh), birthError);
            return View(model);
        }

        var hasPasswordChange = !string.IsNullOrWhiteSpace(model.MatKhauHienTai)
                                || !string.IsNullOrWhiteSpace(model.MatKhauMoi)
                                || !string.IsNullOrWhiteSpace(model.XacNhanMatKhau);

        if (hasPasswordChange)
        {
            if (string.IsNullOrWhiteSpace(model.MatKhauHienTai))
            {
                ModelState.AddModelError(nameof(AccountProfileViewModel.MatKhauHienTai), "Vui lòng nhập mật khẩu hiện tại.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.MatKhauMoi))
            {
                ModelState.AddModelError(nameof(AccountProfileViewModel.MatKhauMoi), "Vui lòng nhập mật khẩu mới.");
                return View(model);
            }

            if (!PasswordHelper.Verify(model.MatKhauHienTai, taiKhoan.MatKhau))
            {
                ModelState.AddModelError(nameof(AccountProfileViewModel.MatKhauHienTai), "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.MatKhauMoi) && model.MatKhauMoi.Length < 6)
            {
                ModelState.AddModelError(nameof(AccountProfileViewModel.MatKhauMoi), "Mật khẩu tối thiểu 6 ký tự.");
                return View(model);
            }

            if (!string.Equals(model.MatKhauMoi, model.XacNhanMatKhau, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(AccountProfileViewModel.XacNhanMatKhau), "Xác nhận mật khẩu không khớp.");
                return View(model);
            }
        }

        taiKhoan.HoTen = model.HoTen.Trim();
        taiKhoan.GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh;
        taiKhoan.NgaySinh = ngaySinh;
        taiKhoan.Sdt = string.IsNullOrWhiteSpace(model.Sdt) ? null : model.Sdt;

        if (hasPasswordChange && !string.IsNullOrWhiteSpace(model.MatKhauMoi))
        {
            taiKhoan.MatKhau = PasswordHelper.Hash(model.MatKhauMoi);
        }

        await _context.SaveChangesAsync();

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

        TempData["AccountSuccess"] = hasPasswordChange
            ? "Cập nhật thông tin và đổi mật khẩu thành công."
            : "Cập nhật thông tin thành công.";
        return RedirectToAction(nameof(Index));
    }

    private static bool TryParseNgaySinh(string? input, out DateOnly? date, out string error)
    {
        date = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return true;

        var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
        if (!DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            error = "Ngày sinh không đúng định dạng.";
            return false;
        }

        if (parsed.Date > DateTime.Today)
        {
            error = "Ngày sinh không được lớn hơn ngày hiện tại.";
            return false;
        }

        date = DateOnly.FromDateTime(parsed);
        return true;
    }
}
