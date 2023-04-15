using System;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageInboxListDto
    {
        public int MessageId { get; set; }
        public string SendDateTime { get; set; }
        public Guid FkSenderId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool ViewFlag { get; set; }
        public bool HasAttachment { get; set; }
    }
}