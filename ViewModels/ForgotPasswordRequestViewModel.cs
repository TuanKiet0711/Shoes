using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.ViewModels;

public class ForgotPasswordRequestViewModel
{
    private const string EmailRegex = @"^[A-Za-z0-9](?:[A-Za-z0-9._%+-]{0,62}[A-Za-z0-9])?@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,24}$";

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [RegularExpression(EmailRegex, ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;
}
