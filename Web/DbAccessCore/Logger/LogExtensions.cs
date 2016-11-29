using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbAccessCore.Log.Reflection;

namespace DbAccessCore.Log
{
    static class LogExtensions
    {
        public static IEnumerable<LogicTransactionObjectParam> EnumerateObjectParams(this Object obj, BaseContext context)
        {
            var objectMetaData = obj.GetType().GetObjectMetaData();
            foreach (var eMD in obj.GetType().GetObjectMetaData())
            {
                var r = new LogicTransactionObjectParam(eMD.DataType)
                {
                    ColumnName = eMD.ColumnName,
                    TableName = eMD.TableName,
                    ParamName = eMD.PropertyName
                };
                if (context != null)
                    r.CreateIid(context);

                var value = eMD.GetValue(obj);

                switch (eMD.DataType)
                {
                    case LogDataType.Binary:
                        r.BinaryValue = value as Byte[];
                        break;
                    case LogDataType.DateTime:
                        r.DateTimeValue = value == null ? null : new DateTime?((DateTime)value);
                        break;
                    case LogDataType.Enum:
                        r.StrValue = value == null ? null : value.ToString();
                        break;
                    case LogDataType.Numeric:
                        r.NumericValue = value == null ? null : new Decimal?(Convert.ToDecimal(value));
                        break;
                    case LogDataType.Reference:
                    case LogDataType.String:
                        r.StrValue = value as String;
                        break;
                    case LogDataType.TimeSpan:
                        r.TimeSpanValue = value == null ? null : new TimeSpan?((TimeSpan)value);
                        break;
                }

                yield return r;
            }
        }

        public static void ApplyObjectParams(this BaseObject obj, IEnumerable<LogicTransactionObjectParam> parameters)
        {
            var paramsWithMetaData = from p in parameters
                                     join md in obj.GetType().GetObjectMetaData() on 
                                        p.ParamName equals md.PropertyName
                                     select new
                                     {
                                         ParamValue = p,
                                         MetaData = md
                                     };
            foreach (var p in paramsWithMetaData)
                switch (p.MetaData.DataType)
                {
                    case LogDataType.Binary:
                        p.MetaData.Setvalue(obj, p.ParamValue.BinaryValue);
                        break;
                    case LogDataType.DateTime:
                        p.MetaData.Setvalue(obj, p.ParamValue.DateTimeValue);
                        break;
                    case LogDataType.Enum:
                        p.MetaData.Setvalue(obj, String.IsNullOrEmpty(p.ParamValue.StrValue) ? null : Enum.Parse(((EnumPropertyMetaData)p.MetaData).EnumType, p.ParamValue.StrValue));
                        break;
                    case LogDataType.Numeric:
                        p.MetaData.Setvalue(obj, p.ParamValue.NumericValue);
                        break;
                    case LogDataType.Reference:
                    case LogDataType.String:
                        p.MetaData.Setvalue(obj, p.ParamValue.StrValue);
                        break;
                    case LogDataType.TimeSpan:
                        p.MetaData.Setvalue(obj, p.ParamValue.TimeSpanValue);
                        break;
                }
        }
    }
}
