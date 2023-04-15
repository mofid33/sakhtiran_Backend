using System;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerGeneralDetailDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public bool EmailVerifed { get; set; }
        public bool MobileVerifed { get; set; }
        public string SocialID { get; set; }
        public int? FkCountryId { get; set; }
        public int? FkProvinceId { get; set; }
        public int? FkCityId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string LastLogin { get; set; }
        public string RegisteryDate { get; set; }
        public string NationalCode { get; set; }
        public string BirthDate { get; set; }
        public string RefundPreference { get; set; }
        public int RefundPreferenceId { get; set; }
        public Guid UserId { get; set; }
        public string Iso { get; set; }
        public string PhoneCode { get; set; }
        public decimal Credit { get; set; }
    }
}