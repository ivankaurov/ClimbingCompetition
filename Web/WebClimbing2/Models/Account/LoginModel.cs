using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebClimbing2.Models.Account
{
    public class LoginModel
    {
        [Display(Name = "Имя пользователя или e-mail", Order = 1)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Display(Name = "Пароль", Order = 2)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня на этом устройстве", Order = 3)]
        public bool RememberMe { get; set; }

        [Display(Order = 999)]
        public string ReturnUrl { get; set; }
    }
}