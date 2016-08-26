using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
using System.Reflection;

namespace DbAccessCore
{
    partial class BaseObject
    {
        public const int IID_SIZE = 25;
        public const int TYPE_SIZE = 512;
        public static readonly Type IID_TYPE = typeof(String);

        readonly Boolean isProxy;

        [NotMapped]
        public Boolean IsProxy { get { return isProxy; } }

        [NotMapped]
        public virtual Boolean NeedLtrForDelete { get { return false; } }

        protected BaseObject()
        {
            this.ObjectClass = this.GetType().FullName;
            this.isProxy = true;
        }

        public BaseObject(BaseContext context)
            : this()
        {
            this.ObjectClass = this.GetType().FullName;
            this.isProxy = false;

            String sIid;
            DateTime dtNow;
            context.GetObjectCreationValues(out sIid, out dtNow);
            this.Iid = sIid;
            this.CreateDate = this.UpdateDate = dtNow;
            this.UserCreated = this.UserUpdated = context.CurrentUserID;
        }

        readonly static ConcurrentDictionary<Type, Tuple<ConstructorInfo, Object>> constructors
            = new ConcurrentDictionary<Type, Tuple<ConstructorInfo, object>>();
        internal static BaseObject CreateEmpty(Type t, String iid)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (String.IsNullOrEmpty(iid))
                throw new ArgumentNullException("iid");
            if (t.IsSubclassOf(typeof(BaseObject)) || t.Equals(typeof(BaseObject)))
            {
                var ci = constructors.GetOrAdd(t, tp =>
                    new Tuple<ConstructorInfo, Object>(
                        t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                         null,
                                         Type.EmptyTypes,
                                         null), new object()));
                if (ci.Item1 == null)
                    return null;
                BaseObject result;
                lock (ci.Item2) { result = (BaseObject)ci.Item1.Invoke(null); }

                result.Iid = iid;
                return result;
            }
            else
                return null;
        }

        internal static BaseObject CreateEmpty(String typeName, String iid)
        {
            var type = Extensions.ObjectExtensions.GetTypeByName(typeName);
            return type == null ? null : CreateEmpty(type, iid);
        }

        protected abstract void RemoveEntity(BaseContext context);
        protected virtual void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr) { }

        public void RemoveObject(BaseContext context, Log.LogicTransaction ltr = null)
        {
            if (context == null)
                return;

            var _ltr = ltr == null ? (this.NeedLtrForDelete ? context.BeginLtr() : null) : ltr;

            RemoveLinkedCollections(context, _ltr);

            RemoveChildCollection(context, this.RightsForThisObject, _ltr);

            if (_ltr != null)
                _ltr.AddDeletedObject(this, context);

            if (_ltr != null && ltr == null)
                _ltr.Commit(context);

            RemoveEntity(context);
        }

        protected void RemoveChildCollection(BaseContext context, IEnumerable<IIIDObject> childCollection, Log.LogicTransaction ltr)
        {
            if (context == null || childCollection == null)
                return;
            childCollection.ToList().ForEach(obj => obj.RemoveObject(context));
        }
    }
}
