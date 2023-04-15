using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCategory
    {
        public TCategory()
        {
            InverseFkParent = new HashSet<TCategory>();
            TCategoryBrand = new HashSet<TCategoryBrand>();
            TCategoryGuarantee = new HashSet<TCategoryGuarantee>();
            TCategorySpecification = new HashSet<TCategorySpecification>();
            TCategorySpecificationGroup = new HashSet<TCategorySpecificationGroup>();
            TDiscountCategory = new HashSet<TDiscountCategory>();
            TGoods = new HashSet<TGoods>();
            TGoodsSurveyQuestions = new HashSet<TGoodsSurveyQuestions>();
            TPopupItem = new HashSet<TPopupItem>();
            TShopCategory = new HashSet<TShopCategory>();
            TVariationPerCategory = new HashSet<TVariationPerCategory>();
            WebIndexModuleList = new HashSet<WebIndexModuleList>();
            WebSlider = new HashSet<WebSlider>();
        }

        public int CategoryId { get; set; }
        public int? FkParentId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryPath { get; set; }
        public string IconUrl { get; set; }
        public int PriorityNumber { get; set; }
        public double? CommissionFee { get; set; }
        public bool ReturningAllowed { get; set; }
        public bool AppearInFooter { get; set; }
        public string PageTitle { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public bool ToBeDisplayed { get; set; }
        public bool HaveWebPage { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }

        public virtual TCategory FkParent { get; set; }
        public virtual ICollection<TCategory> InverseFkParent { get; set; }
        public virtual ICollection<TCategoryBrand> TCategoryBrand { get; set; }
        public virtual ICollection<TCategoryGuarantee> TCategoryGuarantee { get; set; }
        public virtual ICollection<TCategorySpecification> TCategorySpecification { get; set; }
        public virtual ICollection<TCategorySpecificationGroup> TCategorySpecificationGroup { get; set; }
        public virtual ICollection<TDiscountCategory> TDiscountCategory { get; set; }
        public virtual ICollection<TGoods> TGoods { get; set; }
        public virtual ICollection<TGoodsSurveyQuestions> TGoodsSurveyQuestions { get; set; }
        public virtual ICollection<TPopupItem> TPopupItem { get; set; }
        public virtual ICollection<TShopCategory> TShopCategory { get; set; }
        public virtual ICollection<TVariationPerCategory> TVariationPerCategory { get; set; }
        public virtual ICollection<WebIndexModuleList> WebIndexModuleList { get; set; }
        public virtual ICollection<WebSlider> WebSlider { get; set; }
    }
}
