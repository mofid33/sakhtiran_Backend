using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsSerializeDto
    {
        public string Goods { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile File { get; set; }
    }
}