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

        // ✅ Update trạng thái sản phẩm theo biến thể (optional: để DB cũng đúng)
        private async Task UpdateTrangThaiSanPhamAsync(string maSanPham)
        {
            var conHang = await _context.BienTheSanPham
                .AsNoTracking()
                .AnyAsync(x => x.MaSanPham == maSanPham && (x.SoLuong ?? 0) > 0);

            var sp = await _context.SanPham.FirstOrDefaultAsync(x => x.MaSanPham == maSanPham);
            if (sp == null) return;

            sp.TrangThai = conHang ? "ConHang" : "HetHang";
            await _context.SaveChangesAsync();
        }

        // GET: /Admin/SanPham
        public async Task<IActionResult> Index(
            string? q,
            string? danhMuc,
            string? thuongHieu,
            string? trangThai, // "con" | "het"
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

            // ✅ Lọc trạng thái theo BIẾN THỂ (không dùng sp.TrangThai)
            // ✅ Lọc trạng thái theo biến thể (NHANH - KHÔNG TREO)
if (!string.IsNullOrWhiteSpace(trangThai))
{
    var maConHang = await _context.BienTheSanPham
        .AsNoTracking()
        .GroupBy(x => x.MaSanPham)
        .Where(g => g.Sum(x => x.SoLuong ?? 0) > 0)
        .Select(g => g.Key)
        .ToListAsync();

    if (trangThai == "con")
        query = query.Where(sp => maConHang.Contains(sp.MaSanPham));
    else if (trangThai == "het")
        query = query.Where(sp => !maConHang.Contains(sp.MaSanPham));
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

            // ✅ Tính tồn kho theo biến thể để view hiển thị nhanh
            var maSps = data.Select(x => x.MaSanPham).ToList();

            var tonKho = await _context.BienTheSanPham
                .AsNoTracking()
                .Where(x => x.MaSanPham != null && maSps.Contains(x.MaSanPham))
                .GroupBy(x => x.MaSanPham)
                .Select(g => new { MaSanPham = g.Key!, Tong = g.Sum(x => x.SoLuong ?? 0) })
                .ToDictionaryAsync(x => x.MaSanPham, x => x.Tong);

            ViewBag.TonKho = tonKho;

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
            // ✅ Không nhập trạng thái thủ công nữa
            // Nếu chưa có biến thể => mặc định Hết hàng
            model.TrangThai = "HetHang";

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = await _context.DanhMuc.AsNoTracking().ToListAsync();
                ViewBag.ThuongHieuList = await _context.ThuongHieu.AsNoTracking().ToListAsync();
                return View(model);
            }

            model.NgayTao = DateTime.Now;

            _context.SanPham.Add(model);
            await _context.SaveChangesAsync();

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

            // ✅ Không cho sửa TrangThai thủ công
            // Giữ lại trạng thái hiện tại trong DB, rồi cuối cùng update theo biến thể
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
            sp.MoTa = model.MoTa;

            await _context.SaveChangesAsync();

            // ✅ cập nhật lại trạng thái theo biến thể
            await UpdateTrangThaiSanPhamAsync(id);

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "✅ Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/SanPham/Details/id
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["Title"] = "Chi tiết sản phẩm";

            var sp = await _context.SanPham
                .AsNoTracking()
                .Include(x => x.MaDanhMucNavigation)
                .Include(x => x.MaThuongHieuNavigation)
                .Include(x => x.HinhAnhSanPham)
                .Include(x => x.BienTheSanPham)
                .FirstOrDefaultAsync(x => x.MaSanPham == id);

            if (sp == null)
                return NotFound();

            return View(sp);
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

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "🗑️ Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
