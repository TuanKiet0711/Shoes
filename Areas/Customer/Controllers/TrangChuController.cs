using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TrangChuController : Controller
    {
        private readonly WebBanGiayContext _context;

        public TrangChuController(WebBanGiayContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // ===== LOAD DANH MỤC & THƯƠNG HIỆU =====
            ViewBag.DanhMuc = await _context.DanhMuc.ToListAsync();
            ViewBag.ThuongHieu = await _context.ThuongHieu.ToListAsync();

            // ===== SẢN PHẨM NỔI BẬT (KHÔNG FILTER) =====
            var sanPhamNoiBat = await _context.SanPham
                .Include(sp => sp.HinhAnhSanPham)
                .Include(sp => sp.MaThuongHieuNavigation)
                .OrderByDescending(sp => sp.NgayTao ?? DateTime.MinValue)
                .Take(8)
                .ToListAsync();

            return View(sanPhamNoiBat);
        }
    }
}
