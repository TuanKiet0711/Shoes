using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HinhAnhSanPhamController : Controller
    {
        private readonly WebBanGiayContext _context;
        private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".avif" };

        public HinhAnhSanPhamController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Admin/HinhAnhSanPham
        public async Task<IActionResult> Index(string? q, string? maSanPham, bool? laAnhDaiDien)
        {
            ViewData["Title"] = "Hình ảnh sản phẩm";

            var query = _context.HinhAnhSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.MaHinhAnh.Contains(q) ||
                    (x.MaSanPham != null && x.MaSanPham.Contains(q)) ||
                    (x.MaSanPhamNavigation != null && x.MaSanPhamNavigation.TenSanPham.Contains(q)) ||
                    (x.DuongDanAnh != null && x.DuongDanAnh.Contains(q))
                );
            }

            if (!string.IsNullOrWhiteSpace(maSanPham))
                query = query.Where(x => x.MaSanPham == maSanPham);

            if (laAnhDaiDien.HasValue)
                query = query.Where(x => (x.LaAnhDaiDien ?? false) == laAnhDaiDien.Value);

            ViewBag.Query = q;
            ViewBag.MaSanPham = maSanPham;
            ViewBag.LaAnhDaiDien = laAnhDaiDien;

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            var data = await query
                .OrderBy(x => x.MaSanPham)
                .ThenByDescending(x => x.LaAnhDaiDien ?? false)
                .ThenBy(x => x.MaHinhAnh)
                .ToListAsync();

            return View(data);
        }

        // GET: /Admin/HinhAnhSanPham/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Chi tiết hình ảnh";

            var img = await _context.HinhAnhSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .FirstOrDefaultAsync(x => x.MaHinhAnh == id);

            if (img == null)
                return NotFound();

            return View(img);
        }

        // GET: /Admin/HinhAnhSanPham/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm hình ảnh";

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            return View();
        }

        // POST: /Admin/HinhAnhSanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnhSanPham model, IFormFile? anhFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();
                return View(model);
            }

            if (anhFile == null || anhFile.Length == 0)
            {
                ModelState.AddModelError(nameof(HinhAnhSanPham.DuongDanAnh), "Vui lòng chọn ảnh.");
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();
                return View(model);
            }

            var exists = await _context.HinhAnhSanPham
                .AsNoTracking()
                .AnyAsync(x => x.MaHinhAnh == model.MaHinhAnh);

            if (exists)
            {
                ModelState.AddModelError(nameof(HinhAnhSanPham.MaHinhAnh), "Mã hình ảnh đã tồn tại!");
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();
                return View(model);
            }

            var laAnhValues = Request.Form["LaAnhDaiDien"];
            model.LaAnhDaiDien = laAnhValues.Any(v => v.Equals("true", StringComparison.OrdinalIgnoreCase));

            model.DuongDanAnh = await SaveImageAsync(anhFile);

            _context.HinhAnhSanPham.Add(model);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm hình ảnh thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/HinhAnhSanPham/Edit/id
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Sửa hình ảnh";

            var img = await _context.HinhAnhSanPham
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaHinhAnh == id);

            if (img == null)
                return NotFound();

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            return View(img);
        }

        // POST: /Admin/HinhAnhSanPham/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, HinhAnhSanPham model, IFormFile? anhFile)
        {
            if (id != model.MaHinhAnh)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();
                return View(model);
            }

            var img = await _context.HinhAnhSanPham.FirstOrDefaultAsync(x => x.MaHinhAnh == id);
            if (img == null)
                return NotFound();

            img.MaSanPham = model.MaSanPham;
            var laAnhValues = Request.Form["LaAnhDaiDien"];
            img.LaAnhDaiDien = laAnhValues.Any(v => v.Equals("true", StringComparison.OrdinalIgnoreCase));

            if (anhFile != null && anhFile.Length > 0)
            {
                img.DuongDanAnh = await SaveImageAsync(anhFile);
            }

            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật hình ảnh thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/HinhAnhSanPham/Delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Xóa hình ảnh";

            var img = await _context.HinhAnhSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .FirstOrDefaultAsync(x => x.MaHinhAnh == id);

            if (img == null)
                return NotFound();

            return View(img);
        }

        // POST: /Admin/HinhAnhSanPham/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var img = await _context.HinhAnhSanPham.FirstOrDefaultAsync(x => x.MaHinhAnh == id);
            if (img == null)
                return NotFound();

            _context.HinhAnhSanPham.Remove(img);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Xóa hình ảnh thành công!";

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new InvalidOperationException("Định dạng ảnh không hỗ trợ.");

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hinh-anh");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/hinh-anh/{fileName}";
        }
    }
}
