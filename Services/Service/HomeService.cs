using System;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Pagination;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.WebModule;

namespace MarketPlace.API.Services.Service
{
    public class HomeService : IHomeService
    {
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }
        public ICategoryService _categoryService { get; set; }
        public ISpecificationRepository _specificationRepository { get; set; }
        public IHomeRepository _homeRepository { get; set; }
        public IBrandRepository _brandRepository { get; set; }
        public IWebSliderRepository _webSliderRepository { get; set; }
        public ISettingRepository _settingRepository { get; set; }
        public IMapper _mapper { get; set; }
        public IWebModuleRepository _webModuleRepository { get; set; }
        public IGoodsCommentRepository _goodsCommentRepository { get; set; }
        public IUserActivityRepository _userActivityRepository { get; set; }
        public IUserOrderRepository _userOrderRepository { get; set; }
        public IShopRepository _shopRepository { get; set; }
        public IForceUpdateRepository _forceUpdateRepository { get; set; }
        public ICustomerRepository _customerRepository { get; set; }
        public IDiscountRepository _discountRepository { get; set; }
        public IHelpRepository _helpRepository { get; set; }
        public IRecommendationRepository _recommendationRepository { get; set; }
        public IPupupRepository _PupupRepository { get; }

        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public HomeService(IMessageLanguageService ms,
        IHomeRepository homeRepository,
        IDiscountRepository discountRepository,
        IWebSliderRepository webSliderRepository,
        ISettingRepository settingRepository,
        IPupupRepository PupupRepository,
        IHelpRepository helpRepository,
        IMapper mapper,
        ISpecificationRepository specificationRepository,
        IBrandRepository brandRepository,
        ICategoryService categoryService,
        IWebModuleRepository webModuleRepository,
        ICategoryRepository categoryRepository,
        IGoodsCommentRepository goodsCommentRepository,
        IUserActivityRepository userActivityRepository,
        IShopRepository shopRepository,
        ICustomerRepository customerRepository,
        IUserOrderRepository userOrderRepository,
        IRecommendationRepository recommendationRepository,
        IHttpContextAccessor httpContextAccessor,
        IForceUpdateRepository forceUpdateRepository)
        {
            _forceUpdateRepository = forceUpdateRepository;
            _discountRepository = discountRepository;
            _userOrderRepository = userOrderRepository;
            _specificationRepository = specificationRepository;
            _brandRepository = brandRepository;
            this._PupupRepository = PupupRepository;
            _categoryService = categoryService;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _httpContextAccessor = httpContextAccessor;
            _ms = ms;
            _categoryRepository = categoryRepository;
            _homeRepository = homeRepository;
            _webSliderRepository = webSliderRepository;
            _settingRepository = settingRepository;
            _mapper = mapper;
            _webModuleRepository = webModuleRepository;
            _customerRepository = customerRepository;
            _goodsCommentRepository = goodsCommentRepository;
            _userActivityRepository = userActivityRepository;
            _shopRepository = shopRepository;
            _helpRepository = helpRepository;
            _recommendationRepository = recommendationRepository;
        }

