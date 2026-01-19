using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    public string GioiTinh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    [RegularExpression(@"^\d{2}/\d{2}/\d{4}$", ErrorMessage = "Ngày sinh phải theo định dạng dd/MM/yyyy.")]
    public string NgaySinh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
    public string Sdt { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu.")]
    [Compare(nameof(MatKhau), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}
