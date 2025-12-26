using System;
using System.Collections.Generic;

namespace WebBanGiay.Models;

public partial class TaiXe
{
    public string MaTaiXe { get; set; } = null!;

    public string? MaTaiKhoan { get; set; }

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<PhanCongTaiXe> PhanCongTaiXe { get; set; } = new List<PhanCongTaiXe>();
}
