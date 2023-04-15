using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopTaxDto
    {
        public int ShopId { get; set; }
        public bool GoodsIncludedVat { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public List<ShopFileDto> TShopFiles { get; set; }
    }
}