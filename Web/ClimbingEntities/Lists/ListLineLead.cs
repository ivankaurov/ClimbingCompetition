using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_lead")]
    public class ListLineLead : ListLine
    {
        public const long DNS = -2, DSQ = -1;
         
        protected ListLineLead() { }
        public ListLineLead(ClimbingContext2 context) : base(context) { }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsLead.Remove(this);
        }

        [Column("time_value")]
        public long TimeValue { get; set; }
        [Column("time_text"), MaxLength(IID_SIZE)]
        public String TimeText { get; set; }

        protected override int CompareResult(ListLine other)
        {
            return other.Result.Value.CompareTo(this.Result.Value);
        }

        protected override bool IsDns()
        {
            return DNS == this.Result;
        }

        protected override bool IsDsq()
        {
            return DSQ == this.Result;
        }

        public override bool HasResult()
        {
            return this.Result.HasValue && !string.IsNullOrEmpty(this.ResText);
        }
    }
}
