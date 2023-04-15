using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public class WebIndexModuleListDto
    {
        public int IModuleId { get; set; }
        public int FkModuleId { get; set; }
        public int? SequenceNumber { get; set; }
        public bool Status { get; set; }
        public string SelectedHeight { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string ModuleTitle { get; set; }
        public int? FkCategoryId { get; set; }

        public WebModuleDto FkModule { get; set; }
        public List<WebModuleCollectionsDto> WebModuleCollections { get; set; }
    }
}