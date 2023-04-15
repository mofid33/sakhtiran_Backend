using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopGeneralDto
    {
        public int ShopId { get; set; }
        public short FkStatusId { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public int FkCityId { get; set; }
        public int FkPersonId { get; set; }
        public string VendorUrlid { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double Dist { get; set; }
        public double? SurveyScore { get; set; }


        public string CompanyName { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string LogoImage { get; set; }
        public double? LocationX { get; set; }
        public double? LocationY { get; set; }
        public string Address { get; set; }
        public string Iso { get; set; }
        public Guid UserId { get; set; }
        public string StatusTitle { get; set; }
        public short? MaxSliderForShopWebPage { get; set; }
        public bool? AutoAccountRecharge { get; set; }
        public string RegisteryDateTime { get; set; }
        public string ShopShippingCode { get; set; }

    }
}