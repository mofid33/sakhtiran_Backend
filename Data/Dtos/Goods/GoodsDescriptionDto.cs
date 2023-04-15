using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsDescriptionDto
    {
        public int GoodsId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}