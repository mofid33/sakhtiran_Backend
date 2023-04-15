using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopRegisterDto
    {
        public int ShopId { get; set; }
        public short FkStatusId { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public int FkCityId { get; set; }
        public int FkPersonId { get; set; }
        public string VendorUrlid { get; set; }
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string PhoneCode { get; set; }
        public string CompanyName { get; set; }
        public string FullName { get; set; }
        public double? LocationX { get; set; }
        public double? LocationY { get; set; }
        public string Address { get; set; }
        public string BankBeneficiaryName { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public string BankSwiftCode { get; set; }
        public int? FkCurrencyId { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public string CaptchaToken { get; set; }

        public List<ShopCategoryDto> TShopCategory { get; set; }
        public List<ShopFileDto> TShopFiles { get; set; }

    }
}