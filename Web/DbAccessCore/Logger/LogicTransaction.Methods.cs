using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessCore.Log
{
    partial class LogicTransaction
    {
        LogicTransactionObject AddLogObject(IBaseObject obj, LogAction action, BaseContext context, int? order)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (String.IsNullOrEmpty(obj.Iid))
                throw new ArgumentNullException("obj.Iid");

            if (this.Objects == null)
                this.Objects = new List<LogicTransactionObject>();
            
            var result = this.Objects.FirstOrDefault(o => o.ObjectIid.Equals(obj.Iid, StringComparison.OrdinalIgnoreCase) && o.ActionChar.Equals(action.GetFirstLetter()));
            if (result == null)
                this.Objects.Add(result = new LogicTransactionObject(action, obj, context)
                {
                    Params = new List<LogicTransactionObjectParam>()
                });
            result.AddOrder = order ?? this.Objects.Count;
            return result;
        }

        IEnumerable<LogicTransactionObjectParam> EnumerateParams(IBaseObject obj, LogValueType recordType, BaseContext context)
        {
            var rtc = recordType.GetFirstLetter();
            return obj.EnumerateObjectParams(context)
                      .Select(p =>
                      {
                          p.RecordType = recordType;
                          return p;
                      });
        }


        public LogicTransactionObject AddCreatedObject(IBaseObject obj, BaseContext context, int? order = null)
        {
            var result = AddLogObject(obj, LogAction.Add, context, order);
            if (obj is BaseObject)
                ((BaseObject)obj).LtrIid = this.Iid;
            result.AddParams(obj, LogValueType.New, context);
            return result;
        }

        public LogicTransactionObject AddDeletedObject(IBaseObject obj, BaseContext context, int? order = null)
        {
            var result = AddLogObject(obj, LogAction.Delete, context, order);
            result.AddParams(obj, LogValueType.Old, context);
            return result;
        }

        public LogicTransactionObject AddUpdatedObjectBefore(IBaseObject obj, BaseContext context, int? order = null)
        {
            var result = AddLogObject(obj, LogAction.Update, context, order);
            result.AddParams(obj, LogValueType.Old, null);
            return result;
        }

        public LogicTransactionObject AddUpatedObjectAfter(IBaseObject obj, BaseContext context)
        {
            var result = AddLogObject(obj, LogAction.Update, context, null);
            if (obj is BaseObject)
                ((BaseObject)obj).LtrIid = this.Iid;
            result.AddNewParams(obj, context);
            return result;
        }

        public void Commit(BaseContext context)
        {
            if (this.State == LtrState.Rollback)
                throw new InvalidOperationException("Ltr is rollbacked");

            if (this.Children != null)
                foreach (var c in this.Children)
                    c.Commit(context);

            if (this.State != LtrState.Commit)
            {
                this.CommitDate = context.Now;
                this.State = LtrState.Commit;
            }
        }

        static readonly String rollbackChar = LtrState.Rollback.GetFirstLetter();
        public Boolean CanRollBack(BaseContext context)
        {
            if (this.State != LtrState.Rollback && this.Objects != null)
                foreach (var obj in this.Objects)
                    if (context.LogicTransactionObjects1
                              .FirstOrDefault(l => l.ObjectIid.Equals(obj.ObjectIid, StringComparison.Ordinal)
                                              && l.Ltr.StateChar != rollbackChar
                                              && l.Ltr.BeginDate > this.BeginDate) != null)
                        return false;
            if (this.Children != null)
                foreach (var c in Children)
                    if (!c.CanRollBack(context))
                        return false;
            return true;
        }

        void RollbackWithoutCheck(BaseContext context)
        {
            if (this.Children != null)
                foreach (var c in this.Children)
                    c.RollbackWithoutCheck(context);

            if (this.State == LtrState.Rollback)
                return;
            if (this.Objects != null)
            {
                object syncRoot = new object();
                foreach (var order in this.Objects
                                         .Select(o => o.AddOrder)
                                         .Distinct()
                                         .OrderByDescending(n => n))
                {
                    var objectsByOrder = Objects.Where(o => o.AddOrder == order).ToArray();
                    switch (objectsByOrder.Length)
                    {
                        case 0: break;
                        case 1: objectsByOrder[0].RollbackObjectAction(context, syncRoot);
                            break;
                        default:
                            Task.WaitAll(objectsByOrder.Select(l => Task.Factory.StartNew(() => l.RollbackObjectAction(context, syncRoot))).ToArray());
                            break;
                    }   
                }
            }
            this.State = LtrState.Rollback;
            this.RollbackDate = context.Now;
            this.LtrUserRollback = context.CurrentUserID;
        }

        public void Rollback(BaseContext context)
        {
            if (!this.CanRollBack(context))
                throw new InvalidOperationException("Can\'t rollback LTR");
            RollbackWithoutCheck(context);
        }

    }
}