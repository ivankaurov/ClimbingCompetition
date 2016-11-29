using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using DbAccessCore.Attributes;
using System.Collections.Concurrent;

namespace DbAccessCore.Log.Reflection
{
    public static class ReflectionStorage
    {
        static readonly ConcurrentDictionary<Type, ObjectMetaData> propertyDictionary
            = new ConcurrentDictionary<Type, ObjectMetaData>();

        static IEnumerable<PropertyInfo> EnumerateLogProperties(this Type t)
        {
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p =>
                                 {
                                     if (!Attribute.IsDefined(p.DeclaringType, typeof(TableAttribute)) || Attribute.IsDefined(p, typeof(NotMappedAttribute)))
                                         return false;
                                     if (!Attribute.IsDefined(p, typeof(LoggableAttribute)))
                                         return true;
                                     return ((LoggableAttribute)Attribute.GetCustomAttribute(p, typeof(LoggableAttribute))).Loggable;
                                 });
        }

        static readonly Type nullableGeneric = typeof(Nullable<DateTime>).GetGenericTypeDefinition();
        static IEnumerable<OrdinaryPropertyMetaData> EnumerateOrdinaryPropertyMetadata(this Type t, IEnumerable<PropertyInfo> toSkip = null)
        {
            var allProperties = t.EnumerateLogProperties()
                                 .Where(p => Attribute.IsDefined(p, typeof(ColumnAttribute)));
            if (toSkip != null)
                allProperties = allProperties.Where(p => !toSkip.Contains(p));

            foreach (var p in allProperties)
            {
                LogDataType dataType;
                OrdinaryPropertyMetaData result;
                var isNullable = p.PropertyType.IsGenericType && nullableGeneric.Equals(p.PropertyType.GetGenericTypeDefinition());
                var ptc = isNullable ? p.PropertyType.GetGenericArguments()[0] : p.PropertyType;

                if (ptc.IsEnum)
                    result = new EnumPropertyMetaData(ptc);
                else
                {
                    if (ptc.Equals(typeof(DateTime)))
                        dataType = LogDataType.DateTime;
                    else if (ptc.Equals(typeof(String)))
                        dataType = LogDataType.String;
                    else if (ptc.Equals(typeof(byte[])))
                        dataType = LogDataType.Binary;
                    else if (ptc.Equals(typeof(TimeSpan)))
                        dataType = LogDataType.TimeSpan;
                    else
                        dataType = LogDataType.Numeric;
                    result = new OrdinaryPropertyMetaData(dataType);
                }
                result.ColumnName = ((ColumnAttribute)Attribute.GetCustomAttribute(p, typeof(ColumnAttribute))).Name;
                result.Property = p;
                result.TableName = ((TableAttribute)Attribute.GetCustomAttribute(p.DeclaringType, typeof(TableAttribute))).Name;
                result.IsNullable = isNullable;
                yield return result;
            }
        }

        static IEnumerable<ForeignKeyPropertyMetaData> EnumerateForeignKeyPropertyMetadata(this Type t)
        {
            var allProperties = t.EnumerateLogProperties().ToArray();

            foreach (var p in allProperties.Where(p => Attribute.IsDefined(p, typeof(ForeignKeyAttribute))))
            {
                var fkAttribute = (ForeignKeyAttribute)Attribute.GetCustomAttribute(p, typeof(ForeignKeyAttribute));
                PropertyInfo fkProperty, navigationProperty;
                if (p.PropertyType.Equals(BaseObject.IID_TYPE))
                {
                    if (!Attribute.IsDefined(p, typeof(ColumnAttribute)))
                        continue;
                    fkProperty = p;
                    navigationProperty = allProperties.FirstOrDefault(np => np.Name.Equals(fkAttribute.Name, StringComparison.OrdinalIgnoreCase)
                                                                    && np.PropertyType.GetInterfaces().Contains(typeof(IIIDObject)));
                }
                else
                {
                    if (!p.PropertyType.GetInterfaces().Contains(typeof(IIIDObject)))
                        continue;
                    navigationProperty = p;
                    fkProperty = allProperties.FirstOrDefault(fp => fp.Name.Equals(fkAttribute.Name, StringComparison.OrdinalIgnoreCase)
                                                             && fp.PropertyType.Equals(BaseObject.IID_TYPE)
                                                             && Attribute.IsDefined(fp, typeof(ColumnAttribute)));
                }
                if (navigationProperty == null || fkProperty == null)
                    continue;

                yield return new ForeignKeyPropertyMetaData
                {
                    ColumnName = ((ColumnAttribute)Attribute.GetCustomAttribute(fkProperty, typeof(ColumnAttribute))).Name,
                    ForignKeyProperty = fkProperty,
                    NavigationalProperty = navigationProperty,
                    TableName = ((TableAttribute)Attribute.GetCustomAttribute(fkProperty.DeclaringType, typeof(TableAttribute))).Name
                };
            }
        }

        public static ObjectMetaData CreateObjectMetaData(this Type t)
        {
            var result = new ObjectMetaData(t);
            foreach (var p in t.EnumerateForeignKeyPropertyMetadata())
                result.ForeignKeyProperties.AddLast(p);

            foreach (var p in t.EnumerateOrdinaryPropertyMetadata(result.ForeignKeyProperties.Select(p => p.ForignKeyProperty)
                                                                       .Union(result.ForeignKeyProperties.Select(p => p.NavigationalProperty))))
                result.OrdinaryProperties.AddLast(p);

            return result;
        }

        public static ObjectMetaData GetObjectMetaData(this Type t)
        {
            return propertyDictionary.GetOrAdd(t, CreateObjectMetaData);
        }
    }
}
