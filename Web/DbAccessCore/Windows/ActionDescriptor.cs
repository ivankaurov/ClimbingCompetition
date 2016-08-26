using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Windows
{
    [Table("wn_actions")]
    public class ActionDescriptor : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.Actions.Remove(this);
        }

        protected ActionDescriptor() { }

        public ActionDescriptor(BaseContext context, String actionKey, WindowDescriptor parent)
            : base(context)
        {
            if (String.IsNullOrEmpty(actionKey))
                throw new ArgumentNullException(actionKey);
            if (parent == null)
                throw new ArgumentNullException("parent");
            this.ParentWindow = parent;
            parent.ChildActions.Add(this);
        }

        [Column("iid_parent"), MaxLength(IID_SIZE),
        Index("wn_actions_UX1", IsUnique = true, Order = 2),
        Index("wn_actions_UX2", IsUnique = true, Order = 2)]
        public String ParentWindowId { get; protected set; }
        [ForeignKey("ParentWindowId")]
        public virtual WindowDescriptor ParentWindow { get; protected set; }

        private String actionKey;
        [Column("action_key"), MaxLength(IID_SIZE), Index("wn_actions_UX1", IsUnique = true, Order = 1)]
        public String ActionKey
        {
            get { return actionKey; }
            protected set
            {
                if (value == null)
                    actionKey = value;
                else
                    actionKey = value.Length > IID_SIZE ? value.Substring(0, IID_SIZE) : value;
            }
        }

        private String actionName;
        [Column("action_name"), MaxLength(2 * IID_SIZE), Index("wn_actions_UX2", IsUnique = true, Order = 1), Required]
        public String ActionName
        {
            get { return actionName; }
            set
            {
                if (value == null)
                    actionName = null;
                else
                    actionName = value.Length > 2 * IID_SIZE ? value.Substring(0, 2 * IID_SIZE) : value;
            }
        }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);
            if (result != null)
                return result;
            var window = this.ParentWindow == null && !String.IsNullOrEmpty(this.ParentWindowId) ? context.Windows.FirstOrDefault(wnd => wnd.Iid.Equals(ParentWindowId, StringComparison.OrdinalIgnoreCase)) : ParentWindow;
            if (window != null)
            {
                result = window.GetRights(securityEntityID, action, context);
                isInherited = true;
            }
            return result;
        }
    }
}