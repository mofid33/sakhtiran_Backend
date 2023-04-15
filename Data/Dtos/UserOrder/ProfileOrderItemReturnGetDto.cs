using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class ProfileOrderItemReturnGetDto
    {
        public long ItemId { get; set; }
        public string Title { get; set; }
        public string GoodsImage { get; set; }
        public int GoodsId { get; set; }
        public string ModelNumber { get; set; }
        public string GoodsCode { get; set; }
        public string ShopName { get; set; }
        public bool ShopSpacialPage { get; set; }
        public string ShopUrl { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnAction { get; set; }
        public int ReturnActionId { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PriceWithDiscount { get; set; }
        public string OrderStatusPlacedDateTime { get; set; }
        public string TrackingCode { get; set; }
        public string CityTitle { get; set; }
        public string ProvinceTitle { get; set; }

        public string AdAddress { get; set; }
        public string AdTransfereeMobile { get; set; }
        public string AdTransfereeFamily { get; set; }
        public string AdTransfereeName { get; set; }
        public string Iso { get; set; }
        public string PhoneCode { get; set; }
        public string StatusTitle { get; set; }
        public int StatusId { get; set; }
        public double Quantity { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}