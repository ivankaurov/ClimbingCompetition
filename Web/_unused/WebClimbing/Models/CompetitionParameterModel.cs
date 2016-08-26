using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace WebClimbing.Models
{

    public enum CompetitionParamType
    {
        SignatureKey,
        SignApplications,
        AllowMultipleTeams
    }

    [Table("MVCCompetitionParams")]
    public class CompetitionParameterModel
    {
        private static CultureInfo DefaultCulture = new CultureInfo("en-US");

        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("param_name"), Display(Name = "Параметр"), Required(AllowEmptyStrings = false), MaxLength(50)]
        public String Name { get; set; }

        public CompetitionParamType ParamType
        {
            get { return (CompetitionParamType)Enum.Parse(typeof(CompetitionParamType), Name, true); }
            set { Name = value.ToString("G"); }
        }

        [Required, ForeignKey("Competition"), Column("comp_id")]
        public long CompetitionIid { get; set; }

        public virtual CompetitionModel Competition { get; set; }

        [MaxLength(4000, ErrorMessage = "Разрешено не более 8000 символов"), Column("value")]
        public string StringValue { get; set; }

        [Column("binary_value", TypeName="image")]
        public byte[] BinaryValue { get; set; }

        [NotMapped]
        public long Int64Value
        {
            get { return Int64.Parse(StringValue); }
            set { StringValue = value.ToString(); }
        }

        [NotMapped]
        public bool BooleanValue
        {
            get { return String.IsNullOrEmpty(StringValue) ? false : !StringValue.Equals("0", StringComparison.Ordinal); }
            set { StringValue = (value ? "1" : String.Empty); }
        }

        [NotMapped]
        public int Int32Value
        {
            get { unchecked { return (int)Int64Value; } }
            set { Int64Value = value; }
        }

        [NotMapped]
        public DateTime DateTimeValue
        {
            get { return DateTime.Parse(StringValue, DefaultCulture); }
            set { StringValue = value.ToString(DefaultCulture); }
        }

        [NotMapped]
        public DateTime DateValue
        {
            get { return DateTimeValue.Date; }
            set { DateTimeValue = value; }
        }

        [NotMapped]
        public double DoubleValue
        {
            get { return double.Parse(StringValue, DefaultCulture); }
            set { StringValue = value.ToString(DefaultCulture); }
        }
    }
}