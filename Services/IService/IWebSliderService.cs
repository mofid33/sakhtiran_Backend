using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IWebSliderService
    {
        Task<ApiResponse<WebSliderAddDto>> SliderAdd(WebSliderSerializeDto webSlderDto);
        Task<ApiResponse<WebSliderAddDto>> SliderEdit(WebSliderSerializeDto webSlider);
        Task<ApiResponse<bool>> ChangePrioritySlider(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> SliderExist(int id);
        Task<ApiResponse<bool>> SliderDelete(int id);
        Task<ApiResponse<bool>> UploadSliderImage(UploadTowImageDto imageDto);
        Task<ApiResponse<WebSliderGetDto>> SliderGetById(int id);
        Task<ApiResponse<List<WebSliderGetListDto>>> SliderGet();
        Task<ApiResponse<List<WebSliderGetListDto>>> CategorySliderGet(int categoryId);
        // Task<ApiResponse<List<WebSliderGetListDto>>> SliderGetForWebsite(string language,double rate);
    }
}