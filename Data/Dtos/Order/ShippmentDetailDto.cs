using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShippmentDetailDto
    {
        public long ItemId { get; set; }
        public long FkOrderId { get; set; }
        public int FkShopId { get; set; }
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public string GoodsCode { get; set; }
        public int FkVarietyId { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public int? FkShippingMethodId { get; set; }
        public string ShippingMethodTitle { get; set; }
        public string ShippmentDate { get; set; }
        public string DeliveredDate { get; set; }
        public decimal? ShippingCost { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string PlacedDateTime { get; set; }


        public int shopId { get; set; }
        public string ShopTitle { get; set; }


        public double? AdLocationX { get; set; }
        public double? AdLocationY { get; set; }
        public int? AdFkCountryId { get; set; }
        public string CountryTitle { get; set; }
        public int? AdFkCityId { get; set; }
        public string CityTitle { get; set; }
        public string AdPostalCode { get; set; }
        public string AdTransfereeTel { get; set; }
        public string AdTransfereeMobile { get; set; }
        public string AdTransfereeFamily { get; set; }
        public string AdTransfereeName { get; set; }
        public string AdAddress { get; set; }


        public decimal? UnitPrice { get; set; }
        public double? ItemCount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }

        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Heigth { get; set; }

    }
}