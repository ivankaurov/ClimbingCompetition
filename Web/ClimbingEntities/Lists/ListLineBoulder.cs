using ClimbingCompetition.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Extensions;
using DbAccessCore.Log;

namespace ClimbingEntities.Lists
{
    [Table("cl_ls_list_lines_boulder")]
    public class ListLineBoulder : ListLine
    {
        protected ListLineBoulder() { }
        public ListLineBoulder(ClimbingContext2 context) : base(context) { this.ResultType = ResultType.RES; }
        protected override void RemoveEntity(ClimbingContext2 context)
        {
            context.ResultsBoulder.Remove(this);
        }

        protected override void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr)
        {
            this.RemoveChildCollection(context, Routes, ltr);
        }

        [NotMapped]
        public ResultType ResultType { get; set; }
        [Column("result_type"), MaxLength(IID_SIZE)]
        public String ResultTypeStr
        {
            get { return ResultType.GetEnumValueString(); }
            set { this.ResultType = value.GetEnumValue<ResultType>() ?? ResultType.RES; }
        }

        public virtual ICollection<ListLineBoulderRoute> Routes { get; set; }

        [NotMapped]
        public int Tops
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Count(r => r.TopAttempt > 0);
            }
        }

        [NotMapped]
        public int TopAttempts
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Sum(r => r.TopAttempt > 0 ? r.TopAttempt : 0);
            }
        }

        [NotMapped]
        public int Bonuses
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Count(r => r.BonusAttempt > 0);
            }
        }

        [NotMapped]
        public int BonusAttempts
        {
            get
            {
                if (this.Routes == null)
                    return 0;
                return this.Routes.Sum(r => r.BonusAttempt > 0 ? r.BonusAttempt : 0);
            }
        }

        [NotMapped]
        public ListLineBoulderRoute this[int route]
        {
            get
            {
                if (this.Routes == null)
                    return null;
                return this.Routes.FirstOrDefault(r => r.RouteNumber == route);
            }
        }

        protected override int CompareResult(ListLine other)
        {
            var br = (ListLineBoulder)other;
            int n = br.Tops.CompareTo(this.Tops);
            if (n != 0)
                return n;

            n = this.TopAttempts.CompareTo(br.TopAttempts);
            if (n != 0)
                return n;

            n = br.Bonuses.CompareTo(this.Bonuses);
            if (n != 0)
                return n;

            return this.BonusAttempts.CompareTo(br.BonusAttempts);
        }

        public override bool HasResult()
        {
            return this.Routes != null && this.Routes.Count > 0;
        }

        protected override bool IsNilResult()
        {
            return this.BonusAttempts == 0;
        }

        protected override bool IsDns()
        {
            return this.ResultType == ResultType.DNS;
        }

        protected override bool IsDsq()
        {
            return this.ResultType == ResultType.DSQ;
        }
    }
}
