using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class ThongTinGiaoHang
{
    public string MaThongTinGiaoHang { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? HoTenNguoiNhan { get; set; }

    public string? SdtnguoiNhan { get; set; }

    public string? TinhThanh { get; set; }

    public string? QuanHuyen { get; set; }

    public string? PhuongXa { get; set; }

    public string? DiaChiNguoiNhan { get; set; }

    public DateOnly? NgayNhanHangDuKien { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
