using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class DanhGia
{
    public string MaDanhGia { get; set; } = null!;

    public string? MaSanPham { get; set; }

    public string? MaTaiKhoan { get; set; }

    public int? SoSao { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
}
