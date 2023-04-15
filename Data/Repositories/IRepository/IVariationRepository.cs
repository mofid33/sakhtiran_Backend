using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IVariationRepository
    {
        Task<TVariationParameter> VariationParameterAdd(TVariationParameter variationParameter);
        Task<TVariationParameter> VariationParameterEdit(TVariationParameter variationParameter);
        Task<RepRes<TVariationParameter>> VariationParameterDelete(int id);
        Task<bool> VariationParameterExist(int id);
        Task<List<VariationParameterGetDto>> VariationParameterGetAll(PaginationDto pagination);
        Task<int> VariationParameterGetAllCount(PaginationDto pagination);
        Task<VariationParameterDto> GetVariationParameterById(int variationParameterId);

        //values
        Task<TVariationParameterValues> VariationParameterValuesAdd(TVariationParameterValues values);
        Task<TVariationParameterValues> VariationParameterValuesEdit(TVariationParameterValues values);
        Task<RepRes<TVariationParameterValues>> VariationParameterValuesDelete(int id);
        Task<bool> VariationParameterValuesExist(int id);
        Task<List<VariationParameterValuesDto>> VariationParameterValuesGetAll(PaginationDto pagination);
        Task<int> VariationParameterValuesGetAllCount(PaginationDto pagination);
        Task<VariationParameterValuesDto> GetVariationParameterValuesById(int valuesId);

        //category parameter
        Task<List<VariationParameterGetDto>> GetVarityParameterByCategpryId(List<int> catIds);
    }
}