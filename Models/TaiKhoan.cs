using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class TaiKhoan
{
    public string MaTaiKhoan { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string Email { get; set; } = null!;

    public string? Sdt { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? VaiTro { get; set; }

    public string? TrangThai { get; set; }

    public string? TokenXacThucEmail { get; set; }

    public DateTime? HanTokenEmail { get; set; }

    public string? TokenDatLaiMatKhau { get; set; }

    public DateTime? HanDatLaiMatKhau { get; set; }

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<DonHang> DonHang { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHang { get; set; } = new List<GioHang>();

    public virtual TaiXe? TaiXe { get; set; }

    public virtual ICollection<SanPham> MaSanPham { get; set; } = new List<SanPham>();
}
