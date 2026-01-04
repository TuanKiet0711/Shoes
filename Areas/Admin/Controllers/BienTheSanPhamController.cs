using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BienTheSanPhamController : Controller
    {
        private readonly WebBanGiayContext _context;

        public BienTheSanPhamController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Admin/BienTheSanPham
        public async Task<IActionResult> Index(string? q, string? maSanPham, int? size, string? mauSac)
        {
            ViewData["Title"] = "Danh sách biến thể";

            var query = _context.BienTheSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .AsQueryable();

            // 🔍 Tìm theo: mã biến thể / mã SP / tên SP / màu
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.MaBienThe.Contains(q) ||
                    (x.MaSanPham != null && x.MaSanPham.Contains(q)) ||
                    (x.MaSanPhamNavigation != null && x.MaSanPhamNavigation.TenSanPham.Contains(q)) ||
                    (x.MauSac != null && x.MauSac.Contains(q))
                );
            }

            if (!string.IsNullOrWhiteSpace(maSanPham))
                query = query.Where(x => x.MaSanPham == maSanPham);

            if (size.HasValue)
                query = query.Where(x => x.Size == size.Value);

            if (!string.IsNullOrWhiteSpace(mauSac))
            {
                mauSac = mauSac.Trim();
                query = query.Where(x => x.MauSac != null && x.MauSac.Contains(mauSac));
            }

            ViewBag.Query = q;
            ViewBag.MaSanPham = maSanPham;
            ViewBag.Size = size;
            ViewBag.MauSac = mauSac;

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            var data = await query
                .OrderBy(x => x.MaSanPham)
                .ThenBy(x => x.Size)
                .ThenBy(x => x.MauSac)
                .ToListAsync();

            return View(data);
        }

        // GET: /Admin/BienTheSanPham/Create
        public async Task<IActionResult> Create(string? maSanPham)
        {
            ViewData["Title"] = "Thêm biến thể";

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            ViewBag.MaSanPham = maSanPham;
            return View();
        }

        // POST: /Admin/BienTheSanPham/Create  (BỎ VALIDATE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BienTheSanPham model)
        {
            // Không validate gì cả, lưu luôn (DB not null/unique thì tự báo lỗi)
            _context.BienTheSanPham.Add(model);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm biến thể thành công!";

            return RedirectToAction(nameof(Index), new { maSanPham = model.MaSanPham });
        }
    }
}
