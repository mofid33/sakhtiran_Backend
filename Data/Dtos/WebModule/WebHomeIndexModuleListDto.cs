using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    // لیست ماژول هایی که به صفحه ی اصلی میفرستیم
    public partial class WebHomeIndexModuleListDto 
    {
        public int IModuleId { get; set; }
        public int FkModuleId { get; set; }
        public int? SequenceNumber { get; set; }
        public bool Status { get; set; }
        public string SelectedHeight { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string IModuleTitle { get; set; }
        public string ModuleTitle { get; set; }
        public int? FkCategoryId { get; set; }
        public List<WebHomeModuleCollectionsDto> WebModuleCollections { get; set; }
    }
}
