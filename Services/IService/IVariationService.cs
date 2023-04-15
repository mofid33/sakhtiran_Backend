using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IVariationService
    {
        Task<ApiResponse<VariationParameterDto>> VariationParameterAdd(VariationParameterDto variationParameterDto);
        Task<ApiResponse<VariationParameterDto>> VariationParameterEdit(VariationParameterDto variationParameterDto);
        Task<ApiResponse<bool>> VariationParameterDelete(int id);
        Task<ApiResponse<bool>> VariationParameterExist(int id);
        Task<ApiResponse<Pagination<VariationParameterGetDto>>> VariationParameterGetAll(PaginationDto pagination);
        Task<ApiResponse<VariationParameterDto>> GetVariationParameterById(int variationParameterId);

        //values
        Task<ApiResponse<VariationParameterValuesDto>> VariationParameterValuesAdd(VariationParameterValuesDto valuesDto);
        Task<ApiResponse<VariationParameterValuesDto>> VariationParameterValuesEdit(VariationParameterValuesDto valuesDto);
        Task<ApiResponse<bool>> VariationParameterValuesDelete(int id);
        Task<ApiResponse<bool>> VariationParameterValuesExist(int id);
        Task<ApiResponse<Pagination<VariationParameterValuesDto>>> VariationParameterValuesGetAll(PaginationDto pagination);
        Task<ApiResponse<VariationParameterValuesDto>> GetVariationParameterValuesById(int valuesId);

    }
}