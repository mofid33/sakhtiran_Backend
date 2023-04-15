using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using AutoMapper;
using MarketPlace.API.Helper;
using AutoMapper.QueryableExtensions;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using System;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class SpecificationGroupRepository : ISpecificationGroupRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public SpecificationGroupRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TSpecificationGroup> SpecificationGroupAdd(TSpecificationGroup specificationGroup)
        {
            try
            {
                specificationGroup.SpecGroupTitle = JsonExtensions.JsonAdd(specificationGroup.SpecGroupTitle, header);
                await _context.TSpecificationGroup.AddAsync(specificationGroup);
                await _context.SaveChangesAsync();
                specificationGroup.SpecGroupTitle = JsonExtensions.JsonGet(specificationGroup.SpecGroupTitle, header);
                return specificationGroup;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TSpecificationGroup>> SpecificationGroupDelete(int id)
        {
            try
            {
                var existInCatSpecGroup = await _context.TCategorySpecificationGroup.AnyAsync(x => x.FkSpecGroupId == id);
                if (existInCatSpecGroup)
                {
                    return new RepRes<TSpecificationGroup>(Message.SpecificationGroupCantDelete, false, null);
                }
                var existInspec = await _context.TSpecification.AnyAsync(x => x.FkSpecGroupId == id);
                if (existInspec)
                {
                    return new RepRes<TSpecificationGroup>(Message.SpecificationGroupCantDelete, false, null);
                }
                var data = await _context.TSpecificationGroup.FindAsync(id);
                _context.TSpecificationGroup.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TSpecificationGroup>(Message.Successfull, true, null);
            }
            catch (System.Exception)
            {
                return new RepRes<TSpecificationGroup>(Message.SpecificationGroupDelete, false, null);
            }
        }

        public async Task<TSpecificationGroup> SpecificationGroupEdit(TSpecificationGroup SpecificationGroup)
        {
            try
            {
                var data = await _context.TSpecificationGroup.FindAsync(SpecificationGroup.SpecGroupId);
                SpecificationGroup.SpecGroupTitle = JsonExtensions.JsonEdit(SpecificationGroup.SpecGroupTitle, data.SpecGroupTitle, header);
                _context.Entry(data).CurrentValues.SetValues(SpecificationGroup);
                await _context.SaveChangesAsync();
                SpecificationGroup.SpecGroupTitle = JsonExtensions.JsonGet(SpecificationGroup.SpecGroupTitle, header);
                return SpecificationGroup;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SpecificationGroupExist(int id)
        {
            try
            {
                var result = await _context.TSpecificationGroup.AsNoTracking().AnyAsync(x => x.SpecGroupId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<SpecificationGroupDto>> SpecificationGroupGetAll(PaginationDto pagination)
        {
            try
            {
                var SpecificationGroup = await _context.TSpecificationGroup
                .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language).Contains(pagination.Filter)))
                .OrderByDescending(x => x.SpecGroupId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new SpecificationGroupDto()
                {
                    SpecGroupId = x.SpecGroupId,
                    SpecGroupTitle = JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language),
                })
                .AsNoTracking().ToListAsync();

                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) &&
                    String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',');
                    var existShop = SpecificationGroup.FirstOrDefault(b => idList.Contains(b.SpecGroupId.ToString()));
                    if (existShop == null)
                    {
                        var findGroup = await _context.TSpecificationGroup.Where(x => idList.Contains(x.SpecGroupId.ToString()))
                        .Select(x => new SpecificationGroupDto()
                        {
                            SpecGroupId = x.SpecGroupId,
                            SpecGroupTitle = JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language),
                        })
                     .AsNoTracking().FirstOrDefaultAsync();
                        if (findGroup != null)
                        {
                            SpecificationGroup.Add(findGroup);
                        }
                    }

                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',');
                    SpecificationGroup = SpecificationGroup.Where(b => !idList.Contains(b.SpecGroupId.ToString())).ToList();
                }
                ///////////////////////
                return SpecificationGroup;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> SpecificationGroupGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TSpecificationGroup
                .AsNoTracking()
                .CountAsync(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language).Contains(pagination.Filter)))
                       && (pagination.Id != 0 ? (x.TCategorySpecificationGroup.Any(a=>a.FkCategoryId == pagination.Id)) : (true) ));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<List<SpecificationGroupGetForGoodsDto>> SpecificationGroupGetWithCategoryId(List<int> categoryId, int goodsId)
        {


           try
           {
            var group = await _context.TCategorySpecificationGroup
            .Where(x => categoryId.Contains(x.FkCategoryId))
            .Include(i => i.FkSpecGroup)
            .ThenInclude(s => s.TSpecification)
            .ThenInclude(o => o.TSpecificationOptions)
            .Include(i => i.FkSpecGroup)
            .ThenInclude(s => s.TSpecification)
            .ThenInclude(l => l.TCategorySpecification).OrderBy(y => y.PriorityNumber).Select(x => x.FkSpecGroup).Distinct().ToListAsync();
            var specificationGroup = group.Select(g => new SpecificationGroupGetForGoodsDto
            {
                SpecGroupTitle = JsonExtensions.JsonGet(g.SpecGroupTitle, header),
                SpecGroupId = g.SpecGroupId,
                Specification = g.TSpecification.Where(h => h.TCategorySpecification.Any(catspec => categoryId.Contains(catspec.FkCategoryId)))
              .OrderBy(y => y.PriorityNumber).Select(spec => new SpecificationGetDto
              {
                  FkSpecGroupId = spec.FkSpecGroupId,
                  IsMultiLineText = spec.IsMultiLineText,
                  IsMultiSelect = spec.IsMultiSelect,
                  IsSelectable = spec.IsSelectable,
                  IsKeySpec = spec.IsKeySpec,
                  IsRequired = spec.IsRequired,
                  SpecId = spec.SpecId,
                  SpecTitle = JsonExtensions.JsonGet(spec.SpecTitle, header),
                  TCategorySpecification = spec.TCategorySpecification
                    .Select(categorySpec => new CategorySpecificationGetDto
                  {
                      FkSpecId = categorySpec.FkSpecId,
                      FkCategoryId = categorySpec.FkCategoryId,
                      Gcsid = categorySpec.Gcsid,
                  }).ToList(),
                  TGoodsSpecification = _context.TGoodsSpecification.Include(r => r.TGoodsSpecificationOptions).Where(v => v.FkGoodsId == goodsId && v.FkSpecId == spec.SpecId)
                    .Select(goodsSpec => new GoodsSpecificationDto
                  {
                      FkGoodsId = goodsSpec.FkGoodsId,
                      FkSpecId = goodsSpec.FkSpecId,
                      SpecValueText = JsonExtensions.JsonValue(goodsSpec.SpecValueText, header.Language),
                      Gsid = goodsSpec.Gsid,
                      TGoodsSpecificationOptions = goodsSpec.TGoodsSpecificationOptions.Select(goodsOption => new GoodsSpecificationOptionsDto
                      {
                          SpecOptionId = goodsOption.SpecOptionId,
                          FkGsid = goodsOption.FkGsid,
                          FkSpecOptionId = goodsOption.FkSpecOptionId
                      }).ToList()
                  }).ToList(),
                  TSpecificationOptions = spec.TSpecificationOptions.OrderBy(c=> c.Priority).Select(specOption => new SpecificationOptionsDto
                  {
                      FkSpecId = specOption.FkSpecId,
                      OptionId = specOption.OptionId,
                      OptionTitle = JsonExtensions.JsonGet(specOption.OptionTitle, header)
                  }).ToList()
              }).ToList(),

            }).ToList();
            return specificationGroup;
           }
           catch (System.Exception)
           {
               
                return null;
           }


        }

        public async Task<List<SpecificationGroupFromDto>> GroupGetByCatId(int categoryId)
        {
            try
            {
                var data = await _context.TCategorySpecificationGroup
                .Where(x => x.FkCategoryId == categoryId)
                .Include(x => x.FkSpecGroup)
                .OrderBy(x => x.PriorityNumber)
                .Select(x => new SpecificationGroupFromDto()
                {
                    SpecGroupTitle = JsonExtensions.JsonValue(x.FkSpecGroup.SpecGroupTitle, header.Language),
                    CatSpecGroupId = x.CatSpecGroupId,
                    FkCategoryId = x.FkCategoryId,
                    FkSpecGroupId = x.FkSpecGroupId,
                    PriorityNumber = x.PriorityNumber
                })  
                .AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<SpecificationGroupDto>> PanelSpecificationGroupGet()
        {
            try
            {
                var data = await _context.TSpecificationGroup
                .Select(x => new SpecificationGroupDto()
                {
                    SpecGroupId = x.SpecGroupId,
                    SpecGroupTitle = JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language),
                })
                .AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<SpecificationGroupGetForGoodsDto>> SpecificationGroupWithSpecGetAll(PaginationDto pagination)
        {
            try
            {
                var SpecificationGroup = await _context.TSpecificationGroup
                 .Include(b=>b.TSpecification)
                 .Include(c=>c.TCategorySpecificationGroup)
                 .ThenInclude(s=>s.FkCategory)
                .Where(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language).Contains(pagination.Filter)))
                       && (pagination.Id != 0 ? (x.TCategorySpecificationGroup.Any(a=>a.FkCategoryId == pagination.Id)) : (true) ))
                .OrderByDescending(x => x.SpecGroupId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new SpecificationGroupGetForGoodsDto()
                {
                    SpecGroupId = x.SpecGroupId,
                    SpecGroupTitle = JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language),
                    Specification = x.TSpecification.Select(s=>new SpecificationGetDto {
                            
                        SpecTitle = JsonExtensions.JsonValue(s.SpecTitle, header.Language) 

                    }).ToList() ,                   
                    Category = x.TCategorySpecificationGroup
                                    .Where(x => (pagination.Id != 0 ? (x.FkCategoryId == pagination.Id) : (true) ))
                    .Select(s=>new CategoryFormGetDto {
                        CategoryId = s.FkCategoryId , 
                        CategoryTitle = JsonExtensions.JsonValue(s.FkCategory.CategoryTitle , header.Language) 
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();

                return SpecificationGroup;
            }
            catch (System.Exception)
            {
                return null;
            }        
            }

        public async Task<SpecificationGroupDto> GroupGetById(int id)
        {
            try
            {
                var data = await _context.TSpecificationGroup
                .Where(x => x.SpecGroupId == id)
                .Select(x => new SpecificationGroupDto()
                {
                    SpecGroupTitle = JsonExtensions.JsonValue(x.SpecGroupTitle, header.Language),
                    SpecGroupId = x.SpecGroupId
                })  
                .AsNoTracking().FirstOrDefaultAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }        }
    }
}