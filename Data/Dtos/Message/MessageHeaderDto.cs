using System;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageHeaderDto
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string SendDateTime { get; set; }
    }
}