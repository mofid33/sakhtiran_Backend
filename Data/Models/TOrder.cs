using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrder
    {
        public TOrder()
        {
            TOrderCanceling = new HashSet<TOrderCanceling>();
            TOrderItem = new HashSet<TOrderItem>();
            TOrderLog = new HashSet<TOrderLog>();
            TOrderReturning = new HashSet<TOrderReturning>();
            TPaymentTransaction = new HashSet<TPaymentTransaction>();
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public long OrderId { get; set; }
        public int FkOrderStatusId { get; set; }
        public int FkCustomerId { get; set; }
        public int? FkDiscountCodeId { get; set; }
        public int? FkPaymentMethodId { get; set; }
        public DateTime InitialDateTime { get; set; }
        public DateTime? PlacedDateTime { get; set; }
        public string TrackingCode { get; set; }
        public bool PaymentStatus { get; set; }
        public decimal? Price { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }
        public double? AdLocationX { get; set; }
        public double? AdLocationY { get; set; }
        public int? AdFkCountryId { get; set; }
        public int? AdFkProvinceId { get; set; }
        public int? AdFkCityId { get; set; }
        public string AdAddress { get; set; }
        public string AdPostalCode { get; set; }
        public string AdTransfereeTel { get; set; }
        public string AdTransfereeMobile { get; set; }
        public string AdTransfereeFamily { get; set; }
        public string AdTransfereeName { get; set; }
        public Guid? CookieId { get; set; }

        public virtual TCity AdFkCity { get; set; }
        public virtual TCountry AdFkCountry { get; set; }
        public virtual TProvince AdFkProvince { get; set; }
        public virtual TCustomer FkCustomer { get; set; }
        public virtual TDiscountCouponCode FkDiscountCode { get; set; }
        public virtual TOrderStatus FkOrderStatus { get; set; }
        public virtual TPaymentMethod FkPaymentMethod { get; set; }
        public virtual ICollection<TOrderCanceling> TOrderCanceling { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
        public virtual ICollection<TOrderLog> TOrderLog { get; set; }
        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
        public virtual ICollection<TPaymentTransaction> TPaymentTransaction { get; set; }
        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
