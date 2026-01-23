using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.ViewModels.Admin;

public class TaiKhoanCreateViewModel
{
    private const string EmailRegex = @"^[A-Za-z0-9](?:[A-Za-z0-9._%+-]{0,62}[A-Za-z0-9])?@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,24}$";

    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [RegularExpression(EmailRegex, ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    public string? Sdt { get; set; }

    public string? GioiTinh { get; set; }

    public string? NgaySinh { get; set; }

    [Required(ErrorMessage = "Vai trò không được để trống.")]
    public string VaiTro { get; set; } = "KhachHang";

    [Required(ErrorMessage = "Trạng thái không được để trống.")]
    public string TrangThai { get; set; } = "HoatDong";

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
    [Compare(nameof(MatKhau), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}
