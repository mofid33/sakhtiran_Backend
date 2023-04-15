namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerAddressDto 
    {
        public int AddressId { get; set; }
        public string TransfereeMobile { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public string CountryName { get; set; }
        public int FkCityId { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string PhoneCode { get; set; }
        public string Address { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public string TransfereeName { get; set; }
        public bool? IsDefualt { get; set; }
        public string TransfereeFamily { get; set; }
        public bool MobileVerifed { get; set; }
        public string Iso { get; set; }

    }
}