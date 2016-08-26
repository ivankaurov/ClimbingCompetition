using System;
using System.Collections.Generic;
using System.Text;

namespace XmlApiData
{
    public static class XmlApiDataConstants
    {
        public const string NAMESPACE = "http://c-f-r.ru/xml_api";

        public static T[] ToArray<T>(IEnumerable<T> source)
        {
            if (source == null)
                return new T[0];
            else
                return (new List<T>(source)).ToArray();
        }
    }
}
