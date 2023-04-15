namespace MarketPlace.API.Data.Dtos.WebModule
{
    public class WebIndexModuleListAddDto 
    {
        public int IModuleId { get; set; }
        public int FkModuleId { get; set; }
        public int? SequenceNumber { get; set; }
        public bool Status { get; set; }
        public string SelectedHeight { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string ModuleTitle { get; set; }
        public int? FkCategoryId { get; set; }

    }
}