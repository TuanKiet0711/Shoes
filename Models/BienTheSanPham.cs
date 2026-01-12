using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.Models;

public partial class BienTheSanPham
{
    [Required(ErrorMessage = "Vui lòng nhập mã biến thể")]
    public string MaBienThe { get; set; } = null!;
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    public string? MaSanPham { get; set; }
    
    public int? Size { get; set; }
    
    public string? MauSac { get; set; }
    
    public int? SoLuong { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHang { get; set; } = new List<ChiTietGioHang>();

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
