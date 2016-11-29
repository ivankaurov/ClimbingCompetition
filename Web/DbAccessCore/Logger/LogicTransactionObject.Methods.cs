using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore.Log
{
    partial class LogicTransactionObject
    {
        internal void FixParamsIid(BaseContext context)
        {
            if (Params == null)
            {
                Params = new List<LogicTransactionObjectParam>();
                return;
            }
            foreach (var p in this.Params.Where(p => p.Iid == null || p.Iid.Equals(String.Empty, StringComparison.Ordinal)).ToArray())
                p.CreateIid(context);
        }

        internal LogicTransactionObjectParam AddParam(LogicTransactionObjectParam value)
        {
            var existingParam = Params.FirstOrDefault(p => p.ParamName.Equals(value.ParamName, StringComparison.Ordinal)
                                                        && p.RecordTypeChar.Equals(value.RecordTypeChar));
            if (existingParam != null)
                Params.Remove(existingParam);
            this.Params.Add(value);
            return value;
        }

        internal LogicTransactionObjectParam[] AddParams(IBaseObject obj, LogValueType recordType, BaseContext context)
        {
            return obj.EnumerateObjectParams(context)
                      .Select(p =>
                      {
                          p.RecordType = recordType;
                          return AddParam(p);
                      })
                      .ToArray();
        }

        readonly static String oldParamChar = LogValueType.Old.GetFirstLetter(),
            newParamChar = LogValueType.New.GetFirstLetter();
        internal void AddNewParams(IBaseObject obj, BaseContext context)
        {
            foreach (var newParam in obj.EnumerateObjectParams(null))
            {
                var oldParam = Params.FirstOrDefault(p => p.RecordTypeChar.Equals(oldParamChar) && p.ParamName.Equals(newParam.ParamName, StringComparison.Ordinal));
                if (oldParam != null && oldParam.ValueEquals(newParam))
                    this.Params.Remove(oldParam);
                else
                {
                    newParam.RecordType = LogValueType.New;
                    this.Params.Add(newParam);
                }
            }
            FixParamsIid(context);
        }

        internal void RollbackObjectAction(BaseContext context, Object contextSyncRoot)
        {
            switch (this.Action)
            {
                case LogAction.Add:
                    ProcessAdd(context, contextSyncRoot);
                    break;
                case LogAction.Delete:
                    ProcessRemove(context, contextSyncRoot);
                    break;
                case LogAction.Update:
                    ProcessUpdate(context, contextSyncRoot);
                    break;
            }
        }

        void ProcessAdd(BaseContext context, Object contextSyncRoot)
        {
            lock (contextSyncRoot)
            {
                var findObject = context.AllObjects.FirstOrDefault(obj => obj.Iid.Equals(this.ObjectIid, StringComparison.Ordinal));
                if (findObject != null)
                    context.AllObjects.Remove(findObject);
            }
        }

        void ProcessUpdate(BaseContext context, Object contextSyncRoot)
        {
            BaseObject findObject;
            lock (contextSyncRoot) { findObject = context.AllObjects.FirstOrDefault(obj => obj.Iid.Equals(this.ObjectIid, StringComparison.Ordinal)); }

            if (findObject == null)
                return;

            findObject.ApplyObjectParams(this.Params.Where(p => p.RecordTypeChar.Equals(oldParamChar)));
        }

        void ProcessRemove(BaseContext context, Object contextSyncRoot)
        {
            BaseObject findObject;
            lock (contextSyncRoot) { findObject = context.AllObjects.FirstOrDefault(obj => obj.Iid.Equals(this.ObjectIid, StringComparison.Ordinal)); }

            if (findObject == null)
            {
                findObject = BaseObject.CreateEmpty(this.ObjectClass, this.ObjectIid);
                if (findObject == null)
                    return;
                lock (contextSyncRoot) { context.AllObjects.Add(findObject); }
            }

            findObject.ApplyObjectParams(this.Params);
        }
    }
}