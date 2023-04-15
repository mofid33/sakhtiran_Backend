using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSpecification
    {
        public TSpecification()
        {
            TCategorySpecification = new HashSet<TCategorySpecification>();
            TGoodsSpecification = new HashSet<TGoodsSpecification>();
            TSpecificationOptions = new HashSet<TSpecificationOptions>();
        }

        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public int FkSpecGroupId { get; set; }
        public bool IsKeySpec { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsMultiSelectInFilter { get; set; }
        public bool IsMultiLineText { get; set; }
        public int? PriorityNumber { get; set; }
        public bool? Status { get; set; }

        public virtual TSpecificationGroup FkSpecGroup { get; set; }
        public virtual ICollection<TCategorySpecification> TCategorySpecification { get; set; }
        public virtual ICollection<TGoodsSpecification> TGoodsSpecification { get; set; }
        public virtual ICollection<TSpecificationOptions> TSpecificationOptions { get; set; }
    }
}
