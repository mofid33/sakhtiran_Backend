using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public partial class WebModuleDto 
    {
        public int ModuleId { get; set; }
        public string ModuleTitle { get; set; }
        public bool HaveLinkCapability { get; set; }
        public int MaxCollectionItemCount { get; set; }
        public string Description { get; set; }
    }
}
