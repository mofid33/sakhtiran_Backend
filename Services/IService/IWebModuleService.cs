using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Height;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IWebModuleService
    {
        Task<ApiResponse<WebIndexModuleListAddDto>> WebIndexModuleListAdd(WebIndexModuleListAddDto webIndexModuleList);
        Task<ApiResponse<WebIndexModuleListAddDto>> WebIndexModuleListEdit(WebIndexModuleListAddDto webIndexModuleList);
        Task<ApiResponse<bool>> UploadModuleListImage(WebIndexModuleListSerialieDto imageDto);
        Task<ApiResponse<bool>> ChangePriorityOfWebIndexModuleList(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> ChangeHeightOfWebIndexModuleList(ChangeHeight changeHeight);
        Task<ApiResponse<bool>> ChangeAcceptOfWebIndexModuleList(AcceptDto accept);
        Task<ApiResponse<bool>> WebIndexModuleListDelete(int id);

        ////// collection type
        Task<ApiResponse<List<WebCollectionTypeDto>>> GetWebCollectionType(bool slider);

        // get web index module list
        Task<ApiResponse<List<WebHomeIndexModuleListDto>>> WebIndexModuleListGet();
        Task<ApiResponse<List<WebHomeIndexModuleListDto>>> CategoryWebIndexModuleListGet(int categoryId);

        Task<ApiResponse<WebModuleCollectionsAddDto>> WebModuleCollectionsAdd(WebModuleCollectionsSerializeDto collectionsDto);
        Task<ApiResponse<WebModuleCollectionsAddDto>> WebModuleCollectionsEdit(WebModuleCollectionsSerializeDto collectionDto);
        Task<ApiResponse<bool>> ChangePriorityOfWebModuleCollections(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> WebModuleCollectionsDelete(int id);
        Task<ApiResponse<bool>> UploadWebModuleCollectionsImage(UploadTowImageDto imageDto);
        Task<ApiResponse<WebModuleCollectionsGetDto>> WebModuleCollectionsGetById(int id);
        Task<ApiResponse<List<WebHomeModuleCollectionsDto>>> WebModuleCollections(int moduleId);

    }
}