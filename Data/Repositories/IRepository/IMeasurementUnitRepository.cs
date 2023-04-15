using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IMeasurementUnitRepository
    {
        Task<TMeasurementUnit> MeasurementUnitAdd(TMeasurementUnit MeasurementUnit);
        Task<TMeasurementUnit> MeasurementUnitEdit(TMeasurementUnit MeasurementUnit);
        Task<RepRes<TMeasurementUnit>> MeasurementUnitDelete(int id);
        Task<bool> MeasurementUnitExist(int id);
        Task<List<MeasurementUnitDto>> MeasurementUnitGetAll(PaginationDto pagination);
        Task<List<MeasurementUnitDto>> MeasurementUnitGetAllForm();
        Task<int> MeasurementUnitGetAllCount(PaginationDto pagination);
    }
}