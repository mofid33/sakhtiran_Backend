using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsProviderSerializeDto
    {
        public string GoodsProvider { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ParameterValuesIds { get; set; }
        public int GoodsId { get; set; }
    }
}