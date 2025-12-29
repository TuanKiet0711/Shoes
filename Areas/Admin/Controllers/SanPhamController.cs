using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly WebBanGiayContext _context;

        public SanPhamController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Admin/SanPham
        public async Task<IActionResult> Index(
            string? q,
            string? danhMuc,
            string? thuongHieu,
            string? trangThai,
            string? gioiTinh)
        {
            ViewData["Title"] = "Danh sách sản phẩm";

            var query = _context.SanPham
                .AsNoTracking()
                .Include(x => x.MaDanhMucNavigation)
                .Include(x => x.MaThuongHieuNavigation)
                .AsQueryable();

            // 🔍 Tìm theo mã / tên
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.MaSanPham.Contains(q) || x.TenSanPham.Contains(q));
            }

            // 📂 Danh mục
            if (!string.IsNullOrWhiteSpace(danhMuc))
                query = query.Where(x => x.MaDanhMuc == danhMuc);

            // 🏷️ Thương hiệu
            if (!string.IsNullOrWhiteSpace(thuongHieu))
                query = query.Where(x => x.MaThuongHieu == thuongHieu);

            // 🚻 Giới tính
            if (!string.IsNullOrWhiteSpace(gioiTinh))
                query = query.Where(x => x.GioiTinh == gioiTinh);

            // 📦 Trạng thái (DB: ConHang / HetHang)
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                if (trangThai == "con")
                    query = query.Where(x => x.TrangThai == "ConHang");
                else if (trangThai == "het")
                    query = query.Where(x => x.TrangThai == "HetHang");
            }

            // giữ filter
            ViewBag.Query = q;
            ViewBag.DanhMuc = danhMuc;
            ViewBag.ThuongHieu = thuongHieu;
            ViewBag.TrangThai = trangThai;
            ViewBag.GioiTinh = gioiTinh;

            ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
            ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return View(data);
        }

        // GET: /Admin/SanPham/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Thêm sản phẩm";

            ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
            ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();

            return View();
        }

        // POST: /Admin/SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham model)
        {
            // 🔒 đảm bảo đúng constraint DB
            if (!string.IsNullOrWhiteSpace(model.TrangThai))
            {
                model.TrangThai = model.TrangThai.Trim();
                if (model.TrangThai != "ConHang" && model.TrangThai != "HetHang")
                    model.TrangThai = "ConHang";
            }
            else
            {
                model.TrangThai = "ConHang";
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
                ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();
                return View(model);
            }

            model.NgayTao = DateTime.Now;

            _context.SanPham.Add(model);
            await _context.SaveChangesAsync();

            // ✅ TOAST
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Thêm sản phẩm thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/SanPham/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Sửa sản phẩm";

            var sp = await _context.SanPham
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaSanPham == id);

            if (sp == null)
                return NotFound();

            ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
            ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();

            return View(sp);
        }

        // POST: /Admin/SanPham/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SanPham model)
        {
            if (id != model.MaSanPham)
                return BadRequest();

            // 🔒 đảm bảo đúng constraint DB (ConHang/HetHang)
            if (!string.IsNullOrWhiteSpace(model.TrangThai))
            {
                model.TrangThai = model.TrangThai.Trim();
                if (model.TrangThai != "ConHang" && model.TrangThai != "HetHang")
                    model.TrangThai = "ConHang";
            }
            else
            {
                model.TrangThai = "ConHang";
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
                ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();
                return View(model);
            }

            var sp = await _context.SanPham.FirstOrDefaultAsync(x => x.MaSanPham == id);
            if (sp == null) return NotFound();

            sp.TenSanPham = model.TenSanPham;
            sp.MaDanhMuc = model.MaDanhMuc;
            sp.MaThuongHieu = model.MaThuongHieu;
            sp.Gia = model.Gia;
            sp.GioiTinh = model.GioiTinh;
            sp.TrangThai = model.TrangThai;
            sp.MoTa = model.MoTa;

            await _context.SaveChangesAsync();

            // ✅ TOAST
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật sản phẩm thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/SanPham/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Xóa sản phẩm";

            var sp = await _context.SanPham
                .AsNoTracking()
                .Include(x => x.MaDanhMucNavigation)
                .Include(x => x.MaThuongHieuNavigation)
                .FirstOrDefaultAsync(x => x.MaSanPham == id);

            if (sp == null)
                return NotFound();

            return View(sp);
        }

        // POST: /Admin/SanPham/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var sp = await _context.SanPham.FirstOrDefaultAsync(x => x.MaSanPham == id);
            if (sp == null)
                return NotFound();

            _context.SanPham.Remove(sp);
            await _context.SaveChangesAsync();

            // ✅ TOAST (đỏ cho xóa)
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "🗑️ Xóa sản phẩm thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}
