using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Message
{
    public class SerializMessageDto
    {
        public string Message { get; set; }
        public List<IFormFile> Attachment { get; set; }
    }
}