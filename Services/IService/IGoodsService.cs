using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IGoodsService
    {
        Task<ApiResponse<GoodsBaseDetailDto>> GoodsAdd(GoodsSerializeDto goodsDto);
        Task<ApiResponse<GoodsBaseDetailDto>> GoodsEdit(GoodsSerializeDto goodsDto);
        Task<ApiResponse<GoodsFormDto>> GetGoodsBaseDataById(int goodsId);
        Task<ApiResponse<GoodsBaseDetailDto>> GoodsBaseDataDetail(int goodsId);
        Task<ApiResponse<GoodsDescriptionDto>> EditDescription(GoodsDescriptionDto goods);
        Task<ApiResponse<GoodsDescriptionDto>> GetGoodsDescription(int goodsId);
        Task<ApiResponse<List<VariationParameterGetDto>>> GetVarityParameter(int goodsId);
        Task<ApiResponse<bool>> GoodsExist(int goodsId);
        Task<ApiResponse<bool>> GoodsShow(int goodsId);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto> accept);
        Task<ApiResponse<GoodsDocumentDto>> UploadGoodsDocument(GoodsDocumentAddDto GoodsDocument);
        Task<ApiResponse<List<GoodsDocumentDto>>> GetGoodsDocumentById(int goodsId,int? varityId);
        Task<ApiResponse<bool>> DeleteImageById(int imageId,int goodsId);
        Task<ApiResponse<bool>> GoodsDelete(int goodsId);
        Task<ApiResponse<bool>> CanChangeGoodsShow(int goodsId);
        Task<ApiResponse<List<GoodsSpecificationDto>>> AddGoodsSpecification(List<GoodsSpecificationDto> goodsSpec);
        Task<ApiResponse<List<GoodsBaseDetailDto>>> GetGoodsByCategoryId(int CategoryId, string Filter);
        Task<ApiResponse<Pagination<GoodsListGetDto>>> GetAllGoodsByCategoryId(GoodsPaginationDto pagination);
        Task<ApiResponse<List<SpecificationGroupGetForGoodsDto>>> GetGoodsSpecification(int goodsId);
        Task<ApiResponse<bool>> AddGoodsProvider(GoodsProviderSerializeDto goodsProviderDto);
        Task<ApiResponse<List<GoodsProviderGetDto>>> GetGoodsProvider(int goodsId,int shopId);
        Task<ApiResponse<bool>> DeleteGoodsProvider(int goodsId,int goodsProviderId);
        Task<ApiResponse<NoVariationGoodsProviderGetDto>> GetNoVariationGoodsProvider(int goodsId,int shopId);
        // // List<ApiResponse<GoodsHomeDto>> ConvetDataTable(DataTable dt);
        // Task<ApiResponse<List<GoodsBaseDetailDto>>> SearchGoodsForAutoComplate(string Filter);
        // Task<ApiResponse<Pagination<GoodsFormPriceDto>>> GetGoodsOrder(GoodsFiltersDto pagination,TokenParseDto token);
        Task<ApiResponse<GoodsMetaDto>> EditGoodsMetaService(GoodsMetaDto goodsmeta);
        Task<ApiResponse<GoodsMetaDto>> GetGoodsMeta(int goodsId);
        Task<ApiResponse<GoodsDto>> GetGoodsById(int goodsId);
        Task<ApiResponse<bool>> GetGoodsIncludeVat(int shopId);
        Task<ApiResponse<bool>> GoodsGroupEditing(GoodsGroupEditingDto goodsGroupEditing);
    }
}