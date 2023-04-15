using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TUser
    {
        public TUser()
        {
            TMessage = new HashSet<TMessage>();
            TMessageRecipient = new HashSet<TMessageRecipient>();
            TOrderLog = new HashSet<TOrderLog>();
            TOrderReturningLog = new HashSet<TOrderReturningLog>();
            TUserAccessControl = new HashSet<TUserAccessControl>();
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public Guid UserId { get; set; }
        public Guid FkUserGroupId { get; set; }
        public int? FkCustumerId { get; set; }
        public int? FkShopId { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool Active { get; set; }
        public DateTime? LastLoginDatetime { get; set; }
        public string ProviderFirebasePushNotificationKey { get; set; }
        public string ClientMobileFirebasePushNotificationKey { get; set; }
        public string ClientWebFirebasePushNotificationKey { get; set; }

        public virtual TCustomer FkCustumer { get; set; }
        public virtual TShop FkShop { get; set; }
        public virtual TUserGroup FkUserGroup { get; set; }
        public virtual ICollection<TMessage> TMessage { get; set; }
        public virtual ICollection<TMessageRecipient> TMessageRecipient { get; set; }
        public virtual ICollection<TOrderLog> TOrderLog { get; set; }
        public virtual ICollection<TOrderReturningLog> TOrderReturningLog { get; set; }
        public virtual ICollection<TUserAccessControl> TUserAccessControl { get; set; }
        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
