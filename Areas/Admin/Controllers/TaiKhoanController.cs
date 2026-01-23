using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Helpers;
using WebBanGiay.Models;
using WebBanGiay.ViewModels.Admin;

namespace WebBanGiay.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TaiKhoanController : Controller
{
    private readonly WebBanGiayContext _context;

    public TaiKhoanController(WebBanGiayContext context)
    {
        _context = context;
    }

    // GET: /Admin/TaiKhoan
    public async Task<IActionResult> Index(string? q, string? role, string? status)
    {
        ViewData["Title"] = "Tài khoản";

        var query = _context.TaiKhoan.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(x =>
                x.MaTaiKhoan.Contains(q) ||
                (x.HoTen != null && x.HoTen.Contains(q)) ||
                x.Email.Contains(q) ||
                (x.Sdt != null && x.Sdt.Contains(q)));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleValue = role.Trim().ToLower();
            query = query.Where(x => x.VaiTro != null && x.VaiTro.ToLower() == roleValue);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusValue = status.Trim().ToLower();
            query = query.Where(x => x.TrangThai != null && x.TrangThai.ToLower() == statusValue);
        }

        ViewBag.Query = q;
        ViewBag.Role = role;
        ViewBag.Status = status;

        var data = await query
            .OrderByDescending(x => x.MaTaiKhoan)
            .ToListAsync();

        return View(data);
    }

    // GET: /Admin/TaiKhoan/Details/id
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        ViewData["Title"] = "Chi tiết tài khoản";

        var tk = await _context.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == id);

        if (tk == null)
            return NotFound();

        return View(tk);
    }

    // GET: /Admin/TaiKhoan/Create
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Thêm tài khoản";
        return View(new TaiKhoanCreateViewModel());
    }

    // POST: /Admin/TaiKhoan/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!TryParseNgaySinh(model.NgaySinh, out var ngaySinh, out var birthError))
        {
            ModelState.AddModelError(nameof(TaiKhoanCreateViewModel.NgaySinh), birthError);
            return View(model);
        }

        var emailExists = await _context.TaiKhoan
            .AsNoTracking()
            .AnyAsync(x => x.Email == model.Email);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(TaiKhoanCreateViewModel.Email), "Email đã tồn tại.");
            return View(model);
        }

        var taiKhoan = new TaiKhoan
        {
            MaTaiKhoan = $"TK{Guid.NewGuid():N}",
            HoTen = model.HoTen,
            Email = model.Email,
            Sdt = string.IsNullOrWhiteSpace(model.Sdt) ? null : model.Sdt,
            GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh,
            NgaySinh = ngaySinh,
            MatKhau = PasswordHelper.Hash(model.MatKhau),
            VaiTro = model.VaiTro,
            TrangThai = model.TrangThai,
            TokenXacThucEmail = null,
            HanTokenEmail = null
        };

        _context.TaiKhoan.Add(taiKhoan);
        await _context.SaveChangesAsync();

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "✅ Thêm tài khoản thành công!";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/TaiKhoan/Edit/id
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        ViewData["Title"] = "Sửa tài khoản";

        var tk = await _context.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == id);

        if (tk == null)
            return NotFound();

        var model = new TaiKhoanEditViewModel
        {
            MaTaiKhoan = tk.MaTaiKhoan,
            HoTen = tk.HoTen ?? string.Empty,
            Email = tk.Email,
            Sdt = tk.Sdt,
            GioiTinh = tk.GioiTinh,
            NgaySinh = tk.NgaySinh.HasValue ? tk.NgaySinh.Value.ToString("dd/MM/yyyy") : null,
            VaiTro = string.IsNullOrWhiteSpace(tk.VaiTro) ? "KhachHang" : tk.VaiTro,
            TrangThai = string.IsNullOrWhiteSpace(tk.TrangThai) ? "HoatDong" : tk.TrangThai
        };

        return View(model);
    }

    // POST: /Admin/TaiKhoan/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, TaiKhoanEditViewModel model)
    {
        if (id != model.MaTaiKhoan)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        if (!TryParseNgaySinh(model.NgaySinh, out var ngaySinh, out var birthError))
        {
            ModelState.AddModelError(nameof(TaiKhoanEditViewModel.NgaySinh), birthError);
            return View(model);
        }

        var emailExists = await _context.TaiKhoan
            .AsNoTracking()
            .AnyAsync(x => x.Email == model.Email && x.MaTaiKhoan != id);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(TaiKhoanEditViewModel.Email), "Email đã tồn tại.");
            return View(model);
        }

        var tk = await _context.TaiKhoan.FirstOrDefaultAsync(x => x.MaTaiKhoan == id);
        if (tk == null)
            return NotFound();

        tk.HoTen = model.HoTen;
        tk.Email = model.Email;
        tk.Sdt = string.IsNullOrWhiteSpace(model.Sdt) ? null : model.Sdt;
        tk.GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh;
        tk.NgaySinh = ngaySinh;
        tk.VaiTro = model.VaiTro;
        tk.TrangThai = model.TrangThai;

        if (!string.IsNullOrWhiteSpace(model.MatKhau))
        {
            tk.MatKhau = PasswordHelper.Hash(model.MatKhau);
        }

        await _context.SaveChangesAsync();

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "✅ Cập nhật tài khoản thành công!";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/TaiKhoan/Delete/id
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        ViewData["Title"] = "Xóa tài khoản";

        var tk = await _context.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == id);

        if (tk == null)
            return NotFound();

        return View(tk);
    }

    // POST: /Admin/TaiKhoan/Delete/id
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var tk = await _context.TaiKhoan.FirstOrDefaultAsync(x => x.MaTaiKhoan == id);
        if (tk == null)
            return NotFound();

        var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(currentId) && string.Equals(currentId, tk.MaTaiKhoan, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Không thể xóa tài khoản đang đăng nhập.");
            return View("Delete", tk);
        }

        if (string.Equals(tk.VaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Không thể xóa tài khoản Admin.");
            return View("Delete", tk);
        }

        _context.TaiKhoan.Remove(tk);
        await _context.SaveChangesAsync();

        TempData["ToastType"] = "success";
        TempData["ToastMessage"] = "✅ Xóa tài khoản thành công!";

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
