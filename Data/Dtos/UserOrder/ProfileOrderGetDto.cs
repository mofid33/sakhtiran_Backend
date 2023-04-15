using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class ProfileOrderGetDto
    {
        public long OrderId { get; set; }
        public string TrackingCode { get; set; }
        public string PlacedDateTime { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal Shipping { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalWithOutDiscountCode { get; set; }
        public decimal Total { get; set; }

        public string TransfereeName { get; set; }
        public string TransfereeFamily { get; set; }
        public string TransfereeMobile { get; set; }
        public string Address { get; set; }
        public string ProvinceTitle { get; set; }
        public string CityTitle { get; set; }
        public string StatusTitle { get; set; }
        public int StatusId { get; set; }
        public double? OrderItemCount { get; set; }
        public bool? ReturningAllowed { get; set; }
        public bool? CancelingAllowed { get; set; }

        public string Payment { get; set; }
        public string Iso { get; set; }
        public double? ItemQuantity { get; set; }
        public List<ProfileOrderItemGetDto> Items { get; set; }
    }
}