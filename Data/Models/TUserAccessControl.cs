using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TUserAccessControl
    {
        public int UserAccessControlId { get; set; }
        public Guid FkUserId { get; set; }
        public int FkMenuItemId { get; set; }

        public virtual TMenuItem FkMenuItem { get; set; }
        public virtual TUser FkUser { get; set; }
    }
}
