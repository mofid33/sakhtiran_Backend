using System;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class MessageUserFilterDto
    {
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}