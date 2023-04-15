using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TUserGroup
    {
        public TUserGroup()
        {
            TUser = new HashSet<TUser>();
        }

        public string UserGroupTitle { get; set; }
        public Guid UserGroupId { get; set; }

        public virtual ICollection<TUser> TUser { get; set; }
    }
}
