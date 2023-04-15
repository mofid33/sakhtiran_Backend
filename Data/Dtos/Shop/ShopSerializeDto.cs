using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopSerializeDto
    {
        public string Shop { get; set; }
        public List<IFormFile> Files { get; set; }
        public string DeleteFilesId { get; set; }
    }
}