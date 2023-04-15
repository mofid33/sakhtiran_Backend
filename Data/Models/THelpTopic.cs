using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class THelpTopic
    {
        public THelpTopic()
        {
            InverseFkTopic = new HashSet<THelpTopic>();
            THelpArticle = new HashSet<THelpArticle>();
        }

        public int TopicId { get; set; }
        public int? FkTopicId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool Status { get; set; }

        public virtual THelpTopic FkTopic { get; set; }
        public virtual ICollection<THelpTopic> InverseFkTopic { get; set; }
        public virtual ICollection<THelpArticle> THelpArticle { get; set; }
    }
}
