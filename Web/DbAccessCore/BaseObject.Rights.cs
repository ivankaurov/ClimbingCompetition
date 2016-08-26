using DbAccessCore.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore
{
    partial class BaseObject
    {
        private RightsEnum? GetRightsFromCollection(RightsActionEnum action, IEnumerable<DbRights> rights)
        {
            if (rights == null)
                return null;
            RightsEnum? result = null;
            foreach (var right in rights.Where(a => a.Action >= action).OrderByDescending(a => a.Action))
            {
                if (!result.HasValue || result.Value < right.Rights)
                    result = right.Rights;
            }

            return result;
        }

        public virtual RightsEnum? GetRights(String securityEntityID, RightsActionEnum action, BaseContext context, out Boolean isInherited)
        {
            if (context.AdminAllowAll && context.UserIsAdmin(securityEntityID))
            {
                isInherited = false;
                return RightsEnum.Allow;
            }
            var directRights = this.GetRightsFromCollection(action, context.AllRights
                                                                           .Where(r => r.SubjectId == this.Iid && r.ObjectId == securityEntityID)
                                                                           .ToList());

            if (directRights.HasValue)
            {
                isInherited = false;
                return directRights;
            }
            isInherited = true;

            var groupRightsForThisUser = (from gm in context.GroupMembers
                                          join gr in context.UserGroups on gm.GroupId equals gr.Iid
                                          join or in context.AllRights on gr.Iid equals or.ObjectId
                                          where or.SubjectId == this.Iid
                                          select or).ToList();
            switch (groupRightsForThisUser.Count)
            {
                case 0: return null;
                case 1: return groupRightsForThisUser[0].Rights;
            }

            var byGroup = groupRightsForThisUser.ToLookup(g => g.ObjectId);
            foreach(var e in byGroup)
            {
                var rights = this.GetRightsFromCollection(action, e);
                if (!directRights.HasValue || directRights.Value > rights)
                    directRights = rights;
            }

            return directRights;
        }

        public RightsEnum? GetRights(String securityEntityID, RightsActionEnum action, BaseContext context)
        {
            Boolean isInherited;
            return GetRights(securityEntityID, action, context, out isInherited);
        }

        public RightsEnum GetNotNullableRigths(string securityEntityId, RightsActionEnum action, BaseContext context)
        {
            return this.GetRights(securityEntityId, action, context) ?? RightsEnum.Deny;
        }

        public void SetRights(String securityEntityID, RightsActionEnum action,RightsEnum? rightsValue, BaseContext context, Log.LogicTransaction ltr = null)
        {
            var ace = context.AllRights.Where(r => r.SubjectId.Equals(this.Iid, StringComparison.OrdinalIgnoreCase) && r.ObjectId.Equals(securityEntityID, StringComparison.OrdinalIgnoreCase))
                                       .ToList()
                                       .FirstOrDefault(r => r.Action == action);
            if (rightsValue == null)
            {
                if (ace != null)
                    ace.RemoveObject(context, ltr);
                return;
            }
            if (ace == null)
            {
                ace = context.AllRights.Add(new Users.DbRights(context)
                {
                    Subject = this,
                    SubjectId = this.Iid,
                    ObjectId = securityEntityID,
                    Rights = rightsValue.Value,
                    Action = action
                });
                if (ltr != null)
                    ltr.AddCreatedObject(ace, context);
                return;
            }

            if (ace.Rights != rightsValue.Value)
            {
                if (ltr != null)
                    ltr.AddUpdatedObjectBefore(ace, context);
                ace.Rights = rightsValue.Value;
                if (ltr != null)
                    ltr.AddUpatedObjectAfter(ace, context);
            }
        }
    }
}
