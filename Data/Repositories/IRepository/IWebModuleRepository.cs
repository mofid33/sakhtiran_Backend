using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Height;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IWebModuleRepository
    {
        Task<WebIndexModuleList> WebIndexModuleListAdd(WebIndexModuleList webIndexModuleList);
        Task<WebIndexModuleList> WebIndexModuleListEdit(WebIndexModuleList webIndexModuleList);
        Task<bool> UploadModuleListImage(string fileName,string title, int Id);
        Task<List<WebHomeIndexModuleListDto>> GetModuleCollection(int getType,decimal rate,int? categoryId);
        Task<bool> ChangePriorityOfWebIndexModuleList(ChangePriorityDto changePriority);
        Task<bool> ChangeHeightOfWebIndexModuleList(ChangeHeight changeHeight);
        Task<bool> ChangeAcceptOfWebIndexModuleList(AcceptDto changePriority);
        Task<bool> WebIndexModuleListExist(int id);
        ////// collection type
        Task<List<WebCollectionType>> GetWebCollectionType(bool slider);
        Task<WebIndexModuleList> WebIndexModuleListDelete(int id);
        Task<WebModuleCollections> WebModuleCollectionsAdd(WebModuleCollections webModuleCollections);
        Task<WebModuleCollections> WebModuleCollectionsEdit(WebModuleCollections webModuleCollections);
        Task<bool> ChangePriorityOfWebModuleCollections(ChangePriorityDto changePriority);
        Task<bool> WebModuleCollectionsExist(int id);
        Task<WebModuleCollections> WebModuleCollectionsDelete(int id);
        Task<WebModuleCollectionsAddDto> UploadWebModuleCollectionsImage(string fileName,string ResponsiveFileName, int id);
        Task<WebModuleCollectionsGetDto> WebModuleCollectionsGetById(int id);
        Task<List<WebHomeModuleCollectionsDto>> WebModuleCollectionsByModuleId(int moduleId);
    }
}