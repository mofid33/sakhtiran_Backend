using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderDetailGetDto
    {
        public long OrderId { get; set; }
        //customer
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }

        //shiping address
        public double? AdLocationX { get; set; }
        public double? AdLocationY { get; set; }
        public int? AdFkCountryId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string AdPostalCode { get; set; }
        public string AdTransfereeTel { get; set; }
        public string AdTransfereeMobile { get; set; }
        public string AdTransfereeFamily { get; set; }
        public string AdTransfereeName { get; set; }
        public string AdAddress { get; set; }


        //total
        public decimal? Price { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }

        //payment information
        public bool PaymentStatus { get; set; }
        public int? FkPaymentMethodId { get; set; }
        public string MethodTitle { get; set; }

        public string TrackingCode { get; set; }
        public int? FkDiscountCodeId { get; set; }
        public string DiscountCode { get; set; }
        public bool? ReturningAllowed { get; set; }
        public bool? ReturningCanceld { get; set; }
        public string PlacedDateTime { get; set; }


        public List<OrderDetailListDto> Items { get; set; }


    }
}