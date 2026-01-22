using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Helpers;
using WebBanGiay.Models;
using WebBanGiay.ViewModels;

namespace WebBanGiay.Areas.Customer.Controllers;

[Area("Customer")]
public class ForgotPasswordController : Controller
{
    private const int ResetTokenHours = 2;
    private readonly WebBanGiayContext _context;
    private readonly IConfiguration _configuration;

    public ForgotPasswordController(WebBanGiayContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ForgotPasswordRequestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ForgotPasswordRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ForgotPasswordError"] = ModelState[nameof(ForgotPasswordRequestViewModel.Email)]?.Errors.FirstOrDefault()?.ErrorMessage
                                               ?? "Dữ liệu không hợp lệ.";
            TempData["ForgotPasswordEmail"] = model.Email;
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        var taiKhoan = await _context.TaiKhoan.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (taiKhoan == null)
        {
            TempData["ForgotPasswordError"] = "Email không tồn tại.";
            TempData["ForgotPasswordEmail"] = model.Email;
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        if (!string.IsNullOrWhiteSpace(taiKhoan.TokenXacThucEmail))
        {
            TempData["ForgotPasswordError"] = "Email chưa xác thực. Vui lòng xác thực trước khi đặt lại mật khẩu.";
            TempData["ForgotPasswordEmail"] = model.Email;
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        var token = GenerateResetToken();
        taiKhoan.TokenDatLaiMatKhau = token;
        taiKhoan.HanDatLaiMatKhau = DateTime.UtcNow.AddHours(ResetTokenHours);
        await _context.SaveChangesAsync();

        if (!await SendResetEmailAsync(taiKhoan.Email, token))
        {
            TempData["ForgotPasswordError"] = "Không gửi được email đặt lại mật khẩu. Vui lòng thử lại.";
            TempData["ForgotPasswordEmail"] = model.Email;
            return RedirectToAction("Index", "Login", new { area = "Customer" });
        }

        TempData["ForgotPasswordSuccess"] = "Đã gửi email đặt lại mật khẩu. Vui lòng kiểm tra hộp thư.";
        TempData["ForgotPasswordEmail"] = model.Email;
        return RedirectToAction("Index", "Login", new { area = "Customer" });
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string? email, string? token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            ViewBag.Message = "Liên kết đặt lại mật khẩu không hợp lệ.";
            return View("ResetPasswordMessage");
        }

        var taiKhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(x => x.Email == email && x.TokenDatLaiMatKhau == token);

        if (taiKhoan == null)
        {
            ViewBag.Message = "Liên kết đặt lại mật khẩu không hợp lệ.";
            return View("ResetPasswordMessage");
        }

        if (taiKhoan.HanDatLaiMatKhau.HasValue && taiKhoan.HanDatLaiMatKhau.Value < DateTime.UtcNow)
        {
            ViewBag.Message = "Liên kết đã hết hạn. Vui lòng yêu cầu lại.";
            return View("ResetPasswordMessage");
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var taiKhoan = await _context.TaiKhoan
            .FirstOrDefaultAsync(x => x.Email == model.Email && x.TokenDatLaiMatKhau == model.Token);

        if (taiKhoan == null)
        {
            ModelState.AddModelError(string.Empty, "Token đặt lại mật khẩu không hợp lệ.");
            return View(model);
        }

        if (taiKhoan.HanDatLaiMatKhau.HasValue && taiKhoan.HanDatLaiMatKhau.Value < DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "Token đã hết hạn. Vui lòng yêu cầu lại.");
            return View(model);
        }

        taiKhoan.MatKhau = PasswordHelper.Hash(model.MatKhauMoi);
        taiKhoan.TokenDatLaiMatKhau = null;
        taiKhoan.HanDatLaiMatKhau = null;
        await _context.SaveChangesAsync();

        ViewBag.Message = "Đổi mật khẩu thành công.";
        return View("ResetPasswordMessage");
    }

    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private async Task<bool> SendResetEmailAsync(string email, string token)
    {
        var publicBaseUrl = _configuration["PublicBaseUrl"];
        var resetUrl = Url.Action(
            "ResetPassword",
            "ForgotPassword",
            new { area = "Customer", email, token },
            Request.Scheme
        );

        if (!string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            resetUrl = Url.Action(
                "ResetPassword",
                "ForgotPassword",
                new { area = "Customer", email, token }
            );

            if (!string.IsNullOrWhiteSpace(resetUrl))
            {
                resetUrl = $"{publicBaseUrl.TrimEnd('/')}{resetUrl}";
            }
        }

        if (string.IsNullOrWhiteSpace(resetUrl))
        {
            return false;
        }

        var subject = "Đặt lại mật khẩu";
        var body = $"<p>Vui lòng bấm vào liên kết để đặt lại mật khẩu:</p><p><a href=\"{resetUrl}\">{resetUrl}</a></p>";
        return await EmailHelper.SendEmailAsync(_configuration, email, subject, body);
    }
}
