using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IGoodsRepository
    {
        Task<TGoods> GoodsAdd(TGoods goods);
        Task<RepRes<bool>> GoodsEdit(TGoods goods , bool? goodsProviderIsAccepted , int goodsProviderId );
        Task<RepRes<TGoods>> GoodsDelete(int goodsId);
        Task<bool> GoodsShow(int goodsId);
        Task<bool> ChangeAccept(List<AcceptNullDto> accept);
        Task<bool> ChangeProviderToBeDisplay(List<AcceptNullDto> accept);
        Task<GoodsBaseDetailDto> GetGoodsBaseDetailDto(int goodsId);
        Task<bool> GoodsExist(int goodsId);
        Task<RepRes<bool>> CanShopAddGoods(int shopId,int categoryId);
        Task<bool> GoodsDocumentExist(int imageId);
        Task<string> GetGoodsDescription(int goodsId);
        Task<GoodsDto> GetGoodsById(int goodsId);
        Task<bool> CanChangeGoodsShow(List<int> catIds);
        Task<GoodsDescriptionDto> EditDescription(GoodsDescriptionDto goods);
        Task<List<int>> GetGoodsParentCategoryIds(int goodsId);
        Task<TGoodsDocument> UploadGoodsDocument(TGoodsDocument GoodsDocument);
        Task<List<TGoodsDocument>> GetGoodsDocumentById(int goodsId);
        Task<RepRes<bool>> AddGoodsProvider(TGoodsProvider goodsProvider,List<int> parameterIds,List<string> fileNames);
        Task<List<GoodsProviderGetDto>> GetGoodsProvider(int goodsId, int shopId);
        Task<RepRes<bool>> DeleteGoodsProvider(int goodsId,int goodsProviderId , int shopId);
        Task<RepRes<TGoodsDocument>> DeleteImageById(int imageId);
        Task<List<GoodsBaseDetailDto>> GetGoodsByCategoryId(List<int> CategoryId, string Filter);
        Task<List<GoodsListGetDto>> GetAllGoodsByCategoryId(GoodsPaginationDto pagination);
        Task<int> GetAllGoodsByCategoryIdCount(GoodsPaginationDto pagination);
        Task<bool> DeleteGoodsSpecificationByGoodsId(int goodsId);
        Task<List<TGoodsSpecification>> AddGoodsSpecification(List<TGoodsSpecification> goodsSpecs);
        Task<bool> ShopHasThisGoods(int shopId, int goodsId,bool forShowing);
        Task<List<GoodsBaseDetailDto>> GoodsGetByIds(List<int> ids);
        Task<NoVariationGoodsProviderGetDto> GetNoVariationGoodsProvider(int goodsId, int shopId);
        Task<bool> AcceptShopGoodsAdding();
        Task<GoodsMetaDto> EditGoodsMeta(GoodsMetaDto goodsMeta);
        Task<GoodsMetaDto> GetGoodsMeta(int goodsId);
        Task<bool> GetGoodsIncludeVat(int shopId);
        Task<bool> GoodsGroupEditing(GoodsGroupEditingDto goodsGroupEditing , int shopId);
    }
}