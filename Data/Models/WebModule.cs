using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class WebModule
    {
        public WebModule()
        {
            WebIndexModuleList = new HashSet<WebIndexModuleList>();
        }

        public int ModuleId { get; set; }
        public string ModuleTitle { get; set; }
        public bool HaveLinkCapability { get; set; }
        public int MaxCollectionItemCount { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WebIndexModuleList> WebIndexModuleList { get; set; }
    }
}
