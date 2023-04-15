using System;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageSentListDto
    {
        public int MessageId { get; set; }
        public string SendDateTime { get; set; }
        public string Subject { get; set; }
        public bool HasAttachment { get; set; }
        public int RecipientCount { get; set; }
    }
}