using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebClimbing.Models.UserAuthentication;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using XmlApiData;

namespace WebClimbing.Models
{

    [Table("MVCRegions"), DisplayColumn("Name")]
    public class RegionModel : IComparable<RegionModel>
    {
        public const int CODE_LENGTH = 50;
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("sym_code"), Required(AllowEmptyStrings = false), StringLength(CODE_LENGTH, MinimumLength = 1)]
        public String SymCode { get; set; }

        [Column("iid_parent"), ForeignKey("Parent")]
        public long? IidParent { get; set; }

        [Display(Name = "Регион - владелец")]
        public virtual RegionModel Parent { get; set; }

        [Column("name"), Required(AllowEmptyStrings = false), MaxLength(255), Display(Name = "Наименование")]
        public string Name { get; set; }

        public virtual ICollection<CompetitionModel> CompetitionsHold { get; set; }

        public virtual ICollection<Comp_ClimberTeam> PeopleCompetitions { get; set; }

        public virtual ICollection<UserProfileModel> Users { get; set; }

        public virtual ICollection<UserRoleModel> UserRoles { get; set; }

        public virtual ICollection<RegionModel> Children { get; set; }

        public PersonModel[] Climbers
        {
            get
            {
                if (PeopleCompetitions == null)
                    return new PersonModel[0];
                var lst = PeopleCompetitions
                           .Select(pc => pc.Climber)
                           .Select(clm => clm.Person)
                           .Distinct()
                           .ToList();
                lst.Sort();
                return lst.ToArray();
            }
        }

        public override bool Equals(object obj)
        {
            RegionModel other = obj as RegionModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }

        public int CompareTo(RegionModel other)
        {
            if (other == null)
                return 1;
            if (this.Parent == null && other.Parent != null)
                return -1;
            if (this.Parent != null && other.Parent == null)
                return 1;
            if (this.Parent != null && other.Parent != null)
            {
                int n = this.Parent.CompareTo(other.Parent);
                if (n != 0)
                    return n;
            }
            return this.Name.CompareTo(other.Name);
        }
    }
}