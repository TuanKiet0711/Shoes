using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhMucController : Controller
    {
        private readonly WebBanGiayContext _context;

        public DanhMucController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Admin/DanhMuc
        public async Task<IActionResult> Index(string? q)
        {
            ViewData["Title"] = "Danh mục";

            var query = _context.DanhMuc.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.MaDanhMuc.Contains(q) || x.TenDanhMuc.Contains(q));
            }

            ViewBag.Query = q;

            var data = await query
                .OrderBy(x => x.TenDanhMuc)
                .ToListAsync();

            return View(data);
        }

        // GET: /Admin/DanhMuc/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Chi tiết danh mục";

            var dm = await _context.DanhMuc
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDanhMuc == id);

            if (dm == null)
                return NotFound();

            return View(dm);
        }

        // GET: /Admin/DanhMuc/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm danh mục";
            return View();
        }

        // POST: /Admin/DanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhMuc model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exists = await _context.DanhMuc
                .AsNoTracking()
                .AnyAsync(x => x.MaDanhMuc == model.MaDanhMuc);

            if (exists)
            {
                ModelState.AddModelError(nameof(DanhMuc.MaDanhMuc), "Mã danh mục đã tồn tại!");
                return View(model);
            }

            _context.DanhMuc.Add(model);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/DanhMuc/Edit/id
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Sửa danh mục";

            var dm = await _context.DanhMuc
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDanhMuc == id);

            if (dm == null)
                return NotFound();

            return View(dm);
        }

        // POST: /Admin/DanhMuc/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DanhMuc model)
        {
            if (id != model.MaDanhMuc)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var dm = await _context.DanhMuc.FirstOrDefaultAsync(x => x.MaDanhMuc == id);
            if (dm == null)
                return NotFound();

            dm.TenDanhMuc = model.TenDanhMuc;
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/DanhMuc/Delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Xóa danh mục";

            var dm = await _context.DanhMuc
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDanhMuc == id);

            if (dm == null)
                return NotFound();

            return View(dm);
        }

        // POST: /Admin/DanhMuc/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var dm = await _context.DanhMuc.FirstOrDefaultAsync(x => x.MaDanhMuc == id);
            if (dm == null)
                return NotFound();

            _context.DanhMuc.Remove(dm);
            await _context.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Xóa danh mục thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
