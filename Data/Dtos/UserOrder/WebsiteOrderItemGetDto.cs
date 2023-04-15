using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Home;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class WebsiteOrderItemGetDto
    {
        public long ItemId { get; set; }
        public double Quantity { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Heigth { get; set; }
        public double Width { get; set; }
        public string Title { get; set; }
        public string ModelNumber { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsImage { get; set; }
        public string GoodsBrand { get; set; }
        public string StoreName { get; set; }
        public List<HomeGoodsVarietyGetDto> GoodsVariety { get; set; }
        public int GoodsId { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int ShopId { get; set; }
        public int PostId { get; set; }
        public bool ShopHaveMicroStore { get; set; }
        public string ShopUrl { get; set; }
        public string ShopShippingCode { get; set; }

        public bool Exist { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Shipping { get; set; }
        public int ShippingDay { get; set; }
        public decimal PriceWithDiscount { get; set; }
        public decimal DiscountCouponAmount { get; set; }
        public int Method { get; set; }
        public bool ShippingAvailable { get; set; }
        public bool SaleWithCall { get; set; }
        public bool IsDownloadable { get; set; }
        public bool HaveGuarantee { get; set; }
        public bool ReturningAllowed { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string CityTitle { get; set; }
        public string CountryTitle { get; set; }
        public string CountryIso { get; set; }
        public int ProvinceId { get; set; }
        public double InventoryCount { get; set; }
    }
}