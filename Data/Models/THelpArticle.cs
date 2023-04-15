using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class THelpArticle
    {
        public int ArticleId { get; set; }
        public int FkTopicId { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int HelpfulCount { get; set; }
        public int UnhelpfulCount { get; set; }
        public bool Status { get; set; }

        public virtual THelpTopic FkTopic { get; set; }
    }
}
