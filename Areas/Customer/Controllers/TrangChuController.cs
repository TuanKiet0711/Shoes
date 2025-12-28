using Microsoft.AspNetCore.Mvc;
using WebBanGiay.Models;
using Microsoft.EntityFrameworkCore;

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
            ViewBag.DanhMuc = await _context.DanhMuc.ToListAsync();
            ViewBag.ThuongHieu = await _context.ThuongHieu.ToListAsync();

            return View();
        }
    }
}
