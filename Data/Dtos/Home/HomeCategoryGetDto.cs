using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.WebSlider;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class HomeCategoryGetDto
    {
        public List<WebHomeIndexModuleListDto> WebHomeModuleList { get; set; }
        public List<WebSliderGetListDto> Slider { get; set; }
        public CategoryWebDto Category { get; set; }
    }
}