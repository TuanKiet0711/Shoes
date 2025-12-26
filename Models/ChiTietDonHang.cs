using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class ChiTietDonHang
{
    public string MaChiTietDonHang { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? MaBienThe { get; set; }

    public int? GiaTaiThoiDiem { get; set; }

    public int? SoLuong { get; set; }

    public virtual BienTheSanPham? MaBienTheNavigation { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
