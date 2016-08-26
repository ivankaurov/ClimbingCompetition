using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Extensions;
using ClimbingCompetition.Common;

namespace ClimbingEntities.Competitions
{
    [Table("cl_cm_comp_parameters")]
    public class CompetitionParameter : ClimbingBaseObject
    {
        protected CompetitionParameter() { }

        public CompetitionParameter(ClimbingContext2 context) : base(context) { }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        public string CompId { get; set; }

        [ForeignKey("CompId")]
        public virtual Competition Competition { get; set; }

        [Column("param_id")]
        [Required]
        [MaxLength(2 * IID_SIZE)]
        public String ParamIdString { get; protected set; }

        [NotMapped]
        public CompetitionParamId ParamId
        {
            get
            {
                return ParamIdString.GetEnumValue<CompetitionParamId>() ?? default(CompetitionParamId);
            }
            set { this.ParamIdString = value.GetEnumValueString(); }
        }

        [Column("string_value")]
        public String StringValue { get; set; }

        [NotMapped]
        public DateTime? DateTimeValue
        {
            get { return StringValue.GetFormalizedDate(); }
            set { StringValue = value.GetFormalizedDateString(); }
        }

        [NotMapped]
        public Double NumericValue
        {
            get { return StringValue.GetNumericValue() ?? 0.0; }
            set { value.GetNumericValueString(); }
        }

        [Column("binary_value", TypeName = "varbinary(max)")]
        public byte[] BinaryValue { get; set; }

        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.CompetitionParameters.Remove(this);
        }
    }
}
