using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.Setting;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class FooterGetDto
    {
        public List<CategoryFormDto> Footer { get; set; }
        public WebsiteSettingWebDto  Links { get; set; }
    }
}