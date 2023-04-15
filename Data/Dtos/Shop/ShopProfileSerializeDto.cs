using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopProfileSerializeDto
    {
        public int ShopId { get; set; }
        public bool IsProfileNull { get; set; }
        public bool IsLogoNull { get; set; }
        public IFormFile Profile { get; set; }
        public IFormFile Logo { get; set; }
    }
}