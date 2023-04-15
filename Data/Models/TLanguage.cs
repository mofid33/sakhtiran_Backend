using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TLanguage
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageTitle { get; set; }
        public string JsonFile { get; set; }
        public bool IsRtl { get; set; }
        public bool DefaultLanguage { get; set; }
    }
}
