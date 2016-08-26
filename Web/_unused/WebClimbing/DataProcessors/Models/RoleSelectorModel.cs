using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models.UserAuthentication;
using System.ComponentModel.DataAnnotations;

namespace WebClimbing.DataProcessors.Models
{
    public class RoleSelectorModel
    {
        public long UserId { get; set; }

        [Display(Name="Пользователь")]
        public String UserName { get; set; }

        [Display(Name="Регион")]
        public String RegionName { get; set; }

        [Display(Name="Роль")]
        public RoleEnum? Role { get; set; }

        public bool ReadOnly { get; set; }
    }
}