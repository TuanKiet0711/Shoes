using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class GiamGia
{
    public string MaGiamGia { get; set; } = null!;

    public string? TenMa { get; set; }

    public string? LoaiGiamGia { get; set; }

    public int? GiaTriGiam { get; set; }

    public int? GiaTriDonToiThieu { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHang { get; set; } = new List<DonHang>();
}
