using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Accept;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Repositories.IRepository;

namespace MarketPlace.API.Services.Service
{
    public class CategoryService : ICategoryService
    {
        public IMapper _mapper { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public List<CategoryGetDto> AllCategory { get; set; }

        public CategoryService(
        IMapper mapper,
        ICategoryRepository categoryRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        IFileUploadService fileUploadService)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
            this._fileUploadService = fileUploadService;
            this._ms = ms;
        }
        public async Task<ApiResponse<CategoryAddGetDto>> CategoryAdd(CategorySerializeDto categoryDto)
        {
            var CategoryObj = Extentions.Deserialize<CategoryAddGetDto>(categoryDto.Category);
            if (CategoryObj == null)
            {
                return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BrandDeserialize));
            }
            if(CategoryObj.FkParentId == null )
            {
                if(CategoryObj.CommissionFee ==null )
                {
                    return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ComisionAndRentRequired));
                }
            }
            else
            {
                CategoryObj.CommissionFee = null;
            }
            var CategoryFileName = "";
            if (categoryDto.Image != null)
            {
                CategoryFileName = _fileUploadService.UploadImage(categoryDto.Image, Pathes.CategoryImgTemp);
                if (CategoryFileName == null)
                {
                    return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                CategoryObj.ImageUrl = CategoryFileName;
            }
             var CategoryIconFileName = "";
            if (categoryDto.Icon != null)
            {
                CategoryIconFileName = _fileUploadService.UploadImage(categoryDto.Icon, Pathes.CategoryImgTemp);
                if (CategoryIconFileName == null)
                {
                    return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                CategoryObj.IconUrl = CategoryIconFileName;
            }

            var mapCategory = _mapper.Map<TCategory>(CategoryObj);
            var craetedCategory = await _categoryRepository.CategoryAdd(mapCategory);
            if (craetedCategory.Result == false)
            {
                if (categoryDto.Image != null)
                {
                    _fileUploadService.DeleteImage(CategoryFileName, Pathes.CategoryImgTemp);
                }
                if (categoryDto.Icon != null)
                {
                    _fileUploadService.DeleteImage(CategoryIconFileName, Pathes.CategoryImgTemp);
                }
                return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(craetedCategory.Message));
            }
            if (categoryDto.Image != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(CategoryFileName, Pathes.CategoryImgTemp, Pathes.Category + craetedCategory.Data.CategoryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(CategoryFileName, Pathes.CategoryImgTemp);
                }
            }            
            if (categoryDto.Icon != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(CategoryIconFileName, Pathes.CategoryImgTemp, Pathes.Category + craetedCategory.Data.CategoryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(CategoryIconFileName, Pathes.CategoryImgTemp);
                }
            }
            var mapCraetedCategory = _mapper.Map<CategoryAddGetDto>(craetedCategory.Data);
            return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.Success, mapCraetedCategory, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<CategoryTreeView>>> CategoryGet()
        {
            var dt = await _categoryRepository.GetForWebsite();
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                AllCategory = dt;
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.Success, this.TreeViewChild(null, ""), _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }
        public async Task<ApiResponse<List<CategoryTreeView>>> CategoryGetbyGoodsId(int goodsId)
        {
            var goodsCatId = await _categoryRepository.GetGoodsCategoryId(goodsId);
            if (goodsCatId == 0)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            var CategoryIds = await _categoryRepository.GetParentCatIds(goodsCatId);
            var dt = await _categoryRepository.GetForWebsite();
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                AllCategory = dt;
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.Success, TreeViewChildByCategoryIds(null, "", CategoryIds), _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }


        public async Task<ApiResponse<List<CategoryTreeView>>> GetCategoryTreeView(List<int> catIds)
        {
            var dt = await _categoryRepository.GetForWebsite();
            var ids = new List<int>();
            foreach (var item in catIds)
            {
                var cats = dt.Where(x => x.CategoryPath.Contains("/" + item + "/")).Select(x => x.CategoryPath).ToList();
                foreach (var item2 in cats)
                {
                    ids.AddRange(Extentions.GetParentIds(item2));
                }
            }
            ids = ids.Distinct().ToList();
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                AllCategory = dt;
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.Success, TreeViewChildByCategoryIds(null, "", ids), _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeView>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }

        public async Task<ApiResponse<bool>> CategoryExist(int id)
        {
            var result = await _categoryRepository.CategoryExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.CategoryNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
        public async Task<ApiResponse<CategoryEditDto>> CategoryEdit(CategorySerializeDto categoryDto)
        {
            var CategoryObj = Extentions.Deserialize<CategoryEditDto>(categoryDto.Category);
            if (CategoryObj == null)
            {
                return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BrandDeserialize));
            }
            if(CategoryObj.FkParentId == null )
            {
                if(CategoryObj.CommissionFee ==null )
                {
                    return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ComisionAndRentRequired));
                }
            }
            else
            {
                CategoryObj.CommissionFee = null;
            }
            var exist = await this.CategoryExist(CategoryObj.CategoryId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<CategoryEditDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var fileName = "";
            if (categoryDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(categoryDto.Image, Pathes.CategoryImgTemp);
                if (fileName == null)
                {
                    return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                var isMoved = _fileUploadService.ChangeDestOfFile(fileName, Pathes.CategoryImgTemp, Pathes.Category + CategoryObj.CategoryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.CategoryImgTemp);
                }
                _fileUploadService.DeleteImage(CategoryObj.ImageUrl, Pathes.Category + CategoryObj.CategoryId + "/");
                CategoryObj.ImageUrl = fileName;
            }
            var fileIconName = "";
            if (categoryDto.Icon!= null)
            {
                fileIconName = _fileUploadService.UploadImage(categoryDto.Icon, Pathes.CategoryImgTemp);
                if (fileIconName == null)
                {
                    return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                var isMoved = _fileUploadService.ChangeDestOfFile(fileIconName, Pathes.CategoryImgTemp, Pathes.Category + CategoryObj.CategoryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileIconName, Pathes.CategoryImgTemp);
                }
                _fileUploadService.DeleteImage(CategoryObj.IconUrl, Pathes.Category + CategoryObj.CategoryId + "/");
                CategoryObj.IconUrl = fileIconName;
            }
            var craetedCategory = await _categoryRepository.CategoryEdit(_mapper.Map<TCategory>(CategoryObj));
            if (craetedCategory.Result == false)
            {
                return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(craetedCategory.Message));
            }
            return new ApiResponse<CategoryEditDto>(ResponseStatusEnum.Success, CategoryObj, _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<CategoryTreeViewDto>>> CategoryGetOne(int CategoryId)
        {
            var dt = await _categoryRepository.CategoryGetOne(CategoryId);
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.Success, dt, _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }
        public async Task<ApiResponse<List<CategoryTreeViewDto>>> GetCategoryChildsByCatIdAndPath(CategoryPathDto categoryPath)
        {
            var dt = await _categoryRepository.CategoryGetOne(categoryPath.CategoryId);
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                var Category = dt;
                foreach (var item in Category)
                {
                    if (String.IsNullOrWhiteSpace(categoryPath.CategoryPath))
                    {
                        item.CategoryPath = item.CategoryId.ToString();
                    }
                    else
                    {
                        item.CategoryPath = categoryPath.CategoryPath + "," + item.CategoryId;
                    }
                }
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.Success, Category, _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }
        public async Task<ApiResponse<List<CategoryTreeViewDto>>> GetTrueStatusCategoryChildsByCatIdAndPath(CategoryPathDto categoryPath)
        {
            var dt = await _categoryRepository.CategoryGetOne(categoryPath.CategoryId);
            if (dt == null)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                var Category = dt;
                Category = Category.Where(x => x.IsActive == true).ToList();
                foreach (var item in Category)
                {
                    if (String.IsNullOrWhiteSpace(categoryPath.CategoryPath))
                    {
                        item.CategoryPath = item.CategoryId.ToString();
                    }
                    else
                    {
                        item.CategoryPath = categoryPath.CategoryPath + "," + item.CategoryId;
                    }

                }
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.Success, Category, _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }
        public async Task<ApiResponse<bool>> ChangePriority(ChangePriorityDto changePriority)
        {
            var exist = await this.CategoryExist(changePriority.Id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, exist.Message);
            }
            var result = await _categoryRepository.ChangePriority(changePriority);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CategoryChangePriority));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
        public async Task<ApiResponse<bool>> CategoryDelete(int id)
        {
            var exist = await this.CategoryExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _categoryRepository.CategoryDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                _fileUploadService.DeleteDirectory(Pathes.Category + result.Data.CategoryId);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }
        public async Task<ApiResponse<CategoryGetDto>> GetCategoryById(int CategoryId)
        {
            var dt = await _categoryRepository.GetCategoryById(CategoryId);
            if (dt == null)
            {
                return new ApiResponse<CategoryGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            try
            {
                return new ApiResponse<CategoryGetDto>(ResponseStatusEnum.Success, dt, _ms.MessageService(Message.Successfull));
            }
            catch (System.Exception)
            {
                return new ApiResponse<CategoryGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
        }
        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept)
        {
            // var exist = await this.CategoryExist(accept.Id);
            // if (exist.Status == (int)ResponseStatusEnum.NotFound)
            // {
            //     return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            // }
            var result = await _categoryRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CategoryChangeStatus));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
        public async Task<ApiResponse<bool>> ChangeDisplay(AcceptDto accept)
        {
            var exist = await this.CategoryExist(accept.Id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _categoryRepository.ChangeDisplay(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CategoryChangeDisplayed));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
        public async Task<ApiResponse<bool>> ChangeReturning(AcceptDto accept)
        {
            var exist = await this.CategoryExist(accept.Id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _categoryRepository.ChangeReturning(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CategoryChangeReturning));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> ChangeAppearInFooter(AcceptDto accept)
        {
            if (accept.Accept == true)
            {
                if (!await _categoryRepository.CanAddNewCategoryInFooter())
                {
                    return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.CanNotAddAnotherCategoryInFooter));
                }
            }
            var result = await _categoryRepository.ChangeAppearInFooter(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.CategoryChangeAppearInFooter));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }


        public async Task<ApiResponse<List<CategoryTreeViewDto>>> GetParentCategoryForWebsite(int CategoryId, string categoryPath)
        {
            String[] strlist = categoryPath.Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<int> CategoryIds = new List<int>();
            CategoryIds = Array.ConvertAll(strlist, s => int.Parse(s)).ToList();
           // CategoryIds.Remove(CategoryId);
            var ParentCategory = await _categoryRepository.GetParentCategoryForWebsite(CategoryIds);
            if (ParentCategory == null)
            {
                return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            var newParentCategory = new List<CategoryTreeViewDto>();
            for (int i = 0; i < CategoryIds.Count; i++)
            {
                for (int j = 0; j < ParentCategory.Count; j++)
                {
                    if (CategoryIds[i] == ParentCategory[j].CategoryId)
                    {
                        newParentCategory.Add(ParentCategory[j]);
                    }
                }
            }
            for (int i = 0; i < newParentCategory.Count; i++)
            {
                if (i == 0)
                {
                    newParentCategory[i].CategoryPath = newParentCategory[i].CategoryId.ToString();
                }
                else
                {
                    newParentCategory[i].CategoryPath = newParentCategory[i - 1].CategoryPath + "," + newParentCategory[i].CategoryId.ToString();
                }
            }
            return new ApiResponse<List<CategoryTreeViewDto>>(ResponseStatusEnum.Success, newParentCategory, _ms.MessageService(Message.Successfull));
        }
        public List<CategoryTreeView> TreeViewChild(int? parentId, string path)
        {
            var thisLevel = AllCategory.Where(x => x.FkParentId == parentId).OrderBy(x => x.PriorityNumber).ToList();
            var mapThisLevel = _mapper.Map<List<CategoryTreeView>>(thisLevel);
            if (mapThisLevel.Count > 0)
            {
                for (var i = 0; i < thisLevel.Count; i++)
                {
                    if (path == "")
                    {
                        mapThisLevel[i].CategoryPath = path + mapThisLevel[i].CategoryId;
                    }
                    else
                    {
                        mapThisLevel[i].CategoryPath = path + "," + mapThisLevel[i].CategoryId;

                    }
                    mapThisLevel[i].Child = TreeViewChild(mapThisLevel[i].CategoryId, mapThisLevel[i].CategoryPath);
                }
                return mapThisLevel;
            }
            else
            {
                return mapThisLevel;
            }
        }
        public List<CategoryTreeView> TreeViewChildByCategoryIds(int? parentId, string path, List<int> CategoryIds)
        {
            var thisLevel = new List<CategoryGetDto>();
            var mapThisLevel = new List<CategoryTreeView>();

            if (parentId == null || CategoryIds.Contains(parentId == null ? 0 : (int)parentId))
            {
                thisLevel = AllCategory.Where(x => x.FkParentId == parentId).OrderBy(x => x.PriorityNumber).ToList();
                mapThisLevel = _mapper.Map<List<CategoryTreeView>>(thisLevel);
            }
            if (mapThisLevel.Count > 0)
            {
                for (var i = 0; i < thisLevel.Count; i++)
                {
                    if (path == "")
                    {
                        mapThisLevel[i].CategoryPath = path + mapThisLevel[i].CategoryId;
                    }
                    else
                    {
                        mapThisLevel[i].CategoryPath = path + "," + mapThisLevel[i].CategoryId;

                    }
                    mapThisLevel[i].Child = TreeViewChildByCategoryIds(mapThisLevel[i].CategoryId, mapThisLevel[i].CategoryPath, CategoryIds);
                    if (mapThisLevel[i].Child.Count > 0)
                    {
                        mapThisLevel[i].Expanded = true;
                    }
                }

                return mapThisLevel;
            }
            else
            {
                return mapThisLevel;
            }
        }

        public async Task<ApiResponse<CategoryAddGetDto>> CategoryGetForEdit(int categoryId)
        {
            var data = await _categoryRepository.CategoryGetForEdit(categoryId);
            if (data == null)
            {
                return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            return new ApiResponse<CategoryAddGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CategorySettingPathDto>>> GetFooter()
        {
            var data = await _categoryRepository.GetFooter();
            if (data == null)
            {
                return new ApiResponse<List<CategorySettingPathDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.FooterGetting));
            }
            return new ApiResponse<List<CategorySettingPathDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CategoryGetDto>>> GetAllCategoryGrid(CategoryPaginationDto categoryPagination)
        { 
           
           if(categoryPagination.CategoryId != 0) {
            categoryPagination.Childs = await _categoryRepository.GetCategoriesDirectChilds(categoryPagination.CategoryId);
           } else {
               categoryPagination.Childs = new List<int>();
           }
            var data = await _categoryRepository.GetAllCategoryGrid(categoryPagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CategoryGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            var count = await _categoryRepository.GetAllCategoryGridCount(categoryPagination);
            return new ApiResponse<Pagination<CategoryGetDto>>(ResponseStatusEnum.Success, new Pagination<CategoryGetDto>(count, data), _ms.MessageService(Message.Successfull));         
        }
        

    }
}