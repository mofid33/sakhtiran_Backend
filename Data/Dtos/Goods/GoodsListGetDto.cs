using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsListGetDto
    {
        public int GoodsId { get; set; }
        public string CategoryTitle { get; set; }
        public int CategoryId { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string BrandTitle { get; set; }
        public string RegisterDate { get; set; }
        public string ShopList { get; set; }
        public bool IsCommon { get; set; }
        public bool HaveVariation { get; set; }
        public List<ShopFormDto> Shops { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public ShopFormDto Shop { get; set; }

    }
}