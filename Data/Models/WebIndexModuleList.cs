using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebIndexModuleList
    {
        public WebIndexModuleList()
        {
            WebModuleCollections = new HashSet<WebModuleCollections>();
        }

        public int IModuleId { get; set; }
        public int FkModuleId { get; set; }
        public int? SequenceNumber { get; set; }
        public bool Status { get; set; }
        public string SelectedHeight { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string ModuleTitle { get; set; }
        public int? FkCategoryId { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual WebModule FkModule { get; set; }
        public virtual ICollection<WebModuleCollections> WebModuleCollections { get; set; }
    }
}
