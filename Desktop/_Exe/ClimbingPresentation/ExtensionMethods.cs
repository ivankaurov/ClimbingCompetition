using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClimbingPresentation
{
    public static class ExtensionMethods
    {
        public static string GetStyleRoundString(this ListShow.ListShowControl.ListForShow lst)
        {
            if (lst == null)
                return String.Empty;
            return lst.Style.ToUpper() + "_" + lst.Round.ToUpper() + (lst.RouteNumber > 0 ? "_" + lst.RouteNumber.ToString() : String.Empty);
        }

        public static string GetListHeader(this ListShow.ListShowControl.ListForShow lst)
        {
            if (lst == null)
                return String.Empty;
            return String.Format("{0} {1}", lst.Group ?? String.Empty, lst.Round ?? String.Empty);
        }
    }
}
