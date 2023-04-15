using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopRegisterSerializeDto
    {
        public string Shop { get; set; }
        public List<IFormFile> FilesDto { get; set; }
    }
}