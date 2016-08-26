using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebClimbing.ServiceClasses;

namespace WebClimbing.Models
{
    
    [EnumCustomDisplay]
    public enum Gender
    {
        [Display(Name = "Мужской")]
        Male = 0,
        [Display(Name = "Женский")]
        Female = 1
    }

    [Table("MVCPeople"), DisplayColumn("FullName")]
    public class PersonModel : IComparable<PersonModel>
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        private string surname;
        [Column("surname"), Required(AllowEmptyStrings = false), MaxLength(255), Display(Name = "Фамилия")]
        public string Surname { get { return surname ?? String.Empty; } set { surname = value; } }

        private string name;
        [Column("given_name"), Required(AllowEmptyStrings = false), MaxLength(255), Display(Name = "Имя")]
        public string Name { get { return name ?? String.Empty; } set { name = value; } }

        private string patronymic;
        [Column("patronymic"), MaxLength(255), Display(Name = "Отчество")]
        public string Patronymic { get { return patronymic ?? String.Empty; } set { patronymic = value; } }

        [NotMapped, Display(Name = "Фамилия, Имя")]
        public string Fullname { get { return Surname + " " + Name; } }

        [Column("birthdate"), Required, Display(Name = "Дата рождения"), DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [NotMapped, Display(Name = "Год рождения")]
        public int YearOfBirth { get { return DateOfBirth.Year; } }

        private string email;
        [Column("email"), DataType(DataType.EmailAddress), Display(Name = "E-mail")]
        public String Email { get { return email ?? String.Empty; } set { email = value; } }

        private string homeaddress;
        [Column("address"), MaxLength(1024), Display(Name = "Адрес")]
        public String HomeAddress { get { return homeaddress ?? String.Empty; } set { homeaddress = value; } }

        private string coach;
        [Column("coach"), MaxLength(1024), Display(Name = "Тренер")]
        public String Coach { get { return coach ?? String.Empty; } set { coach = value; } }

        [Required, Column("genderFemale"), Display(Name="Пол")]
        public bool GenderFemale { get; set; }

        public Gender GenderProperty
        {
            get { return GenderFemale ? Gender.Female : Gender.Male; }
            set { this.GenderFemale = (value == Gender.Female); }
        }

        public virtual ICollection<Comp_CompetitiorRegistrationModel> Competitions { get; set; }
        public virtual ICollection<PersonPictureModel> Pictures { get; set; }

        public RegionModel[] Regions
        {
            get
            {
                if (Competitions == null)
                    return new RegionModel[0];
                var lst = this.Competitions
                            .SelectMany(c => c.Teams)
                            .Select(t => t.Region)
                            .Distinct()
                            .ToList();
                lst.Sort();
                return lst.ToArray();
            }
        }

        public int CompareTo(PersonModel other)
        {
            if (other == null)
                return 1;
            if (this.Iid == other.Iid)
                return 0;
            int n = this.Surname.CompareTo(other.Surname);
            if (n != 0)
                return n;
            n = this.Name.CompareTo(other.Name);
            if (n != 0)
                return n;
            n = this.Patronymic.CompareTo(other.Patronymic);
            if (n != 0)
                return n;
            return this.DateOfBirth.CompareTo(other.DateOfBirth);
        }
        public override bool Equals(object obj)
        {
            var other = obj as PersonModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }
        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }
    }

    [Table("MVCPeoplePictures")]
    public class PersonPictureModel
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("iid_parent"), ForeignKey("Person"), Required]
        public long PersonId { get; set; }
        public virtual PersonModel Person { get; set; }

        [Column("picture_date"), Required, Display(Name = "Дата снимка")]
        public DateTime PictureDate { get; set; }

        [Column("picture", TypeName = "image"), Required]
        public byte[] Image { get; set; }
    }

    public class PersonEditModel : IComparable<PersonEditModel>, IValidatableObject
    {
        public bool ReadOnly { get; set; }

        private bool confirmed = false;
        public bool Confirmed { get { return confirmed; } set { confirmed = value; } }

        private bool toDelete = false;
        public bool Deleted { get { return toDelete; } set { toDelete = value; } }

        public long? initialId { get; set; }

        public int? prefixIndex { get; set; }

        private string surname;
        [MaxLength(255), Display(Name = "Фамилия")]
        public string Surname { get { return surname ?? String.Empty; } set { surname = value; } }

        public string ChangeSurname { get; set; }

        private string name;
        [MaxLength(255), Display(Name = "Имя")]
        public string Name { get { return name ?? String.Empty; } set { name = value; } }

        public string ChangeName { get; set; }

        private string patronymic;
        [MaxLength(255), Display(Name = "Отчество")]
        public string Patronymic { get { return patronymic ?? String.Empty; } set { patronymic = value; } }

        public string ChangePatronymic { get; set; }

        [/*Required, */Display(Name = "Дата рождения")]
        public DateTime? DateOfBirth { get; set; }

        public string ChangeDateOfBirth { get; set; }

        private string email;
        [DataType(DataType.EmailAddress, ErrorMessage="Email недействителен"), Display(Name="E-mail")]
        public String Email { get { return email ?? String.Empty; } set { email = value; } }

        public string ChangeEmail { get; set; }

        [Display(Name="Пол")]
        public Gender? GenderProperty { get; set; }

        public string ChangeGender { get; set; }

        private String homeaddress;
        [MaxLength(1024), Display(Name = "Адрес")]
        public String HomeAddress { get { return homeaddress ?? String.Empty; } set { homeaddress = value; } }

        public string ChangeHomeAddress { get; set; }

        private String coach;
        [MaxLength(1024), Display(Name = "Тренер")]
        public String Coach { get { return coach ?? String.Empty; } set { coach = value; } }

        public String ChangeCoach { get; set; }
                
        public PersonEditModel()
        {
            this.Surname = this.Name = this.Patronymic = this.Email = this.Coach = this.HomeAddress = String.Empty;
            this.New = true;
        }

        public PersonEditModel(PersonModel p)
        {
            this.Surname = p.Surname;
            this.Name = p.Name;
            this.Patronymic = p.Patronymic;
            this.DateOfBirth = p.DateOfBirth;
            this.initialId = p.Iid;
            this.Email = p.Email;
            this.Coach = p.Coach;
            this.HomeAddress = p.HomeAddress;
            this.GenderProperty = p.GenderProperty;
        }

        public static explicit operator PersonEditModel(PersonModel p)
        {
            return new PersonEditModel(p);
        }

        public bool New { get; set; }

        public int CompareTo(PersonEditModel other)
        {
            if (other == null)
                return 1;
            int n = this.Surname.CompareTo(other.Surname);
            if (n != 0)
                return n;
            n = this.Name.CompareTo(other.Name);
            if (n != 0)
                return n;
            n = this.Patronymic.CompareTo(other.Patronymic);
            if (n != 0 || !this.DateOfBirth.HasValue || !other.DateOfBirth.HasValue)
                return n;
            return this.DateOfBirth.Value.CompareTo(other.DateOfBirth.Value);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(Surname))
                yield return new ValidationResult("Фамилия не введена", new[] { "Surname" });
            if(String.IsNullOrEmpty(Name))
                yield return new ValidationResult("Имя не введено", new[]{"Name"});
            if (DateOfBirth == null)
                yield return new ValidationResult("Дата рождения не введена", new[] { "DateOfBirth" });
            if (GenderProperty == null)
                yield return new ValidationResult("Пол не указан", new[] { "GenderProperty" });
            foreach (var p in this.GetType().GetProperties())
            {
                foreach (var atr in Attribute.GetCustomAttributes(p, typeof(ValidationAttribute)).Select(a=>a as ValidationAttribute))
                {
                    var pValue = p.GetValue(this, null);
                    var vRes = atr.GetValidationResult(pValue, validationContext);
                    if (vRes != null)
                        yield return vRes;
                }
            }
        }
    }
}