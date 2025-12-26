using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class DonHang
{
    public string MaDonHang { get; set; } = null!;

    public string? MaTaiKhoan { get; set; }

    public string? MaGiamGia { get; set; }

    public DateTime? NgayDatHang { get; set; }

    public int? TamTinh { get; set; }

    public int? PhiVanChuyen { get; set; }

    public int? TienGiamGia { get; set; }

    public int? TongCong { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public string? TrangThaiThanhToan { get; set; }

    public string? TrangThaiDonHang { get; set; }

    public string? LyDoHuy { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; } = new List<ChiTietDonHang>();

    public virtual GiamGia? MaGiamGiaNavigation { get; set; }

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<PhanCongTaiXe> PhanCongTaiXe { get; set; } = new List<PhanCongTaiXe>();

    public virtual ICollection<ThanhToan> ThanhToan { get; set; } = new List<ThanhToan>();

    public virtual ICollection<ThongTinGiaoHang> ThongTinGiaoHang { get; set; } = new List<ThongTinGiaoHang>();
}
