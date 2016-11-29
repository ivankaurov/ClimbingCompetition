using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore.Log.Reflection
{
    public sealed class ObjectMetaData : IEnumerable<PropertyMetaData>
    {
        public ObjectMetaData(Type declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
            this.declaringType = declaringType;
        }

        readonly LinkedList<OrdinaryPropertyMetaData> ordinaryProperties = new LinkedList<OrdinaryPropertyMetaData>();
        readonly LinkedList<ForeignKeyPropertyMetaData> foreignKeyProperties = new LinkedList<ForeignKeyPropertyMetaData>();

        public LinkedList<OrdinaryPropertyMetaData> OrdinaryProperties { get { return ordinaryProperties; } }
        public LinkedList<ForeignKeyPropertyMetaData> ForeignKeyProperties { get { return foreignKeyProperties; } }

        readonly Type declaringType;
        public Type DeclaringType { get { return declaringType; } }
        public String DeclaringTypeName { get { return DeclaringType.FullName; } }

        public IEnumerator<PropertyMetaData> GetEnumerator()
        {
            return ((IEnumerable<PropertyMetaData>)ordinaryProperties)
                      .Union(foreignKeyProperties)
                      .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
