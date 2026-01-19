using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

            // sort (nhóm theo sản phẩm)
            var data = await query
                .OrderBy(x => x.MaSanPhamNavigation!.TenSanPham)
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

        // POST: /Admin/BienTheSanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BienTheSanPham model)
        {
            // ✅ Bật validate để Required chạy (tránh MaBienThe null -> crash)
            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();

                ViewBag.MaSanPham = model.MaSanPham;
                return View(model);
            }

            // ✅ (Bonus) báo trùng mã biến thể cho đẹp
            var exists = await _context.BienTheSanPham
                .AsNoTracking()
                .AnyAsync(x => x.MaBienThe == model.MaBienThe);

            if (exists)
            {
                ModelState.AddModelError(nameof(BienTheSanPham.MaBienThe), "Mã biến thể đã tồn tại!");
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();

                ViewBag.MaSanPham = model.MaSanPham;
                return View(model);
            }

            _context.BienTheSanPham.Add(model);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm biến thể thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/BienTheSanPham/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Sửa biến thể";

            var bt = await _context.BienTheSanPham
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaBienThe == id);

            if (bt == null)
                return NotFound();

            ViewBag.SanPhamList = await _context.SanPham
                .AsNoTracking()
                .OrderBy(x => x.TenSanPham)
                .ToListAsync();

            ViewBag.MaSanPham = bt.MaSanPham;
            return View(bt);
        }

        // POST: /Admin/BienTheSanPham/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BienTheSanPham model)
        {
            if (id != model.MaBienThe)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.SanPhamList = await _context.SanPham
                    .AsNoTracking()
                    .OrderBy(x => x.TenSanPham)
                    .ToListAsync();

                ViewBag.MaSanPham = model.MaSanPham;
                return View(model);
            }

            var bt = await _context.BienTheSanPham
                .FirstOrDefaultAsync(x => x.MaBienThe == id);

            if (bt == null)
                return NotFound();

            bt.MaSanPham = model.MaSanPham;
            bt.Size = model.Size;
            bt.MauSac = model.MauSac;
            bt.SoLuong = model.SoLuong;

            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật biến thể thành công!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Chi tiết biến thể";

            var bt = await _context.BienTheSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .FirstOrDefaultAsync(x => x.MaBienThe == id);

            if (bt == null)
                return NotFound();

            return View(bt);
        }
    
        // GET: /Admin/BienTheSanPham/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Xóa biến thể";

            var bt = await _context.BienTheSanPham
                .AsNoTracking()
                .Include(x => x.MaSanPhamNavigation)
                .FirstOrDefaultAsync(x => x.MaBienThe == id);

            if (bt == null)
                return NotFound();

            return View(bt);
        }

        // POST: /Admin/BienTheSanPham/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var bt = await _context.BienTheSanPham.FirstOrDefaultAsync(x => x.MaBienThe == id);
            if (bt == null)
                return NotFound();

            _context.BienTheSanPham.Remove(bt);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Xóa biến thể thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
