using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class DanhMuc
{
    public string MaDanhMuc { get; set; } = null!;

    public string TenDanhMuc { get; set; } = null!;

    public virtual ICollection<SanPham> SanPham { get; set; } = new List<SanPham>();
}
