using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsDocumentAddDto
    {
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public IFormFile Document { get; set; }
    }
}