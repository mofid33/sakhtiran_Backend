using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCustomer
    {
        public TCustomer()
        {
            TCallRequest = new HashSet<TCallRequest>();
            TCustomerAddress = new HashSet<TCustomerAddress>();
            TCustomerBankCard = new HashSet<TCustomerBankCard>();
            TDiscountCustomers = new HashSet<TDiscountCustomers>();
            TGoodsComment = new HashSet<TGoodsComment>();
            TGoodsLike = new HashSet<TGoodsLike>();
            TGoodsQueAns = new HashSet<TGoodsQueAns>();
            TGoodsSurveyAnswers = new HashSet<TGoodsSurveyAnswers>();
            TGoodsView = new HashSet<TGoodsView>();
            TOrder = new HashSet<TOrder>();
            TShopComment = new HashSet<TShopComment>();
            TShopCommentLike = new HashSet<TShopCommentLike>();
            TShopSurveyAnswers = new HashSet<TShopSurveyAnswers>();
            TUser = new HashSet<TUser>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public DateTime RegisteryDate { get; set; }
        public string NationalCode { get; set; }
        public string BirthDate { get; set; }
        public string MobileNumber { get; set; }
        public string TelNumber { get; set; }
        public string Email { get; set; }
        public int? FkCountryId { get; set; }
        public int? FkCityId { get; set; }
        public string BankCard { get; set; }
        public decimal Credit { get; set; }
        public string TempDiscountCode { get; set; }
        public bool? EmailVerifed { get; set; }
        public int? RefundPreference { get; set; }
        public int? FkProvinceId { get; set; }
        public bool? MobileVerifed { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TCountry FkCountry { get; set; }
        public virtual TProvince FkProvince { get; set; }
        public virtual ICollection<TCallRequest> TCallRequest { get; set; }
        public virtual ICollection<TCustomerAddress> TCustomerAddress { get; set; }
        public virtual ICollection<TCustomerBankCard> TCustomerBankCard { get; set; }
        public virtual ICollection<TDiscountCustomers> TDiscountCustomers { get; set; }
        public virtual ICollection<TGoodsComment> TGoodsComment { get; set; }
        public virtual ICollection<TGoodsLike> TGoodsLike { get; set; }
        public virtual ICollection<TGoodsQueAns> TGoodsQueAns { get; set; }
        public virtual ICollection<TGoodsSurveyAnswers> TGoodsSurveyAnswers { get; set; }
        public virtual ICollection<TGoodsView> TGoodsView { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TShopComment> TShopComment { get; set; }
        public virtual ICollection<TShopCommentLike> TShopCommentLike { get; set; }
        public virtual ICollection<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
        public virtual ICollection<TUser> TUser { get; set; }
    }
}
