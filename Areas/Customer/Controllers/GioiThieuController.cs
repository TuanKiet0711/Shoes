using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebBanGiay.Models;

namespace WebBanGiay.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioiThieuController : Controller
    {
        private readonly WebBanGiayContext _db;

        // Inject Database vào đây
        public GioiThieuController(WebBanGiayContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Phải lấy dữ liệu đổ vào ViewBag thì Layout mới hiển thị được
            ViewBag.DanhMuc = _db.DanhMuc.ToList(); 
            ViewBag.ThuongHieu = _db.ThuongHieu.ToList();

            return View();
        }
    }
}