using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class ChiTietGioHang
{
    public string MaChiTietGioHang { get; set; } = null!;

    public string? MaGioHang { get; set; }

    public string? MaBienThe { get; set; }

    public int? SoLuong { get; set; }

    public int? TongTien { get; set; }

    public virtual BienTheSanPham? MaBienTheNavigation { get; set; }

    public virtual GioHang? MaGioHangNavigation { get; set; }
}
