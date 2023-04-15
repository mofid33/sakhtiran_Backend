using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TMessageAttachment
    {
        public int AttachmentId { get; set; }
        public int FkMessageId { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }

        public virtual TMessage FkMessage { get; set; }
    }
}
