using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public class MyThreadLocal<T> : IDisposable
    {
        readonly MyConcurrentDictionary<int, T> threadLocalValues = new MyConcurrentDictionary<int, T>(); 
        static readonly Boolean isDisposable;
        static MyThreadLocal()
        {
            isDisposable = typeof(T).GetInterfaces().Contains(typeof(IDisposable));
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (isDisposable && disposing)
                foreach (var dV in threadLocalValues.Values.Where(v => v != null))
                    ((IDisposable)dV).Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MyThreadLocal() { this.Dispose(false); }

        readonly Func<T> valueFactory;
        public MyThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            this.valueFactory = valueFactory;
        }

        public T Value
        {
            get
            {
                return threadLocalValues.GetOrAdd(System.Threading.Thread.CurrentThread.ManagedThreadId, key => valueFactory());
            }
        }
    }
}
