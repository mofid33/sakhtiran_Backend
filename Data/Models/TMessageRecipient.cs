using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMessageRecipient
    {
        public int RecipientId { get; set; }
        public int FkMessageId { get; set; }
        public Guid FkRecieverId { get; set; }
        public DateTime? ViewDateTime { get; set; }
        public bool ViewFlag { get; set; }

        public virtual TMessage FkMessage { get; set; }
        public virtual TUser FkReciever { get; set; }
    }
}
