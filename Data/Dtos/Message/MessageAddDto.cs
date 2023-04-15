using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageAddDto
    {
        public int MessageId { get; set; }
        public Guid FkSenderId { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime SendDateTime { get; set; }
        public int FkInResponseMessageId { get; set; }
        public bool Sms { get; set; }
        public bool Email { get; set; }
        public List<MessageAttachmentDto> TMessageAttachment { get; set; }
    }
}