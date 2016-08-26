using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore
{
    public interface IIIDObject
    {
        String Iid { get; }
        void RemoveObject(BaseContext context, Log.LogicTransaction ltr = null);
    }

    public interface IBaseObject : IIIDObject
    {
        String ObjectClass { get; }
    }
}
