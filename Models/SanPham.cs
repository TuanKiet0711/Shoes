using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.Models;

public partial class SanPham
{
    [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm")]
    public string MaSanPham { get; set; } = null!;
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    public string TenSanPham { get; set; } = null!;

    public string? MaDanhMuc { get; set; }

    public string? MaThuongHieu { get; set; }
   
    public int Gia { get; set; }

    public string? MoTa { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? GioiTinh { get; set; }

    public virtual ICollection<BienTheSanPham> BienTheSanPham { get; set; } = new List<BienTheSanPham>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<HinhAnhSanPham> HinhAnhSanPham { get; set; } = new List<HinhAnhSanPham>();

    public virtual DanhMuc? MaDanhMucNavigation { get; set; }

    public virtual ThuongHieu? MaThuongHieuNavigation { get; set; }

    public virtual ICollection<TaiKhoan> MaTaiKhoan { get; set; } = new List<TaiKhoan>();
}
