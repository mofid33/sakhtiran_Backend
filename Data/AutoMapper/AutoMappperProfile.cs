using AutoMapper;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Models;
using System.Linq;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Recommendation;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.NotificationSetting;
using MarketPlace.API.Data.Dtos.PupupItem;

namespace MarketPlace.API.Data.AutoMapper
{
    public class AutoMappperProfile : Profile //use reverce map
    {
        public AutoMappperProfile()
        {
            //TGuaranteeType
            CreateMap<TGuarantee, GuaranteeDto>().ReverseMap();
            CreateMap<TGuarantee, GuaranteeFormDto>().ReverseMap();

            //TCategoryBrand
            CreateMap<TCategoryGuarantee, CategoryGuaranteeDto>().ReverseMap();
            CreateMap<TCategoryGuarantee, CategoryGuaranteeGetDto>()
            .ForMember(dest => dest.CategoryTitle,
            opt => opt.MapFrom(src => src.FkCategory.CategoryTitle));

            //TBrand
            CreateMap<TBrand, BrandDto>().ReverseMap();
            CreateMap<TBrand, BrandFormDto>().ReverseMap();
            CreateMap<TBrand, BrandGetOneDto>().ReverseMap();

            
            // Pupup ite
            CreateMap<TPopupItem, PupupItemDto>().ReverseMap();

            //TCategoryBrand
            CreateMap<TCategoryBrand, CategoryBrandDto>().ReverseMap();
            CreateMap<TCategoryBrand, CategoryBrandGetDto>()
            .ForMember(dest => dest.CategoryTitle,
            opt => opt.MapFrom(src => src.FkCategory.CategoryTitle)).ReverseMap();

            //TCountry
            CreateMap<TCountry, CountryDto>().ReverseMap();

            //TCity
            CreateMap<TCity, CityDto>().ReverseMap();
            CreateMap<TCity, CityGetDto>()
            .ForMember(dest => dest.CountryTitle,
            opt => opt.MapFrom(src => src.FkCountry.CountryTitle))
            .ForMember(dest => dest.ProvinceName,
            opt => opt.MapFrom(src => src.FkProvince.ProvinceName))
            .ReverseMap();

            //TProvince
            CreateMap<TProvince, ProvinceDto>().ReverseMap();
            CreateMap<TProvince, ProvinceGetDto>()
            .ForMember(dest => dest.CountryTitle,
            opt => opt.MapFrom(src => src.FkCountry.CountryTitle))
            .ReverseMap();

            //TCategory
            CreateMap<CategoryAddGetDto, TCategory>().ReverseMap();
            CreateMap<CategoryGetDto, CategoryTreeView>().ReverseMap();
            CreateMap<CategoryEditDto, TCategory>().ReverseMap();


            //SpecificationGroup
            CreateMap<TSpecificationGroup, SpecificationGroupDto>().ReverseMap();

            //TSpecificationOptions
            CreateMap<TSpecificationOptions, SpecificationOptionsDto>().ReverseMap();

            //TGoodsSpecification
            CreateMap<TGoodsSpecification, GoodsSpecificationDto>().ReverseMap();

            //TGoodsSpecificationOptions
            CreateMap<TGoodsSpecificationOptions, GoodsSpecificationOptionsDto>().ReverseMap();

            // //TCategorySpecification
            CreateMap<CategorySpecificationAddDto, TCategorySpecification>().ReverseMap();
            CreateMap<TCategorySpecification, CategorySpecificationGetDto>()
            .ForMember(dest => dest.SpecTitle,
            opt => opt.MapFrom(src => src.FkSpec.SpecTitle))
            .ForMember(dest => dest.CategoryTitle,
            opt => opt.MapFrom(src => src.FkCategory.CategoryTitle)).ReverseMap();

            // //TSpecification
            CreateMap<SpecificationAddGetDto, TSpecification>().ReverseMap();
            CreateMap<TSpecification, SpecificationGetDto>()
            .ForMember(dest => dest.IsMultiSelectInFilter,
            opt => opt.MapFrom(src => src.IsMultiSelectInFilter)).ReverseMap();
            CreateMap<TSpecification, SpecificationFormDto>().ReverseMap();


            //TCategorySpecificationGroup
            CreateMap<TCategorySpecificationGroup, SpecificationGroupFromDto>()
            .ForMember(dest => dest.SpecGroupTitle,
            opt => opt.MapFrom(src => src.FkSpecGroup.SpecGroupTitle)).ReverseMap();

            //TOrderCancelingReason
            CreateMap<TOrderCancelingReason, OrderCancelingReasonDto>().ReverseMap();

            //TVariationParameter
            CreateMap<TVariationParameter, VariationParameterDto>().ReverseMap();

            //TVariationParameterValues
            CreateMap<TVariationParameterValues, VariationParameterValuesDto>().ReverseMap();

            //TVariationParameter
            CreateMap<TVariationPerCategory, VariationPerCategoryDto>().ReverseMap();

            //TReturningReason
            CreateMap<TReturningReason, ReturningReasonDto>().ReverseMap();

            //TSetting
            CreateMap<TSetting, WebsiteSettingDto>().ReverseMap();

            //Web Module
            CreateMap<WebIndexModuleList, WebIndexModuleListDto>().ReverseMap();
            CreateMap<WebIndexModuleList, WebIndexModuleListAddDto>().ReverseMap();
            CreateMap<WebModule, WebModuleDto>().ReverseMap();
            CreateMap<WebModuleCollections, WebModuleCollectionsDto>().ReverseMap();
            CreateMap<WebModuleCollections, WebModuleCollectionsAddDto>().ReverseMap();
            CreateMap<WebModuleCollections, WebModuleCollectionsGetDto>().ReverseMap();
            CreateMap<WebCollectionType, WebCollectionTypeDto>().ReverseMap();

            // slider module
            CreateMap<WebSlider, WebSliderAddDto>().ReverseMap();
            CreateMap<WebSlider, WebSliderGetDto>().ReverseMap();
            CreateMap<WebSlider, WebSliderGetListDto>().ReverseMap();

            //TGoods
            CreateMap<TGoods, GoodsDto>().ReverseMap();
            CreateMap<TGoodsProvider, GoodsProviderAddDto>().ReverseMap();
            CreateMap<TGoodsVariety, GoodsVarietyDto>().ReverseMap();

            //TshopFile
            CreateMap<TShopFiles, ShopFileDto>().ReverseMap();

            //TShopActivityCountry
            CreateMap<TShopActivityCountry, ShopActivityCountryDto>().ReverseMap();

            //TShopActivityCity
            CreateMap<TShopActivityCity, ShopActivityCityDto>().ReverseMap();

            //TShopSlider
            CreateMap<ShopSliderDto, TShopSlider>().ReverseMap();

            //TShopStatus
            CreateMap<ShopStatusDto, TShopStatus>().ReverseMap();

            //TShopStatus
            CreateMap<TGoodsDocument, GoodsDocumentDto>().ReverseMap();

            //TGoodsProvider
            CreateMap<TGoodsProvider, GoodsProviderDto>().ReverseMap();

            //TDocumentType
            CreateMap<DocumentTypeDto, TDocumentType>().ReverseMap();


            //tDiscount

            CreateMap<TDiscountPlan, DiscountPlanAddDto>().ReverseMap();
            CreateMap<DiscountGoodsDto, TDiscountGoods>().ReverseMap();
            CreateMap<DiscountCustomersDto, TDiscountCustomers>().ReverseMap();
            CreateMap<DiscountCategoryDto, TDiscountCategory>().ReverseMap();
            CreateMap<DiscountFreeGoodsDto, TDiscountFreeGoods>().ReverseMap();
            CreateMap<DiscountShopsDto, TDiscountShops>().ReverseMap();
            CreateMap<TDiscountPlan, DiscountPlanEditDto>().ReverseMap();
            CreateMap<TDiscountCouponCode, DiscountCodeDto>().ReverseMap();

            // register
            CreateMap<TUser, UserRegisterDto>().ReverseMap();
            CreateMap<TUser, UserAccessDto>().ReverseMap();
            CreateMap<TUserAccessControl, AccessControlDto>().ReverseMap();
            CreateMap<TCustomer, CustomerGeneralDetailDto>().ReverseMap();

            // TShippingMethod
            CreateMap<TShippingMethod, ShippingMethodDto>().ReverseMap();
            CreateMap<TShippingOnCountry, ShippingOnCountryDto>().ReverseMap();
            CreateMap<TShippingOnCity, ShippingOnCityDto>().ReverseMap();

            //tShop
            CreateMap<TShop, ShopRegisterDto>().ReverseMap();
            CreateMap<TShopCategory, ShopCategoryDto>().ReverseMap();
            CreateMap<TShopPlan, ShopPlanDto>().ReverseMap();
            CreateMap<TShopPlanExclusive, ShopPlanExclusiveDto>().ReverseMap();

            // Tcustomer
            CreateMap<TCustomerAddress, CustomerAddressDto>().ReverseMap();

            CreateMap<TOrderReturning, OrderReturningAddDto>().ReverseMap();
            CreateMap<TOrderCanceling, OrderCancelingAddDto>().ReverseMap();


            CreateMap<TMessage, MessageAddDto>().ReverseMap();
            CreateMap<TMessageAttachment, MessageAttachmentDto>().ReverseMap();


            CreateMap<THelpTopic, HelpTopicAddDto>().ReverseMap();
            CreateMap<THelpArticle, HelpArticleAddDto>().ReverseMap();

            CreateMap<TRecommendation, RecommendationAddDto>().ReverseMap();
            CreateMap<TShopSurveyQuestions, ShopSurveyQuestionsDto>().ReverseMap();

            CreateMap<TGoodsComment, GoodsCommentAddDto>().ReverseMap();


            // TNotificationSetting
            CreateMap<TNotificationSetting, NotificationSettingDto>().ReverseMap();

            // area
            CreateMap<TShippingMethodAreaCode, CityShippingMethodAreaCodeDto>().ReverseMap();

        }
    }
}