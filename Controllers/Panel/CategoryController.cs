using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public ICategoryService _categoryService { get; }
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CategoryAddGetDto>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] CategorySerializeDto categoryDto)
        {
            var result = await _categoryService.CategoryAdd(categoryDto);
            return new Response<CategoryAddGetDto>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryTreeView>>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var result = await _categoryService.CategoryGet();
            return new Response<List<CategoryTreeView>>().ResponseSending(result);
        }

        [HttpGet("GetAllCategoryGrid")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CategoryGetDto>>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCategoryGrid([FromQuery] CategoryPaginationDto categoryPagination)
        {
            var result = await _categoryService.GetAllCategoryGrid(categoryPagination);
            return new Response<Pagination<CategoryGetDto>>().ResponseSending(result);
        }


        [HttpGet("CategoryGetForEdit/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CategoryAddGetDto>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CategoryGetForEdit([FromRoute] int categoryId)
        {
            var result = await _categoryService.CategoryGetForEdit(categoryId);
            return new Response<CategoryAddGetDto>().ResponseSending(result);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryTreeViewDto>>))]
        [Authorize(Roles = "Admin")]
        public async  Task<IActionResult> GetOne([FromRoute]int categoryId)
        {
            var result = await _categoryService.CategoryGetOne(categoryId);
            return new Response<List<CategoryTreeViewDto>>().ResponseSending(result);
        }

        [HttpGet("GetCategoryChilds")] // in estefade mishe
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryTreeViewDto>>))]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> GetCategoryChildsByCatIdAndPath([FromQuery] CategoryPathDto categoryPathDto, bool webSiteSection = false)
        {
            var result = await _categoryService.GetCategoryChildsByCatIdAndPath(categoryPathDto);
            return new Response<List<CategoryTreeViewDto>>().ResponseSending(result);
        }


        [HttpGet("GetCategoryChildsTrue")] // inam estefade mishe
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryTreeViewDto>>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTrueStatusCategoryChildsByCatIdAndPath([FromQuery] CategoryPathDto categoryPathDto)
        {
            var result = await _categoryService.GetTrueStatusCategoryChildsByCatIdAndPath(categoryPathDto);
            return new Response<List<CategoryTreeViewDto>>().ResponseSending(result);
        }

        [HttpGet("GetCategoryById/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CategoryGetDto>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
        {
            var result = await _categoryService.GetCategoryById(categoryId);
            return new Response<CategoryGetDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CategoryEditDto>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit([FromForm] CategorySerializeDto categoryDto)
        {
            var result = await _categoryService.CategoryEdit(categoryDto);
            return new Response<CategoryEditDto>().ResponseSending(result);
        }

        [HttpPut("changePriority")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> changePriority([FromBody] ChangePriorityDto changePriority)
        {
            var result = await _categoryService.ChangePriority(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptDto> accept)
        {
            var result = await _categoryService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ChangeDisplay")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeDisplay([FromBody] AcceptDto accept)
        {
            var result = await _categoryService.ChangeDisplay(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ChangeReturning")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeReturning([FromBody] AcceptDto accept)
        {
            var result = await _categoryService.ChangeReturning(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ChangeAppearInFooter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeAppearInFooter([FromBody] AcceptDto accept)
        {
            var result = await _categoryService.ChangeAppearInFooter(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("Footer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategorySettingPathDto>>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFooter()
        {
            var result = await _categoryService.GetFooter();
            return new Response<List<CategorySettingPathDto>>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _categoryService.CategoryDelete(id);
            return new Response<bool>().ResponseSending(result);
        }
    }
}