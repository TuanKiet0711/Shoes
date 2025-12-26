using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class ThuongHieu
{
    public string MaThuongHieu { get; set; } = null!;

    public string TenThuongHieu { get; set; } = null!;

    public virtual ICollection<SanPham> SanPham { get; set; } = new List<SanPham>();
}
