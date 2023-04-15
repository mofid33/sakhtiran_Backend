using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMessage
    {
        public TMessage()
        {
            InverseFkInResponseMessage = new HashSet<TMessage>();
            TMessageAttachment = new HashSet<TMessageAttachment>();
            TMessageRecipient = new HashSet<TMessageRecipient>();
        }

        public int MessageId { get; set; }
        public Guid FkSenderId { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime SendDateTime { get; set; }
        public int? FkInResponseMessageId { get; set; }
        public bool Sms { get; set; }
        public bool Email { get; set; }

        public virtual TMessage FkInResponseMessage { get; set; }
        public virtual TUser FkSender { get; set; }
        public virtual ICollection<TMessage> InverseFkInResponseMessage { get; set; }
        public virtual ICollection<TMessageAttachment> TMessageAttachment { get; set; }
        public virtual ICollection<TMessageRecipient> TMessageRecipient { get; set; }
    }
}
