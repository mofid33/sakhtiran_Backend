using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShippingMethod
    {
        public TShippingMethod()
        {
            TOrderItem = new HashSet<TOrderItem>();
            TOrderReturning = new HashSet<TOrderReturning>();
            TShippingMethodAreaCode = new HashSet<TShippingMethodAreaCode>();
            TShippingOnCity = new HashSet<TShippingOnCity>();
            TShippingOnCountry = new HashSet<TShippingOnCountry>();
            TShopActivityCity = new HashSet<TShopActivityCity>();
            TShopActivityCountry = new HashSet<TShopActivityCountry>();
        }

        public int Id { get; set; }
        public string ShippingMethodTitle { get; set; }
        public bool CashOnDelivery { get; set; }
        public bool HaveOnlineService { get; set; }
        public int? BaseWeight { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
        public virtual ICollection<TShippingMethodAreaCode> TShippingMethodAreaCode { get; set; }
        public virtual ICollection<TShippingOnCity> TShippingOnCity { get; set; }
        public virtual ICollection<TShippingOnCountry> TShippingOnCountry { get; set; }
        public virtual ICollection<TShopActivityCity> TShopActivityCity { get; set; }
        public virtual ICollection<TShopActivityCountry> TShopActivityCountry { get; set; }
    }
}
