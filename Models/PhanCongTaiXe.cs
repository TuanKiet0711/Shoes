using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class PhanCongTaiXe
{
    public string MaPhanCong { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? MaTaiXe { get; set; }

    public string? TrangThaiGiao { get; set; }

    public int? PhiGiaoHang { get; set; }

    public DateTime? ThoiGianPhanCong { get; set; }

    public DateTime? ThoiGianNhanHang { get; set; }

    public DateTime? ThoiGianLayHang { get; set; }

    public DateTime? ThoiGianHoanThanh { get; set; }

    public string? LyDoThatBai { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual TaiXe? MaTaiXeNavigation { get; set; }
}
