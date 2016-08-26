using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Extensions
{
    public class MyConcurrentDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public TValue this[TKey key]
        {
            get
            {
                locker.EnterReadLock();
                try { return this[key]; }
                finally { locker.ExitReadLock(); }
            }
            set
            {
                locker.EnterUpgradeableReadLock();
                try
                {
                    dict[key] = value;
                }
                finally { locker.ExitUpgradeableReadLock(); }
            }
        }

        public TValue Remove(TKey key)
        {
            locker.EnterWriteLock();
            try
            {
                var result = default(TValue);
                if (this.dict.ContainsKey(key))
                {
                    result = dict[key];
                    dict.Remove(key);
                }
                return result;
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            locker.EnterReadLock();
            try
            {
                if (dict.ContainsKey(key))
                    return dict[key];
            }
            finally { locker.ExitReadLock(); }

            locker.EnterWriteLock();
            try
            {
                if (dict.ContainsKey(key))
                    return dict[key];
                TValue result;
                dict.Add(key, result = valueFactory(key));
                return result;
            }
            finally { locker.ExitWriteLock(); }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return dict.Values;
                }
                finally { locker.ExitReadLock(); }
            }
        }
    }
}
