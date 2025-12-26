using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class ThanhToan
{
    public string MaThanhToan { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? PhuongThuc { get; set; }

    public int? SoTien { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? ThoiGianThanhToan { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
