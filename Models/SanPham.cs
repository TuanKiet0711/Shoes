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
    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public string? MaDanhMuc { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
    public string? MaThuongHieu { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập giá")]
    public int Gia { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    public string? MoTa { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    public string? GioiTinh { get; set; }

    public virtual ICollection<BienTheSanPham> BienTheSanPham { get; set; } = new List<BienTheSanPham>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<HinhAnhSanPham> HinhAnhSanPham { get; set; } = new List<HinhAnhSanPham>();

    public virtual DanhMuc? MaDanhMucNavigation { get; set; }

    public virtual ThuongHieu? MaThuongHieuNavigation { get; set; }

    public virtual ICollection<TaiKhoan> MaTaiKhoan { get; set; } = new List<TaiKhoan>();
}
