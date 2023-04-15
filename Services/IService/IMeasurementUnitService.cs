using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IMeasurementUnitService
    {
        Task<ApiResponse<MeasurementUnitDto>> MeasurementUnitAdd(MeasurementUnitDto MeasurementUnit);
        Task<ApiResponse<MeasurementUnitDto>> MeasurementUnitEdit(MeasurementUnitDto MeasurementUnit);
        Task<ApiResponse<bool>> MeasurementUnitDelete(int id);
        Task<ApiResponse<bool>> MeasurementUnitExist(int id);
        Task<ApiResponse<Pagination<MeasurementUnitDto>>> MeasurementUnitGetAll(PaginationDto pagination);
        Task<ApiResponse<List<MeasurementUnitDto>>> GetMeasurementUnit();
    }
}