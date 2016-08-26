using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebClimbing.Models.UserAuthentication
{
    public sealed class UserRegistrationModel
    {
        public int? UserId { get; set; }

        [Display(Name="Имя пользователя")]
        [Required(AllowEmptyStrings=false, ErrorMessage="Введите имя пользователя")]
        public String UserName { get; set; }

        [Required(AllowEmptyStrings=false, ErrorMessage="Введите e-mail")]
        [DataType(DataType.EmailAddress, ErrorMessage="Введите действительный e-mail")]
        public String Email { get; set; }

        [Display(Name="Регион")]
        public long? Region { get; set; }
    }

    public sealed class UserActivationModel
    {
        [Display(Name = "Имя пользователя")]
        public String UserName { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Введите пароль")]
#if !DEBUG
        [MinLength(6, ErrorMessage="Пароль должен быть не менее 6 символов")]
#endif
        public String Password { get; set; }

        [Display(Name = "Подтверждение пароля"), Compare("Password", ErrorMessage = "Пароли не совпадают"), DataType(DataType.Password)]
        public String PasswordConfirm { get; set; }

        public String Token { get; set; }
    }

    public sealed class UserEditModel
    {
        public int Iid { get; set; }

        [Display(Name = "Имя пользователя")]
        [Required(AllowEmptyStrings=false, ErrorMessage="Введите имя пользователя")]
        public String UserName { get; set; }

        [Display(Name = "E-mail")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Введите e-mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Введите действительный e-mail")]
        public String Email { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public String PasswordConfirm { get; set; }

        [Display(Name = "Регион")]
        public long? RegionId { get; set; }
    }
}