using ClimbingCompetition.Common;
using ClimbingEntities.Competitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    public interface IListLine
    {
        ClimberOnCompetition Climber { get; }

        bool EqualResults(IListLine other);

        bool NilResult();

        bool HasResult();

        int Pos { get; set; }

        string PosText { get; set; }

        double? Points { get; set; }

        string PtsText { get; set; }

        NextRoundQf Qf { get; set; }

        bool PreQf { get; }

        bool Dns { get; }
        bool Dsq { get; }

        int CompareTo(IListLine other, bool hasPrevRound);
    }
}
