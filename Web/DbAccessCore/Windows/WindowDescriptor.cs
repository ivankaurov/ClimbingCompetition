using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Windows
{
    [Table("wn_windows")]
    public class WindowDescriptor : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.Windows.Remove(this);
        }

        protected override void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr)
        {
            RemoveChildCollection(context, this.ChildWindows, ltr);
            RemoveChildCollection(context, this.ChildActions, ltr);
        }

        protected WindowDescriptor() { }

        public WindowDescriptor(BaseContext context, String iidToSet, String windowName)
        {
            if (String.IsNullOrEmpty(iidToSet))
                throw new ArgumentNullException("iidToSet");

            if (String.IsNullOrWhiteSpace(windowName))
                throw new ArgumentNullException("windowName");

            this.UserCreated = this.UserUpdated = context.CurrentUserID;
            this.UpdateDate = this.CreateDate = context.Now;

            this.Iid = iidToSet;
            this.Name = windowName;
        }

        private String name;

        [Column("name"), MaxLength(2 * IID_SIZE), Index("wn_windows_UX1", IsUnique = true), Required]
        public String Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    name = String.Empty;
                else
                    name = value.Length > 2 * IID_SIZE ? value.Substring(0, 2 * IID_SIZE) : value;
            }
        }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        public String ParentWindowId { get; set; }
        [ForeignKey("ParentWindowId")]
        public virtual WindowDescriptor ParentWindow { get; set; }

        public virtual ICollection<WindowDescriptor> ChildWindows { get; set; }
        public virtual ICollection<ActionDescriptor> ChildActions { get; set; }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);
            if (result != null)
                return result;
            var parentWindow = this.ParentWindow == null && !String.IsNullOrEmpty(ParentWindowId) ? context.Windows.FirstOrDefault(wnd => wnd.Iid.Equals(this.ParentWindowId, StringComparison.OrdinalIgnoreCase)) : this.ParentWindow;
            if (parentWindow != null)
            {
                result = parentWindow.GetRights(securityEntityID, action, context);
                isInherited = true;
            }
            return result;
        }
    }
}