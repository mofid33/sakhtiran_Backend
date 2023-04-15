using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class ShopWebsiteDetailDto
    {
        public int ShopId { get; set; }
        public string CountryTitle { get; set; }
        public string CityTitle { get; set; }
        public string RegisteryDateTime { get; set; }
        public string StoreName { get; set; }
        public string Phone { get; set; }
        public string Iso { get; set; }
        public double? LocationX { get; set; }
        public double? LocationY { get; set; }
        public string Address { get; set; }
        public string LogoImage { get; set; }
        public string ProfileImage { get; set; }
        public string TermCondition { get; set; }
        public double SurveyScore { get; set; }
        public int ProductsSold { get; set; }
        public bool IsDefualtImage { get; set; }
        public int ShopStatus { get; set; }
        public List<ShopSliderGetDto> ShopSlider { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}