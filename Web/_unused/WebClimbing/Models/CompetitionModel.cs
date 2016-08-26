using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WebClimbing.Models.UserAuthentication;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace WebClimbing.Models
{
    [Flags]
    public enum CompetitionLevel : long
    {
        Empty = 0,
        Regional = 1,
        National = 2,
        International = 4,
        NationalOrInternational = (National | International),
        Adult = 0x10,
        Youth = 0x20,
        Lead = 0x100,
        Speed = 0x200,
        Boulder = 0x400,
        Combined = 0x800,
        Relay = 0x1000
    }

    [Table("MVCCompetitions")]
    [DisplayColumn("ShortName", "Start")]
    public class CompetitionModel
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Введите полное название соревнований"),
        Column("full_name"),
        Display(Name = "Название соревнований"),
        MaxLength(4000)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Введите краткое наименование соревнований"),
        Column("short_name"),
        MaxLength(100),
        Display(Name = "Краткое наименование")]
        public string ShortName { get; set; }

        [Display(Name = "Год"), NotMapped]
        public int? Year { get { return Start.Year; } }

        [Column("date_start"), Display(Name = "Дата начала")]
        public DateTime Start { get; set; }

        [Column("date_end"), Display(Name = "Дата окончания")]
        public DateTime End { get; set; }

        [Column("applications_end"), Display(Name = "Окончание приема заявок")]
        public DateTime ApplicationsEnd { get; set; }

        [Column("app_edit_end"), Display(Name = "Окончание корректировки заявок")]
        public DateTime ApplicationsEditEnd { get; set; }

        [Required, Column("description")]
        public long DescriptionCode { get; set; }

        
        [NotMapped]
        public bool SignApplications
        {
            get { return this[CompetitionParamType.SignApplications].BooleanValue; }
            set { this[CompetitionParamType.SignApplications].BooleanValue = value; }
        }

        [NotMapped]
        public bool AllowMultipleTeams
        {
            get { return this[CompetitionParamType.AllowMultipleTeams].BooleanValue; }
            set { this[CompetitionParamType.AllowMultipleTeams].BooleanValue = value; }
        }

        [Required, Column("allow_new_corr"), Display(Name = "Разрешить поздние заявки")]
        public bool AllowLateAppl { get; set; }

        [NotMapped]
        public CompetitionLevel CompetitionDescription
        {
            get
            {
                try { return (CompetitionLevel)DescriptionCode; }
                catch { return CompetitionLevel.Empty; }
            }
            set { DescriptionCode = (long)value; }
        }

        [NotMapped, Display(Name = "Ранг")]
        public CompetitionLevel Rank
        {
            get
            {
                if ((CompetitionDescription & CompetitionLevel.International) > 0)
                    return CompetitionLevel.International;
                if ((CompetitionDescription & CompetitionLevel.National) > 0)
                    return CompetitionLevel.National;
                return CompetitionLevel.Regional;
            }
            set
            {
                CompetitionLevel current = CompetitionDescription;
                current = current & (~CompetitionLevel.National) & (~CompetitionLevel.International);
                if ((value & CompetitionLevel.National) > 0)
                    current = current | CompetitionLevel.National;
                else if ((value & CompetitionLevel.International) > 0)
                    current = current | CompetitionLevel.International;
                else
                    current = current | CompetitionLevel.Regional;
                CompetitionDescription = current;
            }
        }

        [NotMapped, Display(Name = "Группы")]
        public CompetitionLevel Groups
        {
            get
            {
                if ((CompetitionDescription & CompetitionLevel.Youth) > 0)
                    return CompetitionLevel.Youth;
                else
                    return CompetitionLevel.Adult;
            }
            set
            {
                CompetitionLevel current = CompetitionDescription;
                current = current & (~CompetitionLevel.Adult) & (~CompetitionLevel.Youth);
                if ((value & CompetitionLevel.Youth) > 0)
                    current = current | CompetitionLevel.Youth;
                else
                    current = current | CompetitionLevel.Adult;
                CompetitionDescription = current;
            }
        }

        private static bool CheckEnumValue(CompetitionLevel source, CompetitionLevel check) { return ((source & check) == check); }
        private CompetitionLevel SetEnumValue(CompetitionLevel source, CompetitionLevel value, bool reset = false)
        {
            if (reset)
                return (source & ~(value));
            else
                return source | value;
        }

        #region Styles

        [NotMapped, Display(Name = "Трудность")]
        public bool Lead
        {
            get { return CheckEnumValue(CompetitionDescription, CompetitionLevel.Lead); }
            set { CompetitionDescription = SetEnumValue(CompetitionDescription, CompetitionLevel.Lead, !value); }
        }

        [NotMapped, Display(Name = "Скорость")]
        public bool Speed
        {
            get { return CheckEnumValue(CompetitionDescription, CompetitionLevel.Speed); }
            set { CompetitionDescription = SetEnumValue(CompetitionDescription, CompetitionLevel.Speed, !value); }
        }

        [NotMapped, Display(Name = "Боулдеринг")]
        public bool Boulder
        {
            get { return CheckEnumValue(CompetitionDescription, CompetitionLevel.Boulder); }
            set { CompetitionDescription = SetEnumValue(CompetitionDescription, CompetitionLevel.Boulder, !value); }
        }

        [NotMapped, Display(Name = "Эстафета")]
        public bool Relay
        {
            get { return CheckEnumValue(CompetitionDescription, CompetitionLevel.Relay); }
            set { CompetitionDescription = SetEnumValue(CompetitionDescription, CompetitionLevel.Relay, !value); }
        }

        [NotMapped, Display(Name = "Многоборье")]
        public bool Combined
        {
            get { return CheckEnumValue(CompetitionDescription, CompetitionLevel.Combined); }
            set { CompetitionDescription = SetEnumValue(CompetitionDescription, CompetitionLevel.Combined, !value); }
        }

        #endregion

        [Required, ForeignKey("Region"), Column("region_id"), Display(Name = "Регион проведения")]
        public long RegionId { get; set; }

        public virtual RegionModel Region { get; set; }

        public virtual ICollection<CompetitionParameterModel> Parameters { get; set; }

        public virtual ICollection<UserRoleModel> Users { get; set; }

        public virtual ICollection<Comp_AgeGroupModel> AgeGroups { get; set; }

        public virtual ICollection<Comp_CompetitiorRegistrationModel> Climbers { get; set; }

        public virtual ICollection<ListHeaderModel> Lists { get; set; }

        public override bool Equals(object obj)
        {
            CompetitionModel other = obj as CompetitionModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }
        [NotMapped]
        public CompetitionParameterModel this[CompetitionParamType paramType]
        {
            get
            {
                String paramName = paramType.ToString("G");
                var selectedP = Parameters.FirstOrDefault(p => p.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase));
                if (selectedP != null)
                    return selectedP;
                CompetitionParameterModel model = new CompetitionParameterModel
                {
                    StringValue = String.Empty,
                    BinaryValue = null,
                    CompetitionIid = this.Iid,
                    ParamType = paramType
                };
                this.Parameters.Add(model);
                return model;
            }
            set
            {
                var param = this[paramType];
                param.BinaryValue = value.BinaryValue;
                param.StringValue = value.StringValue;
            }
        }

        public byte[] GenerateKeyPair()
        {
            using (var provider = new DSACryptoServiceProvider())
            {
                BinaryFormatter fmt = new BinaryFormatter();
                using (var mstr = new MemoryStream())
                {
                    fmt.Serialize(mstr, provider.ExportParameters(false));
                    this[CompetitionParamType.SignatureKey].BinaryValue = mstr.ToArray();
                }
                return Encoding.Unicode.GetBytes(provider.ToXmlString(true));
            }
        }

        public List<AgeGroupModel> AgeGroupsList()
        {
            var res = this.AgeGroups.ToList().Select(a => a.AgeGroup).Distinct().ToList();
            res.Sort();
            return res;
        }

        public AllowedActions GetAllowedActions(UserProfileModel user, DateTime? dateTime = null)
        {
            if (user == null)
                return AllowedActions.Nothing;
            var checkDateTime = dateTime ?? DateTime.UtcNow;
            if (checkDateTime > this.Start)
                return AllowedActions.Nothing;
            if (user.IsInRoleComp(RoleEnum.Admin, this))
                return AllowedActions.FullApp;
            if (!user.IsInRoleComp(RoleEnum.User, this))
                return AllowedActions.Nothing;
            if (checkDateTime <= this.ApplicationsEnd || (checkDateTime <= this.ApplicationsEditEnd && this.AllowLateAppl))
                return AllowedActions.FullApp;
            if (checkDateTime <= this.ApplicationsEditEnd)
                return AllowedActions.Change;
            return AllowedActions.Nothing;
        }

        public bool AllowedToEdit(UserProfileModel user, DateTime? dateTime = null)
        {
            return ((this.GetAllowedActions(user, dateTime) & AllowedActions.Change) == AllowedActions.Change);
        }

        public bool AllowedToAdd(UserProfileModel user, DateTime? dateTime = null)
        {
            return ((this.GetAllowedActions(user, dateTime) & AllowedActions.Add) == AllowedActions.Add);
        }
    }

    [Flags]
    public enum AllowedActions
    {
        Nothing = 0x0,
        Change = 0x1,
        Add = 0x2,
        FullApp = (Change | Add)
    }

    public sealed class CompetitionGroupEditModel
    {
        [Display(Name = "Группа")]
        public AgeGroupModel AgeGroup { get; set; }
        [Display(Name = "Используется")]
        public bool Confirmed { get; set; }

        public CompetitionGroupEditModel()
        {
            this.Confirmed = false;
            this.AgeGroup = null;
        }

        public CompetitionGroupEditModel(AgeGroupModel model)
        {
            this.AgeGroup = model;
            this.Confirmed = false;
        }
    }
}