using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class HinhAnhSanPham
{
    public string MaHinhAnh { get; set; } = null!;

    public string? MaSanPham { get; set; }

    public string? DuongDanAnh { get; set; }

    public bool? LaAnhDaiDien { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
