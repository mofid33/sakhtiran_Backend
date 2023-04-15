using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Goods;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        public IGoodsService _goodsService { get; }
        public GoodsController(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsBaseDetailDto>))]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] GoodsSerializeDto goodsDto)
        {
            var result = await _goodsService.GoodsAdd(goodsDto);
            return new Response<GoodsBaseDetailDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsBaseDetailDto>))]
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] GoodsSerializeDto goodsDto)
        {
            var result = await _goodsService.GoodsEdit(goodsDto);
            return new Response<GoodsBaseDetailDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsDto>))]
        [HttpGet("{goodsId}")]
        public async Task<IActionResult> GetGoodsById([FromRoute] int goodsId)
        {
            var result = await _goodsService.GetGoodsById(goodsId);
            return new Response<GoodsDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsFormDto>))]
        [HttpGet("BaseData/{goodsId}")]
        public async Task<IActionResult> GetGoodsBaseDataById([FromRoute] int goodsId)
        {
            var result = await _goodsService.GetGoodsBaseDataById(goodsId);
            return new Response<GoodsFormDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsBaseDetailDto>))]
        [HttpGet("BaseDataDetail/{goodsId}")]
        public async Task<IActionResult> GoodsBaseDataDetail([FromRoute] int goodsId)
        {
            var result = await _goodsService.GoodsBaseDataDetail(goodsId);
            return new Response<GoodsBaseDetailDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsDescriptionDto>))]
        [HttpPut("Description")]
        public async Task<IActionResult> EditDescription([FromBody] GoodsDescriptionDto goodsDto)
        {
            var result = await _goodsService.EditDescription(goodsDto);
            return new Response<GoodsDescriptionDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsDescriptionDto>))]
        [HttpGet("Description/{goodsId}")]
        public async Task<IActionResult> GetGoodsDescription([FromRoute] int goodsId)
        {
            var result = await _goodsService.GetGoodsDescription(goodsId);
            return new Response<GoodsDescriptionDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<VariationParameterGetDto>>))]
        [HttpGet("VarityParameter/{goodsId}")]
        public async Task<IActionResult> GetVarityParameter([FromRoute] int goodsId)
        {
            var result = await _goodsService.GetVarityParameter(goodsId);
            return new Response<List<VariationParameterGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsDocumentDto>))]
        [HttpPost("Document")]
        public async Task<IActionResult> UploadGoodsDocument([FromForm] GoodsDocumentAddDto GoodsDocument)
        {
            var result = await _goodsService.UploadGoodsDocument(GoodsDocument);
            return new Response<GoodsDocumentDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsDocumentDto>>))]
        [HttpGet("Document/{goodsId}/{varityId}")]
        public async Task<IActionResult> GetGoodsDocumentById([FromRoute]int goodsId,[FromRoute] int? varityId)
        {
            var result = await _goodsService.GetGoodsDocumentById(goodsId, varityId);
            return new Response<List<GoodsDocumentDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("Document/{imageId}/{goodsId}")]
        public async Task<IActionResult> DeleteImageById([FromRoute]int imageId,[FromRoute] int goodsId)
        {
            var result = await _goodsService.DeleteImageById(imageId, goodsId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("{goodsId}")]
        public async Task<IActionResult> GoodsDelete([FromRoute]int goodsId)
        {
            var result = await _goodsService.GoodsDelete(goodsId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("Show/{goodsId}")]
        public async Task<IActionResult> GoodsShow([FromRoute]int goodsId)
        {
            var result = await _goodsService.GoodsShow(goodsId);
            return new Response<bool>().ResponseSending(result);
        }
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptNullDto> accept)
        {
            var result = await _goodsService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsSpecificationDto>>))]
        [HttpPost("Specification")]
        public async Task<IActionResult> AddGoodsSpecification([FromBody]List<GoodsSpecificationDto> goodsSpecification)
        {
            var result = await _goodsService.AddGoodsSpecification(goodsSpecification);
            return new Response<List<GoodsSpecificationDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecificationGroupGetForGoodsDto>>))]
        [HttpGet("GoodsSpecification/{goodsId}")]
        public async Task<IActionResult> GoodsSpecification([FromRoute]int goodsId)
        {
            var result = await _goodsService.GetGoodsSpecification(goodsId);
            return new Response<List<SpecificationGroupGetForGoodsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("Provider")]
        public async Task<IActionResult> AddGoodsProvider([FromForm] GoodsProviderSerializeDto goodsProviderDto)
        {
            var result = await _goodsService.AddGoodsProvider(goodsProviderDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsProviderGetDto>>))]
        [HttpGet("Provider/{goodsId}/{shopId}")]
        public async Task<IActionResult> GetGoodsProvider([FromRoute]int goodsId,[FromRoute]int shopId)
        {
            var result = await _goodsService.GetGoodsProvider(goodsId,shopId);
            return new Response<List<GoodsProviderGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("DeleteGoodsProvider/{goodsId}/{goodsProviderId}")]
        public async Task<IActionResult> DeleteGoodsProvider([FromRoute]int goodsId,[FromRoute]int goodsProviderId)
        {
            var result = await _goodsService.DeleteGoodsProvider(goodsId,goodsProviderId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<NoVariationGoodsProviderGetDto>))]
        [HttpGet("NoVariationGoodsProvider/{goodsId}/{shopId}")]
        public async Task<IActionResult> GetNoVariationGoodsProvider([FromRoute]int goodsId,[FromRoute]int shopId)
        {
            var result = await _goodsService.GetNoVariationGoodsProvider(goodsId,shopId);
            return new Response<NoVariationGoodsProviderGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsBaseDetailDto>>))]
        [HttpGet("ByCategoryId")]
        public async Task<IActionResult> GetGoodsByCategoryId([FromQuery] int CategoryId, [FromQuery] string Filter)
        {
            var result = await _goodsService.GetGoodsByCategoryId(CategoryId, Filter);
            return new Response<List<GoodsBaseDetailDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<GoodsListGetDto>>))]
        [HttpGet]
        public async Task<IActionResult> GetAllGoodsByCategoryId([FromQuery] GoodsPaginationDto pagination)
        {
            var result = await _goodsService.GetAllGoodsByCategoryId(pagination);
            return new Response<Pagination<GoodsListGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsMetaDto>))]
        [HttpPut("Meta")]
        public async Task<IActionResult> EditGoodsMeta([FromBody]GoodsMetaDto metaDto)
        {
            var result = await _goodsService.EditGoodsMetaService(metaDto);
            return new Response<GoodsMetaDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsMetaDto>))]
        [HttpGet("Meta/{goodsId}")]
        public async Task<IActionResult> GetGoodsMeta([FromRoute]int goodsId)
        {
            var result = await _goodsService.GetGoodsMeta(goodsId);
            return new Response<GoodsMetaDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpGet("GoodsIncludeVat/{shopId}")]
        public async Task<IActionResult> GetGoodsIncludeVat([FromRoute]int shopId)
        {
            var result = await _goodsService.GetGoodsIncludeVat(shopId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("GoodsGroupEditing")]
        public async Task<IActionResult> GoodsGroupEditing([FromBody] GoodsGroupEditingDto goodsGroupEditing)
        {
            var result = await _goodsService.GoodsGroupEditing(goodsGroupEditing);
            return new Response<bool>().ResponseSending(result);
        }
    }
}