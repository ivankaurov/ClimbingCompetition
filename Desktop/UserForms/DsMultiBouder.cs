namespace ClimbingCompetition
{


    public partial class DsMultiBouder
    {

        partial class boulderResultsDataTable
        {
            public boulderResultsRow GetRowByIid(long iid)
            {
                for (int i = 0; i < Rows.Count; i++)
                    if (this[i].iid == iid)
                        return this[i];
                return null;
            }
        }

        partial class boulderResultsRow
        {
            public BoulderRoutesRow GetByRouteNumber(int routeNumber)
            {
                BoulderRoutesRow[] rows = this.GetBoulderRoutesRows();
                if (rows == null)
                    return null;
                for (int i = 0; i < rows.Length; i++)
                    if (rows[i].routeN == routeNumber)
                        return rows[i];
                return null;
            }
        }
    }
}
