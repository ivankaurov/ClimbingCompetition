using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebClimbing.ServiceClasses;
using System.Runtime.Serialization;

namespace WebClimbing.Models
{
    [Table("MVCAgeGroups"), DisplayColumn("FullName")]
    public class AgeGroupModel : IComparable<AgeGroupModel>, IValidatableObject
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Iid { get; set; }

        [Column("min_age"), Display(Name="Минимальный возраст")]
        public int? MinAge { get; set; }

        [Column("max_age"), Display(Name="Максимальный возраст")]
        public int? MaxAge { get; set; }

        [Required(ErrorMessage="Пол не выбран"), Column("gender"), Display(Name="Пол")]
        public int GenderCode { get; set; }
        [NotMapped]
        public Gender GenderProperty
        {
            get { return (Gender)GenderCode; }
            set { GenderCode = (int)value; }
        }

        [Required(ErrorMessage = "Наименование не введено", AllowEmptyStrings = false),
         Column("full_name"),
         MaxLength(255, ErrorMessage = "Не более 255 символов"),
         Display(Name="Полное наименование")
        ]
        public String FullName { get; set; }

        [Required(ErrorMessage = "Наименование не введено", AllowEmptyStrings = false),
         Column("secretary_name"),
         MaxLength(255, ErrorMessage = "Не более 255 символов"),
         Display(Name = "Наименование для протоколов")
        ]
        public String SecretaryName { get; set; }

        public virtual ICollection<Comp_AgeGroupModel> CompetitionGroups { get; set; }

        public override bool Equals(object obj)
        {
            AgeGroupModel other = obj as AgeGroupModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }
        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }

        public int CompareTo(AgeGroupModel other)
        {
            if (other == null)
                return 1;
            if (this.Iid == other.Iid)
                return 0;
            int n = (other.MaxAge ?? int.MaxValue).CompareTo(this.MaxAge ?? int.MaxValue);
            if (n != 0)
                return n;
            n = (other.MinAge ?? int.MinValue).CompareTo(this.MinAge ?? int.MinValue);
            if (n != 0)
                return n;
            n = this.GenderCode.CompareTo(other.GenderCode);
            if (n != 0)
                return n;
            return this.FullName.CompareTo(other.FullName);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int maxAge = MaxAge??int.MaxValue;
            int minAge = MinAge??int.MinValue;
            if (maxAge < minAge)
                yield return new ValidationResult("Максимальный возраст должен быть не менее минимального", new String[] { "MaxAge", "MinAge" });
        }

        public bool IsReadOnly
        {
            get
            {
                return (CompetitionGroups != null && CompetitionGroups.Count > 0);
            }
        }
    }

    public class AgeGroupModelWrapper
    {
        public AgeGroupModel Value { get; set; }
        public bool IsNew { get; set; }
        public bool ReadOnly { get; set; }
        public bool ToDelete { get; set; }
        [Display(Name="Подтвердить")]
        public bool Confirmed { get; set; }

        public String SecretaryNameChange { get; set; }
        public String FullNameChange { get; set; }
        public String GenderChange { get; set; }
        public String MinAgeChange { get; set; }
        public String MaxAgeChange { get; set; }


        public AgeGroupModelWrapper()
        {
            Value = new AgeGroupModel();
            IsNew = true;
            ReadOnly = false;
            ToDelete = false;
        }

        public AgeGroupModelWrapper(AgeGroupModel model, bool isNew = false, bool readOnly = false)
        {
            this.Value = model;
            this.IsNew = isNew;
            this.ReadOnly = readOnly;
            this.ToDelete = false;
            this.SecretaryNameChange = model.SecretaryName;
            this.FullNameChange = model.FullName;
            this.GenderChange = model.GenderProperty.GetFriendlyValue();
            this.MinAgeChange = model.MinAge.GetStringValue();
            this.MaxAgeChange = model.MaxAge.GetStringValue();
        }
    }
}