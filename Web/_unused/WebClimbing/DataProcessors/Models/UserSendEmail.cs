using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebClimbing.Models.UserAuthentication;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class UserSendEmail
    {
        public int UserId { get; set; }

        [Display(Name="Имя пользователя")]
        public String UserName { get; set; }

        [Display(Name = "Команда")]
        public String TeamName { get; set; }

        [Display(Name="E-mail")]
        public String Email { get; set; }

        [Display(Name="Отпарвить")]
        public bool Confirmed { get; set; }

        public UserSendEmail() { this.UserName = this.Email = this.TeamName = String.Empty; }

        public UserSendEmail(UserProfileModel model)
        {
            this.UserName = model.Name;
            this.Email = model.Email ?? String.Empty;
            this.TeamName = model.RegionId == null ? String.Empty : model.Region.Name;
            this.UserId = model.Iid;
        }
    }
}