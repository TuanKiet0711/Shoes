using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class GioHang
{
    public string MaGioHang { get; set; } = null!;

    public string? MaTaiKhoan { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHang { get; set; } = new List<ChiTietGioHang>();

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
}
