using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public class DataEventArgs<T> : EventArgs
    {
        readonly T data;
        public T Data { get { return data; } }

        public DataEventArgs(T data) { this.data = data; }
    }

    public sealed class DataEventArgs<T1, T2> : EventArgs
    {
        readonly T1 data1;
        readonly T2 data2;
        public T1 Data1 { get { return data1; } }
        public T2 Data2 { get { return data2; } }

        public DataEventArgs(T1 data1, T2 data2)
        {
            this.data1 = data1;
            this.data2 = data2;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", data1, data2);
        }
    }
}
