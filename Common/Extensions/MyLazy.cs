using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public sealed class MyLazy<T> : IDisposable
    {
        static readonly bool isDisposable;
        static MyLazy()
        {
            isDisposable = typeof(T).GetInterfaces().Contains(typeof(IDisposable));
        }

        readonly object syncRoot = new object();
        readonly Func<T> valueFactory;

        bool valueCreated = false;
        bool objectDisposed = false;
        T value;

        public bool ValueCreated { get { return this.valueCreated; } }

        public MyLazy(Func<T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            this.valueFactory = valueFactory;
        }

        public void Dispose()
        {
            if (!objectDisposed)
            {
                if (isDisposable)
                    lock (syncRoot)
                    {
                        if (this.valueCreated)
                            ((IDisposable)value).Dispose();
                    }
                this.objectDisposed = true;
            }
        }

        public T Value
        {
            get
            {
                return this.GetOrCreateValue();
            }
        }

        public T GetOrCreateValue()
        {
            if (this.objectDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            lock (syncRoot)
            {
                if (!valueCreated)
                {
                    this.value = valueFactory();
                    this.valueCreated = true;
                }
                return this.value;
            }
        }

        public void ResetValue()
        {
            if (objectDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
            lock (syncRoot)
            {
                if(this.valueCreated)
                {
                    if (isDisposable)
                        ((IDisposable)value).Dispose();
                    this.value = default(T);
                    this.valueCreated = false;
                }
            }
        }
    }
}
