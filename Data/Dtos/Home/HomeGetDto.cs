using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.Setting;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class HomeGetDto
    {
        public List<WebHomeIndexModuleListDto> WebHomeModuleList { get; set; }
        public List<WebSliderGetListDto> Slider { get; set; }
    }
}