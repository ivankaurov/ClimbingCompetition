using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_speed")]
    public class ListLineSpeed : ListLine
    {
        public const long FALL = long.MaxValue / 3,
                         DSQ = long.MaxValue / 3 + 100,
                         DNS = long.MaxValue / 3 + 500;
                         

        protected ListLineSpeed() { }
        public ListLineSpeed(ClimbingContext2 context) : base(context) { }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsSpeed.Remove(this);
        }

        [Column("route1")]
        public long Route1 { get; set; }

        [Column("route1_text"), MaxLength(IID_SIZE)]
        public String Route1Text { get; set; }

        [Column("route2")]
        public long Route2 { get; set; }

        [Column("route2_text"), MaxLength(IID_SIZE)]
        public String Route2Text { get; set; }

        protected override bool IsDns()
        {
            return DNS == this.Result;
        }

        protected override bool IsDsq()
        {
            return DSQ == this.Result;
        }

        protected override bool IsNilResult()
        {
            return FALL == this.Result;
        }

        public override bool HasResult()
        {
            return this.Result.HasValue && !string.IsNullOrEmpty(this.ResText);
        }

        [NotMapped]
        public bool Failed
        {
            get { return this.HasResult() && this.Result.Value >= FALL; }
        }
        
        public ListLineSpeed PreviousRoundResult()
        {
                if (this.Header == null)
                    return null;
                if (this.Header.PreviousRound == null)
                    return null;
                if (this.Header.PreviousRound.Results == null)
                    return null;
                return this.Header.PreviousRound.Results.OfType<ListLineSpeed>().FirstOrDefault(s => s.ClimberId == this.ClimberId);
        }

        [NotMapped]
        public ListLineSpeed BestRecord
        {
            get
            {
                var prevResult = this.PreviousRoundResult();
                if (prevResult == null)
                    return this;
                if ((prevResult.Result ?? long.MaxValue) < (this.Result ?? long.MaxValue))
                    return prevResult;
                return this;
            }
        }

        private long GetResult()
        {
            if (this.Header != null && this.Header.ListType == ClimbingCompetition.Common.ListType.SpeedQualy2 &&
                (this.Header.Rules & ClimbingCompetition.Common.Rules.BestRouteInQf) == ClimbingCompetition.Common.Rules.BestRouteInQf)
                return this.BestRecord.Result ?? 0;
            else
                return this.Result ?? 0;
        }

        public int CompareResult(ListLineSpeed other)
        {
            return this.GetResult().CompareTo(other.GetResult());
        }

        protected override int CompareResult(ListLine other)
        {
            return this.CompareResult((ListLineSpeed)other);
        }
    }
}
