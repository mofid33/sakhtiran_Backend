using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class VariationRepository : IVariationRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public VariationRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }


        public async Task<TVariationParameter> VariationParameterAdd(TVariationParameter variationParameter)
        {
            try
            {
                variationParameter.ParameterTitle = JsonExtensions.JsonAdd(variationParameter.ParameterTitle, header);
                await _context.TVariationParameter.AddAsync(variationParameter);
                await _context.SaveChangesAsync();
                variationParameter.ParameterTitle = JsonExtensions.JsonGet(variationParameter.ParameterTitle, header);
                return variationParameter;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TVariationParameter> VariationParameterEdit(TVariationParameter variationParameter)
        {
            try
            {
                var data = await _context.TVariationParameter
                .Include(x => x.TVariationPerCategory).FirstOrDefaultAsync(x => x.ParameterId == variationParameter.ParameterId);
                variationParameter.ParameterTitle = JsonExtensions.JsonEdit(variationParameter.ParameterTitle, data.ParameterTitle, header);
                _context.Entry(data).CurrentValues.SetValues(variationParameter);
                _context.TVariationPerCategory.RemoveRange(data.TVariationPerCategory);
                await _context.SaveChangesAsync();
                foreach (var item in variationParameter.TVariationPerCategory)
                {
                    item.Id = 0;
                    item.FkParameterId = data.ParameterId;
                }
                await _context.TVariationPerCategory.AddRangeAsync(variationParameter.TVariationPerCategory);
                await _context.SaveChangesAsync();
                variationParameter.ParameterTitle = JsonExtensions.JsonGet(variationParameter.ParameterTitle, header);
                return variationParameter;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TVariationParameter>> VariationParameterDelete(int id)
        {
            try
            {
                var data = await _context.TVariationParameter.Include(x => x.TVariationPerCategory).FirstOrDefaultAsync(x => x.ParameterId == id);
                if (data == null)
                {
                    return new RepRes<TVariationParameter>(Message.VariationParameterNotFoundById, false, null);
                }

                var hasRelation = await _context.TGoodsVariety.AsNoTracking().AnyAsync(x => x.FkVariationParameterId == id);
                if (hasRelation)
                {
                    return new RepRes<TVariationParameter>(Message.VariationParameterCantDelete, false, null);
                }
                hasRelation = await _context.TVariationParameterValues.AsNoTracking().AnyAsync(x => x.FkParameterId == id);
                if (hasRelation)
                {
                    return new RepRes<TVariationParameter>(Message.VariationParameterCantDelete, false, null);
                }
                _context.TVariationPerCategory.RemoveRange(data.TVariationPerCategory);
                _context.TVariationParameter.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TVariationParameter>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> VariationParameterExist(int id)
        {
            try
            {
                var result = await _context.TVariationParameter.AsNoTracking().AnyAsync(x => x.ParameterId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<VariationParameterGetDto>> VariationParameterGetAll(PaginationDto pagination)
        {
            try
            {

                return await _context.TVariationParameter
                    .Include(x => x.TVariationPerCategory)
                    .Include(v => v.TVariationParameterValues)
                    .Where(x => ((string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ParameterTitle, header.Language).Contains(pagination.Filter)))) &&
                    (pagination.Id != 0 ? (x.TVariationPerCategory.Any(t => pagination.ChildIds.Contains(t.FkCategoryId))) : true))
                    .OrderByDescending(x => x.ParameterId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new VariationParameterGetDto()
                    {
                        ParameterId = x.ParameterId,
                        ParameterTitle = JsonExtensions.JsonValue(x.ParameterTitle, header.Language),
                        ValuesHaveImage = x.ValuesHaveImage,
                        VariationPerCategoryTitle =  string.Join(",", x.TVariationPerCategory.Select(t => JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language))),
                        TVariationParameterValues = x.TVariationParameterValues.Select(t => new VariationParameterValuesDto()
                        {
                            ValueId = t.ValueId,
                            Value = JsonExtensions.JsonValue(t.Value, header.Language),
                            FkParameterId = t.FkParameterId,
                        }).ToList(),
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> VariationParameterGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TVariationParameter
                    .Include(x => x.TVariationPerCategory)
                    .Include(x => x.TVariationParameterValues)
                    .AsNoTracking()
                    .CountAsync(x => ((string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ParameterTitle, header.Language).Contains(pagination.Filter)))) &&
                    (pagination.Id != 0 ? (x.TVariationPerCategory.Any(t => pagination.ChildIds.Contains(t.FkCategoryId))) : true));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<VariationParameterDto> GetVariationParameterById(int VariationParameterId)
        {
            try
            {
                var data = await _context.TVariationParameter
                .Include(x => x.TVariationPerCategory)
                .Include(x => x.TVariationParameterValues)
                .Select(x => new VariationParameterDto()
                {
                    ParameterId = x.ParameterId,
                    ParameterTitle = JsonExtensions.JsonValue(x.ParameterTitle, header.Language),
                    ValuesHaveImage = x.ValuesHaveImage,
                    TVariationPerCategory = x.TVariationPerCategory.Select(t => new VariationPerCategoryDto()
                    {
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language),
                        FkCategoryId = t.FkCategoryId,
                        FkParameterId = t.FkParameterId,
                        Id = t.Id
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ParameterId == VariationParameterId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        //values
        public async Task<TVariationParameterValues> VariationParameterValuesAdd(TVariationParameterValues values)
        {
            try
            {
                values.Value = JsonExtensions.JsonAdd(values.Value, header);
                await _context.TVariationParameterValues.AddAsync(values);
                await _context.SaveChangesAsync();
                values.Value = JsonExtensions.JsonGet(values.Value, header);
                return values;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TVariationParameterValues> VariationParameterValuesEdit(TVariationParameterValues values)
        {
            try
            {
                var data = await _context.TVariationParameterValues.FirstOrDefaultAsync(x => x.ValueId == values.ValueId);
                values.Value = JsonExtensions.JsonEdit(values.Value, data.Value, header);
                _context.Entry(data).CurrentValues.SetValues(values);
                await _context.SaveChangesAsync();
                values.Value = JsonExtensions.JsonGet(values.Value, header);
                return values;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TVariationParameterValues>> VariationParameterValuesDelete(int id)
        {
            try
            {
                var data = await _context.TVariationParameterValues.FirstOrDefaultAsync(x => x.ValueId == id);
                if (data == null)
                {
                    return new RepRes<TVariationParameterValues>(Message.VariationParameterValuesNotFoundById, false, null);
                }

                var hasRelation = await _context.TGoodsVariety.AsNoTracking().AnyAsync(x => x.FkVariationParameterValueId == id);
                if (hasRelation)
                {
                    return new RepRes<TVariationParameterValues>(Message.VariationParameterValuesCantDelete, false, null);
                }
                _context.TVariationParameterValues.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TVariationParameterValues>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> VariationParameterValuesExist(int id)
        {
            try
            {
                var result = await _context.TVariationParameterValues.AsNoTracking().AnyAsync(x => x.ValueId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<VariationParameterValuesDto>> VariationParameterValuesGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TVariationParameterValues
                    .Where(x => (pagination.Id != 0 ? (x.FkParameterId == pagination.Id) : true) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Value, header.Language).Contains(pagination.Filter))))
                    .OrderByDescending(x => x.ValueId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new VariationParameterValuesDto()
                    {
                        ValueId = x.ValueId,
                        Value = JsonExtensions.JsonValue(x.Value, header.Language),
                        FkParameterId = x.FkParameterId,
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> VariationParameterValuesGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TVariationParameterValues
                    .AsNoTracking()
                    .CountAsync(x => (pagination.Id != 0 ? (x.FkParameterId == pagination.Id) : true) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Value, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<VariationParameterValuesDto> GetVariationParameterValuesById(int VariationParameterValuesId)
        {
            try
            {
                var data = await _context.TVariationParameterValues
                .Select(x => new VariationParameterValuesDto()
                {
                    ValueId = x.ValueId,
                    Value = JsonExtensions.JsonValue(x.Value, header.Language),
                    FkParameterId = x.FkParameterId,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ValueId == VariationParameterValuesId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<VariationParameterGetDto>> GetVarityParameterByCategpryId(List<int> catIds)
        {
            try
            {
                return await _context.TVariationParameter
                .Include(x => x.TVariationPerCategory)
                    .Where(x => x.TVariationPerCategory.Any(t => catIds.Contains(t.FkCategoryId)))
                    .OrderByDescending(x => x.ParameterId)
                    .Include(x => x.TVariationParameterValues)
                    .Select(x => new VariationParameterGetDto()
                    {
                        ParameterId = x.ParameterId,
                        ParameterTitle = JsonExtensions.JsonValue(x.ParameterTitle, header.Language),
                        ValuesHaveImage = x.ValuesHaveImage,
                        TVariationParameterValues = x.TVariationParameterValues.Select(t => new VariationParameterValuesDto()
                        {
                            ValueId = t.ValueId,
                            Value = JsonExtensions.JsonValue(t.Value, header.Language),
                            FkParameterId = t.FkParameterId,
                        }).ToList()
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }


    }
}