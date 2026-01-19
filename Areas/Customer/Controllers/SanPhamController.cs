using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SanPhamController : Controller
    {
        private readonly WebBanGiayContext _context;

        public SanPhamController(WebBanGiayContext context)
        {
            _context = context;
        }

        // GET: /Customer/SanPham
        public async Task<IActionResult> Index(
            string? q,
            string[]? danhMuc,
            string[]? thuongHieu,
            string[]? gioiTinh,
            int[]? size,
            string[]? mauSac,
            string[]? tinhTrang,
            string[]? gia,
            string? sapXep)
        {
            ViewData["Title"] = "Sản phẩm";

            ViewBag.DanhMuc = await _context.DanhMuc.AsNoTracking().ToListAsync();
            ViewBag.ThuongHieu = await _context.ThuongHieu.AsNoTracking().ToListAsync();

            ViewBag.SizeList = await _context.BienTheSanPham
                .AsNoTracking()
                .Where(x => x.Size.HasValue)
                .Select(x => x.Size!.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.MauSacList = await _context.BienTheSanPham
                .AsNoTracking()
                .Where(x => x.MauSac != null && x.MauSac != "")
                .Select(x => x.MauSac!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var query = _context.SanPham
                .AsNoTracking()
                .Include(x => x.HinhAnhSanPham)
                .Include(x => x.MaThuongHieuNavigation)
                .Include(x => x.MaDanhMucNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.MaSanPham.Contains(q) || x.TenSanPham.Contains(q));
            }

            if (danhMuc != null && danhMuc.Length > 0)
                query = query.Where(x => x.MaDanhMuc != null && danhMuc.Contains(x.MaDanhMuc));

            if (thuongHieu != null && thuongHieu.Length > 0)
                query = query.Where(x => x.MaThuongHieu != null && thuongHieu.Contains(x.MaThuongHieu));

            if (gioiTinh != null && gioiTinh.Length > 0)
                query = query.Where(x => x.GioiTinh != null && gioiTinh.Contains(x.GioiTinh));

            if (size != null && size.Length > 0)
                query = query.Where(x => x.BienTheSanPham.Any(b => b.Size.HasValue && size.Contains(b.Size.Value)));

            if (mauSac != null && mauSac.Length > 0)
                query = query.Where(x => x.BienTheSanPham.Any(b => b.MauSac != null && mauSac.Contains(b.MauSac)));

            if (tinhTrang != null && tinhTrang.Length > 0)
            {
                var hasInStock = tinhTrang.Contains("con-hang");
                var hasOutOfStock = tinhTrang.Contains("het-hang");

                if (hasInStock && !hasOutOfStock)
                    query = query.Where(x => x.BienTheSanPham.Any(b => b.SoLuong > 0));

                if (!hasInStock && hasOutOfStock)
                    query = query.Where(x => !x.BienTheSanPham.Any(b => b.SoLuong > 0));
            }

            if (gia != null && gia.Length > 0)
            {
                var under500 = gia.Contains("under-500");
                var from500To1000 = gia.Contains("500-1000");
                var from1000To2000 = gia.Contains("1000-2000");
                var over2000 = gia.Contains("over-2000");

                query = query.Where(x =>
                    (under500 && x.Gia < 500000) ||
                    (from500To1000 && x.Gia >= 500000 && x.Gia <= 1000000) ||
                    (from1000To2000 && x.Gia > 1000000 && x.Gia <= 2000000) ||
                    (over2000 && x.Gia > 2000000));
            }

            query = sapXep switch
            {
                "price-asc" => query.OrderBy(x => x.Gia),
                "price-desc" => query.OrderByDescending(x => x.Gia),
                "name-asc" => query.OrderBy(x => x.TenSanPham),
                "name-desc" => query.OrderByDescending(x => x.TenSanPham),
                _ => query.OrderByDescending(x => x.NgayTao ?? DateTime.MinValue)
            };

            ViewBag.Query = q;
            ViewBag.DanhMucSelected = danhMuc ?? Array.Empty<string>();
            ViewBag.ThuongHieuSelected = thuongHieu ?? Array.Empty<string>();
            ViewBag.GioiTinh = gioiTinh ?? Array.Empty<string>();
            ViewBag.Size = size ?? Array.Empty<int>();
            ViewBag.MauSac = mauSac ?? Array.Empty<string>();
            ViewBag.TinhTrang = tinhTrang ?? Array.Empty<string>();
            ViewBag.Gia = gia ?? Array.Empty<string>();
            ViewBag.SapXep = sapXep;

            var data = await query.ToListAsync();
            return View(data);
        }

        // GET: /Customer/SanPham/TimKiem?q=...
        [HttpGet]
        public IActionResult TimKiem(string? q)
        {
            return RedirectToAction(nameof(Index), new { q });
        }
    }
}
