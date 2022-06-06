using System.ComponentModel.DataAnnotations;

namespace Univer.ViewModels
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword",ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
