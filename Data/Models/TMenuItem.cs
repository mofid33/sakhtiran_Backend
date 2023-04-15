using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMenuItem
    {
        public TMenuItem()
        {
            InverseParent = new HashSet<TMenuItem>();
            TUserAccessControl = new HashSet<TUserAccessControl>();
        }

        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Expanded { get; set; }
        public int? ParentId { get; set; }

        public virtual TMenuItem Parent { get; set; }
        public virtual ICollection<TMenuItem> InverseParent { get; set; }
        public virtual ICollection<TUserAccessControl> TUserAccessControl { get; set; }
    }
}
