using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class CCategoryConvert
    {
        public int Id { get; set; }
        public string MultiTitle { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string ParentId { get; set; }
        public string ParentBackup { get; set; }
        public string Location { get; set; }
        public string Path { get; set; }
        public string PathTemp { get; set; }
        public string PathIds { get; set; }
        public string Arabic { get; set; }
        public string MainMenu { get; set; }
        public string AllowReturn { get; set; }
        public string Footer { get; set; }
        public string Active { get; set; }
    }
}
