using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoods
    {
        public TGoods()
        {
            TCallRequest = new HashSet<TCallRequest>();
            TDiscountFreeGoods = new HashSet<TDiscountFreeGoods>();
            TDiscountGoods = new HashSet<TDiscountGoods>();
            TGoodsComment = new HashSet<TGoodsComment>();
            TGoodsDocument = new HashSet<TGoodsDocument>();
            TGoodsLike = new HashSet<TGoodsLike>();
            TGoodsProvider = new HashSet<TGoodsProvider>();
            TGoodsQueAns = new HashSet<TGoodsQueAns>();
            TGoodsSpecification = new HashSet<TGoodsSpecification>();
            TGoodsSurveyAnswers = new HashSet<TGoodsSurveyAnswers>();
            TGoodsVariety = new HashSet<TGoodsVariety>();
            TGoodsView = new HashSet<TGoodsView>();
            TOrderItem = new HashSet<TOrderItem>();
        }

        public int GoodsId { get; set; }
        public int FkCategoryId { get; set; }
        public int? FkBrandId { get; set; }
        public int? FkUnitId { get; set; }
        public int? FkOwnerId { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Heigth { get; set; }
        public long ViewCount { get; set; }
        public long SaleCount { get; set; }
        public long ReturnedCount { get; set; }
        public long LikedCount { get; set; }
        public int? SurveyCount { get; set; }
        public double? SurveyScore { get; set; }
        public bool HaveVariation { get; set; }
        public bool IsCommonGoods { get; set; }
        public bool IsDownloadable { get; set; }
        public string DownloadableFileUrl { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string PageTitle { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsAccepted { get; set; }
        public bool SaleWithCall { get; set; }

        public virtual TBrand FkBrand { get; set; }
        public virtual TCategory FkCategory { get; set; }
        public virtual TShop FkOwner { get; set; }
        public virtual TMeasurementUnit FkUnit { get; set; }
        public virtual ICollection<TCallRequest> TCallRequest { get; set; }
        public virtual ICollection<TDiscountFreeGoods> TDiscountFreeGoods { get; set; }
        public virtual ICollection<TDiscountGoods> TDiscountGoods { get; set; }
        public virtual ICollection<TGoodsComment> TGoodsComment { get; set; }
        public virtual ICollection<TGoodsDocument> TGoodsDocument { get; set; }
        public virtual ICollection<TGoodsLike> TGoodsLike { get; set; }
        public virtual ICollection<TGoodsProvider> TGoodsProvider { get; set; }
        public virtual ICollection<TGoodsQueAns> TGoodsQueAns { get; set; }
        public virtual ICollection<TGoodsSpecification> TGoodsSpecification { get; set; }
        public virtual ICollection<TGoodsSurveyAnswers> TGoodsSurveyAnswers { get; set; }
        public virtual ICollection<TGoodsVariety> TGoodsVariety { get; set; }
        public virtual ICollection<TGoodsView> TGoodsView { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
    }
}
