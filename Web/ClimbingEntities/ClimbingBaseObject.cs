using DbAccessCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DbAccessCore.Log;

namespace ClimbingEntities
{
    [Table("all_climbing_objects")]
    public abstract class ClimbingBaseObject : BaseObject
    {
        protected ClimbingBaseObject() { }
        public ClimbingBaseObject(ClimbingContext2 context) : base(context) { }

        protected sealed override void RemoveEntity(BaseContext context)
        {
            RemoveEntity((ClimbingContext2)context);
        }

        protected abstract void RemoveEntity(ClimbingContext2 context);

        protected sealed override void RemoveLinkedCollections(BaseContext context, LogicTransaction ltr)
        {
            RemoveLinkedCollections((ClimbingContext2)context, ltr);
        }

        protected virtual void RemoveLinkedCollections(ClimbingContext2 context, LogicTransaction ltr) { }
    }
}
