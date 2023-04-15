using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCustomerAddress
    {
        public int AddressId { get; set; }
        public int FkCustomerId { get; set; }
        public string TransfereeName { get; set; }
        public string TransfereeFamily { get; set; }
        public string TransfereeMobile { get; set; }
        public bool MobileVerifed { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public int FkCityId { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public bool? IsDefualt { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TCountry FkCountry { get; set; }
        public virtual TCustomer FkCustomer { get; set; }
        public virtual TProvince FkProvince { get; set; }
    }
}
