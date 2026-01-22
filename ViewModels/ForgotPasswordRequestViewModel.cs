using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.ViewModels;

public class ForgotPasswordRequestViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;
}
