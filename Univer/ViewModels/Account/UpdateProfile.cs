using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Univer.ViewModels
{
    public class UpdateProfile
    {
        [Required(ErrorMessage = "Поле не должно быть пустым")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле не должно быть пустым")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile profileImageFile { get; set; }    
    }
}
