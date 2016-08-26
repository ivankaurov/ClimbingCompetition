using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class RegionAdminModel
    {
        public int UserId { get; set; }

        [Display(Name = "Пользователь")]
        public String UserName { get; set; }

        [Display(Name = "E-mail")]
        public String UserEmail { get; set; }

        [Display(Name = "Регион")]
        public String UserRegion { get; set; }

        [Display(Name = "Администратор")]
        public bool IsAdmin { get; set; }
    }
}