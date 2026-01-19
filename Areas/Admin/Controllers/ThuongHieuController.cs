using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ThuongHieuController : Controller
    {
        private readonly WebBanGiayContext _context;

        public ThuongHieuController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Admin/ThuongHieu
        public async Task<IActionResult> Index(string? q)
        {
            ViewData["Title"] = "Thương hiệu";

            var query = _context.ThuongHieu.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.MaThuongHieu.Contains(q) || x.TenThuongHieu.Contains(q));
            }

            ViewBag.Query = q;

            var data = await query
                .OrderBy(x => x.TenThuongHieu)
                .ToListAsync();

            return View(data);
        }

        // GET: /Admin/ThuongHieu/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Chi tiết thương hiệu";

            var th = await _context.ThuongHieu
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaThuongHieu == id);

            if (th == null)
                return NotFound();

            return View(th);
        }

        // GET: /Admin/ThuongHieu/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm thương hiệu";
            return View();
        }

        // POST: /Admin/ThuongHieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThuongHieu model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exists = await _context.ThuongHieu
                .AsNoTracking()
                .AnyAsync(x => x.MaThuongHieu == model.MaThuongHieu);

            if (exists)
            {
                ModelState.AddModelError(nameof(ThuongHieu.MaThuongHieu), "Mã thương hiệu đã tồn tại!");
                return View(model);
            }

            _context.ThuongHieu.Add(model);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ThuongHieu/Edit/id
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Sửa thương hiệu";

            var th = await _context.ThuongHieu
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaThuongHieu == id);

            if (th == null)
                return NotFound();

            return View(th);
        }

        // POST: /Admin/ThuongHieu/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ThuongHieu model)
        {
            if (id != model.MaThuongHieu)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var th = await _context.ThuongHieu.FirstOrDefaultAsync(x => x.MaThuongHieu == id);
            if (th == null)
                return NotFound();

            th.TenThuongHieu = model.TenThuongHieu;
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ThuongHieu/Delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Xóa thương hiệu";

            var th = await _context.ThuongHieu
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaThuongHieu == id);

            if (th == null)
                return NotFound();

            return View(th);
        }

        // POST: /Admin/ThuongHieu/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var th = await _context.ThuongHieu.FirstOrDefaultAsync(x => x.MaThuongHieu == id);
            if (th == null)
                return NotFound();

            _context.ThuongHieu.Remove(th);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Xóa thương hiệu thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
