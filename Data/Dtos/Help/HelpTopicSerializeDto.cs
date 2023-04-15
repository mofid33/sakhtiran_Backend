using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Help
{
    public class HelpTopicSerializeDto
    {
        public string HelpTopic { get; set; }
        public IFormFile Icon { get; set; }
    }
}