        public async Task<ApiResponse<HomeGetDto>> HomeGet(string ipAddress)
        {
            // type == 1 all nav and modules
            // type == 2 all nav 
            // type == 3 all module 
            var data = new HomeGetDto();
            var rate = (decimal)await _settingRepository.GetCurrencyRate();
            var WebModuleSetting = await _settingRepository.WebsiteSettingGet();
            data.Slider = await _webSliderRepository.SliderGet(1, rate, null);
            data.WebHomeModuleList = await _webModuleRepository.GetModuleCollection(1, rate, null); // get type 1 for website
            int indexCollectionLastDay = -WebModuleSetting.IndexCollectionLastDay;
            var fromDay = DateTime.Now.AddDays(indexCollectionLastDay);
            var toDay = DateTime.Now;

            var GoodsIdsInOrder = new List<int>();
            if (token.Rule == UserGroupEnum.Customer)
            {
                if (token.CookieId != Guid.Empty || token.Id != 0)
                {
                    GoodsIdsInOrder = await _userOrderRepository.GetGoodsIdsInOrder(token.Id, token.CookieId);
                }
            }

            foreach (var module in data.WebHomeModuleList)
            {
                if (module.FkModuleId == (int)ModuleTypeEnum.ProductList)
                {
                    foreach (var collection in module.WebModuleCollections)
                    {
                        var xitemIds = new List<int>();
                        if (!string.IsNullOrWhiteSpace(collection.XitemIds))
                        {
                            var baseIds = collection.XitemIds.Split(',');
                            for (int i = 0; i < baseIds.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(baseIds[i]))
                                {
                                    xitemIds.Add(Int32.Parse(baseIds[i]));
                                }
                            }
                        }
                        if (collection.HaveLink == false)
                        {
                            if (collection.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                            {
                                collection.CriteriaFrom = collection.CriteriaFrom / rate;
                                collection.CriteriaTo = collection.CriteriaTo / rate;
                            }
                            switch (collection.FkCollectionTypeId)
                            {
                                case (int)CollectionTypeEnum.New:
                                    collection.Goods = await _homeRepository.GetNewGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.Cheapest:
                                    collection.Goods = await _homeRepository.GetCheapestGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostLike:
                                    collection.Goods = await _homeRepository.GetMostLikesGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.CustomerLike:

                                    collection.Goods = await _homeRepository.GetCustomerLikeGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);

                                    break;
                                case (int)CollectionTypeEnum.Expensive:
                                    collection.Goods = await _homeRepository.GetMostExpensiveGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostDiscount:
                                    collection.Goods = await _homeRepository.GetMostDiscountGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostSeller:
                                    collection.Goods = await _homeRepository.GetMostSellGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostView:
                                    collection.Goods = await _homeRepository.GetMostViewGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.SpecialOffer:

                                    collection.Goods = await _homeRepository.GetSpecialOfferGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);

                                    break;
                                case (int)CollectionTypeEnum.LastView:

                                    collection.Goods = await _homeRepository.GetLastViewGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo, ipAddress);

                                    break;
                                case (int)CollectionTypeEnum.SpecialGoods:
                                    collection.Goods = await _homeRepository.GetSpecialGoodsHome(xitemIds, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.SpecialSale:
                                    collection.Goods = await _homeRepository.GetSpecialSaleGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.AllProduct:
                                    collection.Goods = await _homeRepository.GetAllGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                            }
                            if (collection.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                            {
                                collection.CriteriaFrom = collection.CriteriaFrom / rate;
                                collection.CriteriaTo = collection.CriteriaTo / rate;
                            }
                            foreach (var item in GoodsIdsInOrder)
                            {
                                foreach (var item2 in collection.Goods)
                                {
                                    if (item == item2.GoodsId)
                                    {
                                        item2.InCart = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new ApiResponse<HomeGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HeaderGetDto>> HeaderGet(bool? isDesktop)
        {
            var data = new HeaderGetDto();
            if (token.Id == 0)
            {
                token.Id = (int)CustomerTypeEnum.Unknown;
                data.WishListCount = 0;
                data.CustomerFullName = "";
            }
            else
            {
                data.WishListCount = await _userActivityRepository.GetCustomerWishListCount(token.Id);
                var customerDetial = await _customerRepository.GetCustomerGeneralDetail(token.Id);
                data.CustomerFullName = customerDetial.Name + " " + customerDetial.Family;
                token.CookieId = Guid.Empty;
            }
            data.CartCount = await _userActivityRepository.GetCustomerCartCount(token.Id, token.CookieId);
            if (isDesktop == true)
            {
                data.Categories = await _categoryRepository.GetCategoryAndBrandForWebsite();
            }
            else
            {
                data.Categories = new List<CategoryWebGetDto>();
            }
            var setting = await _settingRepository.WebsiteSettingGet();
            data.LogoUrlShopHeader = setting.LogoUrlShopHeader;
            data.MetaDescription = setting.MetaDescription;
            data.MetaTitle = setting.MetaTitle;
            data.PageTitle = setting.PageTitle;
            data.MetaKeywords = setting.MetaKeyword;
            data.LiveChatStatus = setting.LiveChatStatus;
            for (var i = 0; i < data.Categories.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(data.Categories[i].MetaKeywords))
                {
                    data.MetaKeywords = data.MetaKeywords + " , " + data.Categories[i].MetaKeywords;
                }

                for (int j = 0; j < data.Categories[i].Childs.Count; j++)
                {

                    if (!string.IsNullOrWhiteSpace(data.Categories[i].Childs[j].MetaKeywords))
                    {
                        data.MetaKeywords = data.MetaKeywords + " , " + data.Categories[i].Childs[j].MetaKeywords;
                    }
                }

            }

            return new ApiResponse<HeaderGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<List<CategoryWebGetDto>>> MobileCategory()
        {

            var Categories = await _categoryRepository.GetCategoryForMobile();

            return new ApiResponse<List<CategoryWebGetDto>>(ResponseStatusEnum.Success, Categories, _ms.MessageService(Message.Successfull));

        }


        public async Task<ApiResponse<FooterGetDto>> FooterGet(bool getFooterCategory)
        {
            var data = new FooterGetDto();
            var WebModuleSetting = await _settingRepository.WebsiteSettingGet();
            if (getFooterCategory)
            {
                data.Footer = await _categoryRepository.GetFooterForWebsite(WebModuleSetting.FooterMaxItem, WebModuleSetting.FooterMaxItemPerColumn);
            }
            data.Links = WebModuleSetting;
            return new ApiResponse<FooterGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<FilterGoodsGetDto>> FilterGoodsGet(WebsiteFilterDto filterDto)
        {
            if (filterDto.BrandId == null)
            {
                filterDto.BrandId = new List<int>();
            }
            if (filterDto.OptionIds == null)
            {
                filterDto.OptionIds = new List<int>();
            }
            filterDto.ShopId = 0;
            var FilterGoodsGet = new FilterGoodsGetDto();
            var catIds = new List<int>();
            var goodsIds = new List<int>();
            var catId = 0;
            var allow = true;

            var rate = (decimal)await _settingRepository.GetCurrencyRate();
            if (filterDto.FromPrice != 0)
            {
                filterDto.FromPrice = filterDto.FromPrice / rate;
            }
            if (filterDto.ToPrice != 0)
            {
                filterDto.ToPrice = filterDto.ToPrice / rate;
            }

            if (filterDto.Type == (int)WebsiteFilterTypeEnum.Slider)  //slider
            {
                var slider = await _webSliderRepository.SliderGetById(filterDto.Id);
                FilterGoodsGet.CriteriaType = slider.CriteriaType;
                FilterGoodsGet.CriteriaFrom = slider.CriteriaFrom;
                FilterGoodsGet.CriteriaTo = slider.CriteriaTo;
                if (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                {
                    slider.CriteriaFrom = slider.CriteriaFrom / rate;
                    slider.CriteriaTo = slider.CriteriaTo / rate;
                }
                var ids = new List<int>();
                if (!string.IsNullOrWhiteSpace(slider.XitemIds))
                {
                    var baseIds = slider.XitemIds.Split(',');
                    for (int i = 0; i < baseIds.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(baseIds[i]))
                        {
                            ids.Add(Int32.Parse(baseIds[i]));
                        }
                    }
                }
                if (ids.Count() > 0)
                {
                    if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialGoods)
                    {
                        goodsIds = ids;
                    }
                    else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                    {
                        var plan = await _discountRepository.GetGoodsAndCatIdsByPlanId(ids[0]);
                        if (plan != null)
                        {
                            if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                            {

                            }
                            else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                            {
                                allow = plan.TDiscountCategory.Any(x => x.Allowed == true);
                                catIds = plan.TDiscountCategory.Select(x => x.FkCategoryId).ToList();
                            }
                            else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                            {
                                allow = plan.TDiscountGoods.Any(x => x.Allowed == true);
                                goodsIds = plan.TDiscountGoods.Select(x => x.FkGoodsId).ToList();
                            }
                            filterDto.ShopId = plan.FkShopId == null ? 0 : (int)plan.FkShopId;
                        }
                    }
                    else
                    {
                        catIds = await _categoryRepository.GetCategoriesChildsTrueStatus(ids[0]);
                        catId = ids[0];
                    }
                }
                int indexCollectionLastDay = -await _settingRepository.GetSettingIndexCollectionLastDay();
                var fromDay = DateTime.Now.AddDays(indexCollectionLastDay);
                var toDay = DateTime.Now;
                var goods = await _homeRepository.FilterGoodsGetType1(filterDto, slider, catIds, goodsIds, fromDay, toDay, rate, allow);
                var count = await _homeRepository.FilterGoodsGetType1Count(filterDto, slider, catIds, goodsIds, fromDay, toDay, allow);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType1(filterDto, slider, catIds, goodsIds, fromDay, toDay, rate, allow);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }
            else if (filterDto.Type == (int)WebsiteFilterTypeEnum.Category) // category
            {
                catId = filterDto.Id;
                var getCategoryDetails = await _categoryRepository.CategoryGetForEdit(filterDto.Id);
                if (getCategoryDetails != null)
                {
                    if (getCategoryDetails.ToBeDisplayed != true)
                    {
                        return new ApiResponse<FilterGoodsGetDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.CategoryNotFoundById));
                    }
                }
                catIds = await _categoryRepository.GetCategoriesChildsTrueStatus(filterDto.Id);
                var goods = await _homeRepository.FilterGoodsGetType2(filterDto, catIds, rate);
                var count = await _homeRepository.FilterGoodsGetType2Count(filterDto, catIds);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType2(filterDto, catIds, rate);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }
            else if (filterDto.Type == (int)WebsiteFilterTypeEnum.Module) // module
            {
                var module = await _webModuleRepository.WebModuleCollectionsGetById(filterDto.Id);
                FilterGoodsGet.CriteriaType = module.CriteriaType;
                FilterGoodsGet.CriteriaFrom = module.CriteriaFrom;
                FilterGoodsGet.CriteriaTo = module.CriteriaTo;
                if (module.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                {
                    module.CriteriaFrom = module.CriteriaFrom / rate;
                    module.CriteriaTo = module.CriteriaTo / rate;
                }
                var ids = new List<int>();
                if (!string.IsNullOrWhiteSpace(module.XitemIds))
                {
                    var baseIds = module.XitemIds.Split(',');
                    for (int i = 0; i < baseIds.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(baseIds[i]))
                        {
                            ids.Add(Int32.Parse(baseIds[i]));
                        }
                    }
                }
                if (ids.Count() > 0)
                {
                    if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialGoods)
                    {
                        goodsIds = ids;
                    }
                    else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                    {
                        var plan = await _discountRepository.GetGoodsAndCatIdsByPlanId(ids[0]);
                        if (plan != null)
                        {
                            if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                            {

                            }
                            else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                            {
                                allow = plan.TDiscountCategory.Any(x => x.Allowed == true);
                                catIds = plan.TDiscountCategory.Select(x => x.FkCategoryId).ToList();
                            }
                            else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                            {
                                allow = plan.TDiscountGoods.Any(x => x.Allowed == true);
                                goodsIds = plan.TDiscountGoods.Select(x => x.FkGoodsId).ToList();
                            }
                            filterDto.ShopId = plan.FkShopId == null ? 0 : (int)plan.FkShopId;
                        }
                    }
                    else
                    {
                        catIds = await _categoryRepository.GetCategoriesChildsTrueStatus(ids[0]);
                        catId = ids[0];
                    }
                }
                int indexCollectionLastDay = -await _settingRepository.GetSettingIndexCollectionLastDay();
                var fromDay = DateTime.Now.AddDays(indexCollectionLastDay);
                var toDay = DateTime.Now;
                var goods = await _homeRepository.FilterGoodsGetType3(filterDto, module, catIds, goodsIds, fromDay, toDay, rate, allow);
                var count = await _homeRepository.FilterGoodsGetType3Count(filterDto, module, catIds, goodsIds, fromDay, toDay, allow);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType3(filterDto, module, catIds, goodsIds, fromDay, toDay, rate, allow);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }

            else if (filterDto.Type == (int)WebsiteFilterTypeEnum.PupUp) // module
            {
                var pupup = await _PupupRepository.GetPupupItemById(filterDto.Id);
                if (pupup.FkTDiscountPlanId != null)
                {
                    var plan = await _discountRepository.GetGoodsAndCatIdsByPlanId((int)pupup.FkTDiscountPlanId);
                    if (plan != null)
                    {
                        if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                        {

                        }
                        else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                        {
                            allow = plan.TDiscountCategory.Any(x => x.Allowed == true);
                            catIds = plan.TDiscountCategory.Select(x => x.FkCategoryId).ToList();
                        }
                        else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                        {
                            allow = plan.TDiscountGoods.Any(x => x.Allowed == true);
                            goodsIds = plan.TDiscountGoods.Select(x => x.FkGoodsId).ToList();
                        }
                        filterDto.ShopId = plan.FkShopId == null ? 0 : (int)plan.FkShopId;
                    }
                }
                else
                {
                    catIds = await _categoryRepository.GetCategoriesChildsTrueStatus((int)pupup.FkCategoryId);
                    catId = (int)pupup.FkCategoryId;
                }
                var module = new WebModuleCollectionsGetDto();
                if (pupup.JustNewGoods)
                {
                    module.FkCollectionTypeId = (int)CollectionTypeEnum.New;
                }
                else if (pupup.FkTDiscountPlanId != null)
                {
                    module.FkCollectionTypeId = (int)CollectionTypeEnum.SpecialSale;
                }
                else
                {
                    module.FkCollectionTypeId = (int)CollectionTypeEnum.AllProduct;
                }

                int indexCollectionLastDay = -await _settingRepository.GetSettingIndexCollectionLastDay();
                var fromDay = DateTime.Now.AddDays(indexCollectionLastDay);
                var toDay = DateTime.Now;
                var goods = await _homeRepository.FilterGoodsGetType3(filterDto, module, catIds, goodsIds, fromDay, toDay, rate, allow);
                var count = await _homeRepository.FilterGoodsGetType3Count(filterDto, module, catIds, goodsIds, fromDay, toDay, allow);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType3(filterDto, module, catIds, goodsIds, fromDay, toDay, rate, allow);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }


            else if (filterDto.Type == (int)WebsiteFilterTypeEnum.Search) // search
            {
                catId = filterDto.Id;
                var getCategoryDetails = await _categoryRepository.CategoryGetForEdit(filterDto.Id);
                if (getCategoryDetails != null)
                {
                    if (getCategoryDetails.ToBeDisplayed != true)
                    {
                        return new ApiResponse<FilterGoodsGetDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.CategoryNotFoundById));
                    }
                }
                catIds = await _categoryRepository.GetCategoriesChildsTrueStatus(filterDto.Id);
                var goods = await _homeRepository.FilterGoodsGetType4(filterDto, rate, catIds);
                var count = await _homeRepository.FilterGoodsGetType4Count(filterDto, catIds);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType4(filterDto, rate, catIds);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }
            else if (filterDto.Type == (int)WebsiteFilterTypeEnum.Deals) // Deals
            {
                var goods = await _homeRepository.FilterGoodsGetType5(filterDto, rate);
                var count = await _homeRepository.FilterGoodsGetType5Count(filterDto);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType5(filterDto, rate);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }

            //get brand
            // if (filterDto.GetBrand && catIds.Count> 0)
            // {
            //     FilterGoodsGet.Brands = await _brandRepository.GetBrandForWebsite(catIds);
            // }
            //get spec
            if (filterDto.GetSpecs && catId != 0)
            {
                var categoryIds = await _categoryRepository.GetParentCatIds(catId);
                FilterGoodsGet.Specs = await _specificationRepository.GetSpecsForWebsite(categoryIds);
            }
            // get all count
            if (filterDto.GetAllCount)
            {
                FilterGoodsGet.AllGoodsCount = await _homeRepository.GetAllGoodsCountForSelling(0);
            }

            // get child category
            if (filterDto.GetChild == true && catId != 0)
            {
                FilterGoodsGet.ChildCategory = await _categoryRepository.GetChildCategoryForWebsite(catId);
            }
            // get parent category
            if (filterDto.GetParent == true && catId != 0)
            {
                var categoryPath = "";
                var parentCategoryIds = await _categoryRepository.GetParentCatIds(catId);
                for (int i = 0; i < parentCategoryIds.Count; i++)
                {
                    if (i != parentCategoryIds.Count)
                    {
                        categoryPath = categoryPath + parentCategoryIds[i] + ",";
                    }
                }
                var data = await _categoryService.GetParentCategoryForWebsite(catId, categoryPath);
                FilterGoodsGet.ParentCategory = data.Result;
            }
            var GoodsIdsInOrder = new List<int>();
            if (token.Rule == UserGroupEnum.Customer)
            {
                if (token.CookieId != Guid.Empty || token.Id != 0)
                {
                    GoodsIdsInOrder = await _userOrderRepository.GetGoodsIdsInOrder(token.Id, token.CookieId);
                }
            }
            foreach (var item in GoodsIdsInOrder)
            {
                foreach (var item2 in FilterGoodsGet.Goods.Data)
                {
                    if (item == item2.GoodsId)
                    {
                        item2.InCart = true;
                    }
                }
            }
            return new ApiResponse<FilterGoodsGetDto>(ResponseStatusEnum.Success, FilterGoodsGet, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<FilterShopGoodsGetDto>> FilterShopGoodsGet(string shop, WebsiteFilterDto filterDto)
        {
            if (filterDto.BrandId == null)
            {
                filterDto.BrandId = new List<int>();
            }
            if (filterDto.OptionIds == null)
            {
                filterDto.OptionIds = new List<int>();
            }
            var FilterGoodsGet = new FilterShopGoodsGetDto();
            FilterGoodsGet.ChildCategory = new List<CategoryTreeViewFilterDto>();
            var catIds = new List<int>();
            var goodsIds = new List<int>();
            var catId = 0;

            FilterGoodsGet.Shop = await _shopRepository.GetShopByUrl(shop);
            var WebModuleSetting = await _settingRepository.WebsiteSettingGet();
            FilterGoodsGet.DescriptionCalculateShopRate = WebModuleSetting.DescriptionCalculateShopRatePrice;
            if (FilterGoodsGet.Shop == null)
            {
                return new ApiResponse<FilterShopGoodsGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            if (FilterGoodsGet.Shop.ShopStatus != (int)ShopStatusEnum.Active)
            {
                return new ApiResponse<FilterShopGoodsGetDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.ShopGetting));
            }
            filterDto.ShopId = FilterGoodsGet.Shop.ShopId;


            var rate = (decimal)await _settingRepository.GetCurrencyRate();
            if (filterDto.FromPrice != 0)
            {
                filterDto.FromPrice = filterDto.FromPrice / rate;
            }
            if (filterDto.ToPrice != 0)
            {
                filterDto.ToPrice = filterDto.ToPrice / rate;
            }

            if (filterDto.Type == (int)WebsiteFilterTypeEnum.Category) // category
            {
                if (filterDto.Id != 0)
                {
                    catId = filterDto.Id;
                    catIds.AddRange(await _categoryRepository.GetCategoriesChildsTrueStatus(catId));
                }
                else
                {
                    catId = 0;

                    foreach (var item in FilterGoodsGet.Shop.CategoryIds)
                    {
                        catIds.AddRange(await _categoryRepository.GetCategoriesChildsTrueStatus(item));
                    }
                }
                var goods = await _homeRepository.FilterGoodsGetType2(filterDto, catIds, rate);
                var count = await _homeRepository.FilterGoodsGetType2Count(filterDto, catIds);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType2(filterDto, catIds, rate);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }
            else
            {
                var ids = new List<int>();
                var goods = await _homeRepository.FilterGoodsGetType4(filterDto, rate, ids);
                var count = await _homeRepository.FilterGoodsGetType4Count(filterDto, ids);
                //get max price 
                if (filterDto.GetMaxPrice == true)
                {
                    FilterGoodsGet.MaxPrice = await _homeRepository.MaxPriceGetType4(filterDto, rate, ids);
                }
                var goodsPage = new Pagination<GoodsHomeDto>(count, goods);
                FilterGoodsGet.Goods = goodsPage;
            }

            //get brand
            if (filterDto.GetBrand && catIds.Count > 0)
            {
                FilterGoodsGet.Brands = await _brandRepository.GetBrandForWebsite(catIds);
            }
            //get spec
            if (filterDto.GetSpecs && catIds.Count > 0)
            {
                if (catId == 0)
                {
                    FilterGoodsGet.Specs = await _specificationRepository.GetSpecsForWebsite(FilterGoodsGet.Shop.CategoryIds);
                }
                else
                {
                    var categoryIds = await _categoryRepository.GetParentCatIds(catId);
                    FilterGoodsGet.Specs = await _specificationRepository.GetSpecsForWebsite(categoryIds);
                }
            }
            // get all count
            if (filterDto.GetAllCount)
            {
                FilterGoodsGet.AllGoodsCount = await _homeRepository.GetAllGoodsCountForSelling(filterDto.ShopId);
            }

            // get child category
            if (filterDto.GetChild == true && FilterGoodsGet.Shop.CategoryIds.Count > 0)
            {
                //   if (catId == 0)
                //   {
                foreach (var item in FilterGoodsGet.Shop.CategoryIds)
                {
                    var data = await _categoryRepository.GetChildCategoryForWebsite(item);
                    if (data != null)
                    {
                        FilterGoodsGet.ChildCategory.Add(data);
                    }
                }
                //   }
                // else
                // {
                //     FilterGoodsGet.ChildCategory.Add(await _categoryRepository.GetChildCategoryForWebsite(catId));
                // }
            }
            // get parent category
            if (filterDto.GetParent == true && catId != 0)
            {
                var categoryPath = "";
                var parentCategoryIds = await _categoryRepository.GetParentCatIds(catId);
                for (int i = 0; i < parentCategoryIds.Count; i++)
                {
                    if (i != parentCategoryIds.Count - 1)
                    {
                        categoryPath = categoryPath + parentCategoryIds[i] + ",";
                    }
                }
                var data = await _categoryService.GetParentCategoryForWebsite(catId, categoryPath);
                FilterGoodsGet.ParentCategory = data.Result;
            }
            var GoodsIdsInOrder = new List<int>();
            if (token.Rule == UserGroupEnum.Customer)
            {
                if (token.CookieId != Guid.Empty || token.Id != 0)
                {
                    GoodsIdsInOrder = await _userOrderRepository.GetGoodsIdsInOrder(token.Id, token.CookieId);
                }
            }
            foreach (var item in GoodsIdsInOrder)
            {
                foreach (var item2 in FilterGoodsGet.Goods.Data)
                {
                    if (item == item2.GoodsId)
                    {
                        item2.InCart = true;
                    }
                }
            }

            return new ApiResponse<FilterShopGoodsGetDto>(ResponseStatusEnum.Success, FilterGoodsGet, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<GoodsDetailesDto>> GoodsDetailsGet(int goodsId, int providerId)
        {
            var catIds = new List<int>();
            var data = await _homeRepository.GoodsDetailsGet(goodsId, providerId);
            if (data == null)
            {
                return new ApiResponse<GoodsDetailesDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.GoodsGetting));
            }
            catIds = await _categoryRepository.GetCategoriesChildsTrueStatus(data.FkCategoryId);
            data.Recommendation = await _recommendationRepository.GetRecommendationGoods(catIds, goodsId);
            var categoryPath = "";
            var parentCategoryIds = await _categoryRepository.GetParentCatIds(data.FkCategoryId);
            for (int i = 0; i < parentCategoryIds.Count; i++)
            {
                if (i != parentCategoryIds.Count)
                {
                    categoryPath = categoryPath + parentCategoryIds[i] + ",";
                }
            }
            var dataMap = await _categoryService.GetParentCategoryForWebsite(data.FkCategoryId, categoryPath);
            data.ParentCategory = dataMap.Result;
            return new ApiResponse<GoodsDetailesDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecificationGroupGetForGoodsDto>>> GoodsSpecifictionGet(int goodsId)
        {
            var result = await _homeRepository.GoodsSpecifictionGet(goodsId);
            if (result == null)
            {
                return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            return new ApiResponse<List<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<GoodsCommentWithSurveyDto>> CustomerCommentGet(PaginationFormDto paginatin)
        {
            var data = await _goodsCommentRepository.CustomerCommentGet(paginatin);
            if (data == null)
            {
                return new ApiResponse<GoodsCommentWithSurveyDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CommentGoods));
            }
            data.GoodsCommentCount = await _goodsCommentRepository.CustomerCommentGetCount(paginatin);
            return new ApiResponse<GoodsCommentWithSurveyDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<PostMethodDto>> PostMethod(int shopId, int? cityId, int countryId, int? provinceId)
        {
            var data = await _settingRepository.PostMethod(shopId, cityId, countryId, provinceId);
            if (data == null)
            {
                return new ApiResponse<PostMethodDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.PostMethodGetting));
            }
            return new ApiResponse<PostMethodDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HomeCategoryGetDto>> HomeCategoryGet(int categoryId, string ipAddress)
        {
            var data = new HomeCategoryGetDto();
            var rate = (decimal)await _settingRepository.GetCurrencyRate();

            var WebModuleSetting = await _settingRepository.WebsiteSettingGet();
            data.Slider = await _webSliderRepository.SliderGet(1, rate, categoryId);
            data.WebHomeModuleList = await _webModuleRepository.GetModuleCollection(1, rate, categoryId); // get type 1 for website
            data.Category = await _categoryRepository.GetCategoryAndBrandForCategoryPageWebsite(categoryId);



            int indexCollectionLastDay = -WebModuleSetting.IndexCollectionLastDay;
            var fromDay = DateTime.Now.AddDays(indexCollectionLastDay);
            var toDay = DateTime.Now;

            var GoodsIdsInOrder = new List<int>();
            if (token.Rule == UserGroupEnum.Customer)
            {
                if (token.CookieId != Guid.Empty || token.Id != 0)
                {
                    GoodsIdsInOrder = await _userOrderRepository.GetGoodsIdsInOrder(token.Id, token.CookieId);
                }
            }

            foreach (var module in data.WebHomeModuleList)
            {
                if (module.FkModuleId == (int)ModuleTypeEnum.ProductList)
                {
                    foreach (var collection in module.WebModuleCollections)
                    {
                        var xitemIds = new List<int>();
                        if (!string.IsNullOrWhiteSpace(collection.XitemIds))
                        {
                            var baseIds = collection.XitemIds.Split(',');
                            for (int i = 0; i < baseIds.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(baseIds[i]))
                                {
                                    xitemIds.Add(Int32.Parse(baseIds[i]));
                                }
                            }
                        }
                        if (collection.HaveLink == false)
                        {
                            if (collection.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                            {
                                collection.CriteriaFrom = collection.CriteriaFrom / rate;
                                collection.CriteriaTo = collection.CriteriaTo / rate;
                            }
                            switch (collection.FkCollectionTypeId)
                            {
                                case (int)CollectionTypeEnum.New:
                                    collection.Goods = await _homeRepository.GetNewGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.Cheapest:
                                    collection.Goods = await _homeRepository.GetCheapestGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostLike:
                                    collection.Goods = await _homeRepository.GetMostLikesGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.CustomerLike:
                                    if (token.Id != 0)
                                    {
                                        collection.Goods = await _homeRepository.GetCustomerLikeGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    }
                                    break;
                                case (int)CollectionTypeEnum.Expensive:
                                    collection.Goods = await _homeRepository.GetMostExpensiveGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostDiscount:
                                    collection.Goods = await _homeRepository.GetMostDiscountGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostSeller:
                                    collection.Goods = await _homeRepository.GetMostSellGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.MostView:
                                    collection.Goods = await _homeRepository.GetMostViewGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, fromDay, toDay, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.SpecialOffer:
                                    if (token.Id != 0)
                                    {
                                        collection.Goods = await _homeRepository.GetSpecialOfferGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    }
                                    break;
                                case (int)CollectionTypeEnum.LastView:
                                    if (token.Id != 0)
                                    {
                                        collection.Goods = await _homeRepository.GetLastViewGoodsHome(token.Id, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo, ipAddress);
                                    }
                                    break;
                                case (int)CollectionTypeEnum.SpecialGoods:
                                    collection.Goods = await _homeRepository.GetSpecialGoodsHome(xitemIds, WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.SpecialSale:
                                    collection.Goods = await _homeRepository.GetSpecialSaleGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                                case (int)CollectionTypeEnum.AllProduct:
                                    collection.Goods = await _homeRepository.GetAllGoodsHome(xitemIds.FirstOrDefault(), WebModuleSetting.IndexCollectionCount, rate, collection.CriteriaType, collection.CriteriaFrom, collection.CriteriaTo);
                                    break;
                            }
                            if (collection.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                            {
                                collection.CriteriaFrom = collection.CriteriaFrom / rate;
                                collection.CriteriaTo = collection.CriteriaTo / rate;
                            }
                            foreach (var item in GoodsIdsInOrder)
                            {
                                foreach (var item2 in collection.Goods)
                                {
                                    if (item == item2.GoodsId)
                                    {
                                        item2.InCart = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return new ApiResponse<HomeCategoryGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GoodsHomeDto>>> GetCustomerLikes()
        {
            var data = await _homeRepository.GetCustomerLike(token.Id);
            if (data == null)
            {
                return new ApiResponse<List<GoodsHomeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            return new ApiResponse<List<GoodsHomeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HomeSearchAutoComplete>> GetSearchAutoComplete(string search)
        {
            var data = await _homeRepository.GetSearchAutoComplete(search);
            return new ApiResponse<HomeSearchAutoComplete>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<HelpArticleFormDto>>> HelpAutoComplete(string search)
        {
            var data = await _helpRepository.HelpAutoComplete(search);
            if (data == null)
            {
                return new ApiResponse<List<HelpArticleFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpArticleGetting));
            }
            return new ApiResponse<List<HelpArticleFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<HomeHelpTopicListDto>>> GetHomeHelpTopic()
        {
            var data = await _helpRepository.GetHomeHelpTopic();
            if (data == null)
            {
                return new ApiResponse<List<HomeHelpTopicListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpTopicGetting));
            }
            return new ApiResponse<List<HomeHelpTopicListDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HomeHelpTopicChildDto>> GetHelpTopic(int topicId)
        {
            var data = await _helpRepository.GetHelpTopic(topicId);
            if (data == null)
            {
                return new ApiResponse<HomeHelpTopicChildDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpTopicGetting));
            }
            return new ApiResponse<HomeHelpTopicChildDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<HomeHelpTopicDto>> GetHelpParentTopic(int topicId)
        {
            var data = await _helpRepository.GetHelpParentTopic(topicId);
            if (data == null)
            {
                return new ApiResponse<HomeHelpTopicDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpTopicGetting));
            }
            return new ApiResponse<HomeHelpTopicDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<HomeHelpArticleDto>> GetHelpArticle(int articleId)
        {
            var data = await _helpRepository.GetHelpArticle(articleId);
            if (data == null)
            {
                return new ApiResponse<HomeHelpArticleDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpArticleGetting));
            }
            return new ApiResponse<HomeHelpArticleDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<string>> GetHelpImage()
        {
            var result = await _helpRepository.GetHelpImage();
            return new ApiResponse<string>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<ProfileOrderGetDto>> GetOrderWithCode(string trackingCode)
        {

            var result = await _userOrderRepository.GetProfileOrderItem(0, trackingCode);
            if (result == null)
            {
                return new ApiResponse<ProfileOrderGetDto>(ResponseStatusEnum.NotFound, null, _ms.MessageService(Message.OrderNotFoundWithCode));
            }
            else
            {
                return new ApiResponse<ProfileOrderGetDto>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> AddHelpFul(AcceptDto accept)
        {
            var data = await _helpRepository.AddHelpFul(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.HelpArticleEditing));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<MobileSplashDataDto>> GetMobileSplashData(MobileSplashDataParams model)
        {
            var data = await _homeRepository.GetMobileSplashData();
            var forceUpdateData = await _forceUpdateRepository.GetForceUpdateAsync();
            data.ForceUpdateObj = forceUpdateData;

            data.IsForceUpdate = false;

            if (model.PlatformOS == "android" && forceUpdateData.ForceUpdateAndroid == true)
            {
                if (Int32.Parse(model.BuildNumber) < forceUpdateData.AndroidVersionCode)
                {
                    data.IsForceUpdate = true;
                }
            }

            if (model.PlatformOS == "ios" && forceUpdateData.ForceUpdateIos == true)
            {
                if (Int32.Parse(model.BuildNumber) < forceUpdateData.IosBuildNumber)
                {
                    data.IsForceUpdate = true;
                }
            }

            if (data == null)
            {
                return new ApiResponse<MobileSplashDataDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<MobileSplashDataDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<DefualtLanguageCurrencyDto>> GetDefualtLanguageAndCurrency()
        {
            var data = await _homeRepository.GetDefualtLanguageAndCurrency();
            if (data == null)
            {
                return new ApiResponse<DefualtLanguageCurrencyDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<DefualtLanguageCurrencyDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<string>> GetFooterContent(string content)
        {
            var data = await _homeRepository.GetFooterContent(content);
            return new ApiResponse<string>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<string>> GetMobileDescriptionPageData(MobileDescriptionPageParams model)
        {
            var data = await _homeRepository.GetMobileDescriptionPageData(model);

            return new ApiResponse<string>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}