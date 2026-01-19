using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebBanGiay.Helpers;
using WebBanGiay.Models;
using WebBanGiay.ViewModels;

namespace WebBanGiay.Areas.Customer.Controllers;

[Area("Customer")]
public class RegisterController : Controller
{
    private readonly WebBanGiayContext _context;
    private readonly IConfiguration _configuration;
    private const int EmailTokenHours = 24;

    public RegisterController(WebBanGiayContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var digits = new string(model.NgaySinh.Where(char.IsDigit).ToArray());
        if (digits.Length != 8)
        {
            ModelState.AddModelError(nameof(RegisterViewModel.NgaySinh), "Ngày sinh phải theo định dạng dd/MM/yyyy.");
            return View(model);
        }

        var day = int.Parse(digits.Substring(0, 2));
        var month = int.Parse(digits.Substring(2, 2));
        var year = int.Parse(digits.Substring(4, 4));

        month = Math.Clamp(month, 1, 12);
        var maxDay = DateTime.DaysInMonth(year, month);
        day = Math.Clamp(day, 1, maxDay);

        var ngaySinh = new DateTime(year, month, day);

        var existing = await _context.TaiKhoan.FirstOrDefaultAsync(x => x.Email == model.Email);
        var token = GenerateEmailToken();
        var tokenExpiry = DateTime.UtcNow.AddHours(EmailTokenHours);

        if (existing != null)
        {
            if (string.IsNullOrWhiteSpace(existing.TokenXacThucEmail))
            {
                ModelState.AddModelError(nameof(RegisterViewModel.Email), "Email đã tồn tại.");
                return View(model);
            }

            existing.HoTen = model.HoTen;
            existing.GioiTinh = model.GioiTinh;
            existing.NgaySinh = DateOnly.FromDateTime(ngaySinh);
            existing.Sdt = model.Sdt;
            existing.MatKhau = PasswordHelper.Hash(model.MatKhau);
            existing.TokenXacThucEmail = token;
            existing.HanTokenEmail = tokenExpiry;

            await _context.SaveChangesAsync();

            if (!await SendVerifyEmailAsync(existing.Email, token))
            {
                ModelState.AddModelError(string.Empty, "Không gửi được email xác thực. Vui lòng thử lại.");
                return View(model);
            }

            TempData["RegisterSuccess"] = "Email đã tồn tại nhưng chưa xác thực. Vui lòng kiểm tra email để xác thực.";
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        var taiKhoan = new TaiKhoan
        {
            MaTaiKhoan = $"TK{Guid.NewGuid():N}",
            HoTen = model.HoTen,
            GioiTinh = model.GioiTinh,
            NgaySinh = DateOnly.FromDateTime(ngaySinh),
            Email = model.Email,
            Sdt = model.Sdt,
            MatKhau = PasswordHelper.Hash(model.MatKhau),
            VaiTro = "KhachHang",
            TrangThai = "HoatDong",
            TokenXacThucEmail = token,
            HanTokenEmail = tokenExpiry
        };

        _context.TaiKhoan.Add(taiKhoan);
        await _context.SaveChangesAsync();

        if (!await SendVerifyEmailAsync(taiKhoan.Email, token))
        {
            ModelState.AddModelError(string.Empty, "Không gửi được email xác thực. Vui lòng thử lại.");
            return View(model);
        }

        TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực.";
        return RedirectToAction("Index", "Login", new { area = "Customer" });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string? email, string? token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            ViewBag.Message = "Liên kết xác thực không hợp lệ.";
            return View();
        }

        var taiKhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(x => x.Email == email && x.TokenXacThucEmail == token);

        if (taiKhoan == null)
        {
            ViewBag.Message = "Liên kết xác thực không hợp lệ.";
            return View();
        }

        if (taiKhoan.HanTokenEmail.HasValue && taiKhoan.HanTokenEmail.Value < DateTime.UtcNow)
        {
            ViewBag.Message = "Token đã hết hạn. Vui lòng đăng ký lại để nhận email mới.";
            return View();
        }

        taiKhoan.TokenXacThucEmail = null;
        taiKhoan.HanTokenEmail = null;
        await _context.SaveChangesAsync();

        ViewBag.Message = "Xác thực email thành công. Bạn có thể đăng nhập.";
        return View();
    }

    private static string GenerateEmailToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private async Task<bool> SendVerifyEmailAsync(string email, string token)
    {
        var publicBaseUrl = _configuration["PublicBaseUrl"];
        var verifyUrl = Url.Action(
            "VerifyEmail",
            "Register",
            new { area = "Customer", email, token },
            Request.Scheme
        );

        if (!string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            verifyUrl = Url.Action(
                "VerifyEmail",
                "Register",
                new { area = "Customer", email, token }
            );

            if (!string.IsNullOrWhiteSpace(verifyUrl))
            {
                verifyUrl = $"{publicBaseUrl.TrimEnd('/')}{verifyUrl}";
            }
        }

        if (string.IsNullOrWhiteSpace(verifyUrl))
        {
            return false;
        }

        var subject = "Xác thực email";
        var body = $"<p>Vui lòng bấm vào link để xác thực email:</p><p><a href=\"{verifyUrl}\">{verifyUrl}</a></p>";
        return await EmailHelper.SendEmailAsync(_configuration, email, subject, body);
    }
}
