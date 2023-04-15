using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryAddGetDto
    {
        public int CategoryId { get; set; }
        public int? FkParentId { get; set; }
        public string ParentTitle { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryParentPath { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public int PriorityNumber { get; set; }
        public double? CommissionFee { get; set; }
        public bool? ReturningAllowed { get; set; }
        public bool AppearInFooter { get; set; }
        public string PageTitle { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsActive { get; set; }
        public bool HaveWebPage { get; set; }
    }
}