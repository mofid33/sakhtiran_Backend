using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TProvince
    {
        public TProvince()
        {
            TCity = new HashSet<TCity>();
            TCustomer = new HashSet<TCustomer>();
            TCustomerAddress = new HashSet<TCustomerAddress>();
            TOrder = new HashSet<TOrder>();
            TShippingOnCity = new HashSet<TShippingOnCity>();
            TShop = new HashSet<TShop>();
            TShopActivityCity = new HashSet<TShopActivityCity>();
        }

        public int ProvinceId { get; set; }
        public int FkCountryId { get; set; }
        public string ProvinceName { get; set; }
        public string PName { get; set; }
        public bool Status { get; set; }

        public virtual TCountry FkCountry { get; set; }
        public virtual ICollection<TCity> TCity { get; set; }
        public virtual ICollection<TCustomer> TCustomer { get; set; }
        public virtual ICollection<TCustomerAddress> TCustomerAddress { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TShippingOnCity> TShippingOnCity { get; set; }
        public virtual ICollection<TShop> TShop { get; set; }
        public virtual ICollection<TShopActivityCity> TShopActivityCity { get; set; }
    }
}
