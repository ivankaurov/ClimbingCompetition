using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DbAccessCore.Log.Reflection
{
    public abstract class PropertyMetaData
    {
        public abstract String PropertyName { get; }
        public String TableName { get; set; }
        public String ColumnName { get; set; }
        public abstract LogDataType DataType { get; }
        public abstract Object GetValue(Object obj);

        public void Setvalue(Object obj, Object value)
        {
            SetObjValue(obj, value == DBNull.Value ? null : value);
        }

        protected abstract void SetObjValue(Object obj, Object value);
    }

    public class OrdinaryPropertyMetaData : PropertyMetaData
    {
        public Boolean IsNullable { get; set; }
        public PropertyInfo Property { get; set; }

        readonly LogDataType dataType;

        public OrdinaryPropertyMetaData(LogDataType dataType) { this.dataType = dataType; }

        public override string PropertyName { get { return Property.Name; } }

        public override LogDataType DataType { get { return dataType; } }

        public override object GetValue(object obj)
        {
            var result = Property.GetValue(obj, null);
            if(result == DBNull.Value || result == null)
                return null;
            return result;
        }

        protected override void SetObjValue(object obj, object value)
        {
            Property.SetValue(obj, value, null);
        }
    }

    public sealed class EnumPropertyMetaData : OrdinaryPropertyMetaData
    {
        readonly Type enumType;
        public Type EnumType { get { return enumType; } }

        public EnumPropertyMetaData(Type enumType) : base(LogDataType.Enum) { this.enumType = enumType; }

        public override object GetValue(object obj)
        {
            var result = Property.GetValue(obj, null);
            if (result == null || result == DBNull.Value)
                return null;
            return result.ToString();
        }

        protected override void SetObjValue(object obj, object value)
        {
            if (value != null)
                value = Enum.Parse(enumType, value.ToString(), true);
            Property.SetValue(obj, value, null);
        }
    }

    public sealed class ForeignKeyPropertyMetaData : PropertyMetaData
    {
        public PropertyInfo ForignKeyProperty { get; set; }
        public PropertyInfo NavigationalProperty { get; set; }

        public override LogDataType DataType { get { return LogDataType.Reference; } }

        public override object GetValue(object obj)
        {
            var navigationResult = NavigationalProperty.GetValue(obj, null) as IIIDObject;
            if (navigationResult != null && !String.IsNullOrEmpty(navigationResult.Iid))
                return navigationResult.Iid;
            return ForignKeyProperty.GetValue(obj, null) as String;
        }

        protected override void SetObjValue(object obj, object value)
        {
            ForignKeyProperty.SetValue(obj, value, null);
        }

        public override string PropertyName { get { return ForignKeyProperty.Name; } }
    }
}