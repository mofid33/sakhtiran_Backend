using System;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerListDto
    {
         
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }        
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public int? FkCountryId { get; set; }
        public int? FkProvinceId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public decimal Credit { get; set; }
        public string LastLogin { get; set; }
    }
}