using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageGetOneDto
    {
        public int MessageId { get; set; }
        public Guid FkSenderId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public bool ViewFlag { get; set; }
        public string SendDateTime { get; set; }
        public List<MessageAttachmentDto> TMessageAttachment { get; set; }
    }
}