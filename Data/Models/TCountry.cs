using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCountry
    {
        public TCountry()
        {
            TCity = new HashSet<TCity>();
            TCustomer = new HashSet<TCustomer>();
            TCustomerAddress = new HashSet<TCustomerAddress>();
            TOrder = new HashSet<TOrder>();
            TProvince = new HashSet<TProvince>();
            TShippingOnCountry = new HashSet<TShippingOnCountry>();
            TShop = new HashSet<TShop>();
            TShopActivityCountry = new HashSet<TShopActivityCountry>();
        }

        public int CountryId { get; set; }
        public string Iso3 { get; set; }
        public string CountryTitle { get; set; }
        public string FlagUrl { get; set; }
        public decimal Vat { get; set; }
        public bool Status { get; set; }
        public string Iso2 { get; set; }
        public string PhoneCode { get; set; }
        public bool? DefualtPreCode { get; set; }

        public virtual ICollection<TCity> TCity { get; set; }
        public virtual ICollection<TCustomer> TCustomer { get; set; }
        public virtual ICollection<TCustomerAddress> TCustomerAddress { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TProvince> TProvince { get; set; }
        public virtual ICollection<TShippingOnCountry> TShippingOnCountry { get; set; }
        public virtual ICollection<TShop> TShop { get; set; }
        public virtual ICollection<TShopActivityCountry> TShopActivityCountry { get; set; }
    }
}
