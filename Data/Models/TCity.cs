using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCity
    {
        public TCity()
        {
            TCustomer = new HashSet<TCustomer>();
            TCustomerAddress = new HashSet<TCustomerAddress>();
            TOrder = new HashSet<TOrder>();
            TShippingMethodAreaCode = new HashSet<TShippingMethodAreaCode>();
            TShippingOnCity = new HashSet<TShippingOnCity>();
            TShop = new HashSet<TShop>();
            TShopActivityCity = new HashSet<TShopActivityCity>();
        }

        public int CityId { get; set; }
        public string CityTitle { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool? IsCapital { get; set; }
        public string CName { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }

        public virtual TCountry FkCountry { get; set; }
        public virtual TProvince FkProvince { get; set; }
        public virtual ICollection<TCustomer> TCustomer { get; set; }
        public virtual ICollection<TCustomerAddress> TCustomerAddress { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TShippingMethodAreaCode> TShippingMethodAreaCode { get; set; }
        public virtual ICollection<TShippingOnCity> TShippingOnCity { get; set; }
        public virtual ICollection<TShop> TShop { get; set; }
        public virtual ICollection<TShopActivityCity> TShopActivityCity { get; set; }
    }
}
