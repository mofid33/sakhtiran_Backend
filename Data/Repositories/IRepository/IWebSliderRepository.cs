using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IWebSliderRepository
    {
        Task<WebSlider> WebSliderAdd(WebSlider webSliderAdd);
        Task<WebSlider> SliderEdit(WebSlider webSlider);
        Task<bool> SliderExist(int id);
        Task<bool> ChangePrioritySlider(ChangePriorityDto changePriority);
        Task<RepRes<WebSlider>> SliderDelete(int id);
        Task<WebSliderAddDto> UploadSliderImage(string fileName,string ResponsiveImageUrl, int id);
        Task<WebSliderGetDto> SliderGetById(int id);
        Task<List<WebSliderGetListDto>> SliderGet(int getType, decimal rate,int? categoryId);
        Task<int> SliderCount(int? categotyId);

    }
}