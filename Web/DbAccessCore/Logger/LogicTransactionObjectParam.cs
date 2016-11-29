using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Extensions;

namespace DbAccessCore.Log
{
    [Table("log_logic_transaction_object_params")]
    public class LogicTransactionObjectParam : IIIDObject
    {
        const string NUMERIC_CULTURE_INFO = "en-US";
        static readonly Lazy<CultureInfo> defaultCulture = new Lazy<CultureInfo>(() => CultureInfo.GetCultureInfo(NUMERIC_CULTURE_INFO));

        const string PARAM_INDEX_NAME = "logic_params_UX1";

        protected LogicTransactionObjectParam() { }

        internal LogicTransactionObjectParam(LogDataType dataType)
        {
            this.DataType = dataType;
        }

        internal String CreateIid(BaseContext context)
        {
            return this.Iid = context.CreateNewIid();
        }

        [Key, Column("Iid"), MaxLength(BaseObject.IID_SIZE), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Iid { get; protected set; }

        [Column("ltr_obj_id"), MaxLength(BaseObject.IID_SIZE), Index(PARAM_INDEX_NAME, Order = 3, IsUnique = true)]
        public String LtrObjIid { get; set; }

        [ForeignKey("LtrObjIid")]
        public virtual LogicTransactionObject LtrObj { get; set; }

        [Column("record_type"), Index(PARAM_INDEX_NAME, Order = 2, IsUnique = true), MaxLength(1)]
        public String RecordTypeChar { get; protected set; }

        [NotMapped]
        public LogValueType RecordType
        {
            get { return RecordTypeChar.GetByFirstLetter<LogValueType>(); }
            set { RecordTypeChar = value.GetFirstLetter(); }
        }

        [Column("param_name"), MaxLength(BaseObject.IID_SIZE * 2), Required, Index(PARAM_INDEX_NAME, Order = 1, IsUnique = true)]
        public String ParamName { get; set; }

        [Column("table_name"), MaxLength(BaseObject.IID_SIZE * 2), Required]
        public String TableName { get; set; }

        [Column("column_name"), MaxLength(BaseObject.IID_SIZE * 2), Required]
        public String ColumnName { get; set; }

        [Column("data_type"), MaxLength(BaseObject.IID_SIZE)]
        public String DataTypeStr { get; protected set; }

        [NotMapped]
        public LogDataType DataType
        {
            get { return (LogDataType)Enum.Parse(typeof(LogDataType), DataTypeStr); }
            protected set { DataTypeStr = value.ToString("G"); }
        }

        [Column("str_value")]
        public String StrValue { get; set; }

        [Column("datetime_value")]
        public DateTime? DateTimeValue { get; set; }

        [NotMapped]
        public Decimal? NumericValue
        {
            get { return String.IsNullOrEmpty(StrValue) ? null : new Decimal?(Decimal.Parse(StrValue, defaultCulture.Value)); }
            set { StrValue = value.HasValue ? value.Value.ToString(defaultCulture.Value) : null; }
        }

        [Column("binary_value")]
        public Byte[] BinaryValue { get; set; }

        [Column("timespan_value")]
        public TimeSpan? TimeSpanValue { get; set; }

        public Boolean ValueEquals(LogicTransactionObjectParam other)
        {
            if (Object.ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            if (this.DataType != other.DataType)
                return false;
            switch (this.DataType)
            {
                case LogDataType.Binary:
                    return this.BinaryValue.ArrayEquals(other.BinaryValue);
                case LogDataType.DateTime:
                    return this.DateTimeValue == other.DateTimeValue;
                case LogDataType.Enum:
                case LogDataType.Numeric:
                case LogDataType.Reference:
                case LogDataType.String:
                    return String.Equals(this.StrValue, other.StrValue, StringComparison.Ordinal);
                case LogDataType.TimeSpan:
                    return this.TimeSpanValue == other.TimeSpanValue;
            }
            return false;
        }

        public static bool ValueEquals(LogicTransactionObjectParam a, LogicTransactionObjectParam b)
        {
            if (a == null)
                return (b == null);
            return a.ValueEquals(b);
        }


        public void RemoveObject(BaseContext context, LogicTransaction ltr = null)
        {
            if (context != null)
                context.LogicTransactionObjectParams1.Remove(this);
        }
    }
}