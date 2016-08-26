using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models;
using System.ComponentModel.DataAnnotations;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class Comp_AgeGroupEdit
    {
        public int GroupId { get; set; }

        [Display(Name = "Используется")]
        public bool Confirmed { get; set; }

        [Display(Name = "Группа")]
        public String Name { get; set; }

        public Comp_AgeGroupEdit() { GroupId = 0; }

        public Comp_AgeGroupEdit(AgeGroupModel grp, bool confirmed = false)
        {
            this.GroupId = grp.Iid;
            this.Name = grp.FullName;
            this.Confirmed = confirmed;
        }
    }
}