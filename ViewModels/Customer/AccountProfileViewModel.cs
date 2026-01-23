using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.ViewModels.Customer;

public class AccountProfileViewModel
{
    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    public string HoTen { get; set; } = string.Empty;

    public string? GioiTinh { get; set; }

    [RegularExpression(@"^(\d{2}/\d{2}/\d{4})?$", ErrorMessage = "Ngày sinh phải theo định dạng dd/MM/yyyy.")]
    public string? NgaySinh { get; set; }

    [RegularExpression(@"^(\d{10})?$", ErrorMessage = "Số điện thoại phải đủ 10 số.")]
    public string? Sdt { get; set; }

    [DataType(DataType.Password)]
    public string? MatKhauHienTai { get; set; }

    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string? MatKhauMoi { get; set; }

    [Compare(nameof(MatKhauMoi), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    [DataType(DataType.Password)]
    public string? XacNhanMatKhau { get; set; }
}
