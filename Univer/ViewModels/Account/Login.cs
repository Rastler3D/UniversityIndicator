using System.ComponentModel.DataAnnotations;

namespace Univer.ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    
    }
}
