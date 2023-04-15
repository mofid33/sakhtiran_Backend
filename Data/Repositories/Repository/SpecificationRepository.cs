using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class SpecificationRepository : ISpecificationRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }

        public SpecificationRepository(MarketPlaceDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TSpecification> SpecificationAdd(TSpecification specification)
        {
            try
            {
                specification.SpecTitle = JsonExtensions.JsonAdd(specification.SpecTitle, header);
                if(specification.TSpecificationOptions != null)
                {
                    foreach (var item in specification.TSpecificationOptions)
                    {
                        item.OptionTitle = JsonExtensions.JsonAdd(item.OptionTitle, header);
                    }
                }
                var specGroupId = specification.FkSpecGroupId;
                var CountSpecInGroup = await _context.TSpecification.CountAsync(x => x.FkSpecGroupId == specGroupId);
                specification.PriorityNumber = CountSpecInGroup + 1;
               // specification.Status = false;
                var newCatSpecGroupList = new List<TCategorySpecificationGroup>();
                foreach (var item in specification.TCategorySpecification)
                {
                    var ExistGroupInCat = await _context.TCategorySpecificationGroup.AnyAsync(x => x.FkCategoryId == item.FkCategoryId && x.FkSpecGroupId == specGroupId);
                    if (!ExistGroupInCat)
                    {
                        var CountCatSpecGroup = await _context.TCategorySpecificationGroup.CountAsync(x => x.FkCategoryId == item.FkCategoryId);
                        var newCatSpecGroup = new TCategorySpecificationGroup();
                        newCatSpecGroup.FkCategoryId = item.FkCategoryId;
                        newCatSpecGroup.FkSpecGroupId = specGroupId;
                        newCatSpecGroup.PriorityNumber = CountCatSpecGroup + 1;
                        newCatSpecGroupList.Add(newCatSpecGroup);
                    }
                }
                if(newCatSpecGroupList.Count>0)
                {
                    await _context.TCategorySpecificationGroup.AddRangeAsync(newCatSpecGroupList);
                }
                await _context.TSpecification.AddAsync(specification);
                await _context.SaveChangesAsync();
                return specification;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeKeyAndRequired(SpecificationKeyAndRequiredDto KeyAndRequired)
        {
            try
            {
                var data = await _context.TSpecification.FirstOrDefaultAsync(x => x.SpecId == KeyAndRequired.SpecId );
                if (KeyAndRequired.IsKey)
                {
                    data.IsKeySpec = KeyAndRequired.Value;
                }
                else
                {
                    data.IsRequired = KeyAndRequired.Value;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditPriorityGroup(ChangePriorityDto changePriority)
        {
            try
            {

                var data = await _context.TCategorySpecificationGroup.FirstOrDefaultAsync(x => x.FkSpecGroupId == changePriority.Id && x.FkCategoryId == changePriority.ParentID);
                if (data.PriorityNumber > changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TCategorySpecificationGroup
                    .Where(x => x.FkCategoryId == changePriority.ParentID && x.PriorityNumber < data.PriorityNumber && x.PriorityNumber >= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber + 1;
                    }
                }
                else if (data.PriorityNumber < changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TCategorySpecificationGroup
                    .Where(x => x.FkCategoryId == changePriority.ParentID && x.PriorityNumber > data.PriorityNumber && x.PriorityNumber <= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber - 1;
                    }
                }
                else
                {

                }
                data.PriorityNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }        
        
        public async Task<bool> EditPrioritySpec(ChangePriorityDto changePriority)
        {
            try
            {

                var data = await _context.TSpecification.FirstOrDefaultAsync(x => x.FkSpecGroupId == changePriority.ParentID && x.SpecId == changePriority.Id);
                if (data.PriorityNumber > changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TSpecification
                    .Where(x => x.FkSpecGroupId == changePriority.ParentID && x.PriorityNumber < data.PriorityNumber && x.PriorityNumber >= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber + 1;
                    }
                }
                else if (data.PriorityNumber < changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TSpecification
                    .Where(x => x.FkSpecGroupId == changePriority.ParentID && x.PriorityNumber > data.PriorityNumber && x.PriorityNumber <= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber - 1;
                    }
                }
                else
                {

                }
                data.PriorityNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CategorySpecificationGetDto>> SpecificationGetByCategoryId(int categoryId)
        {
            try
            {
                var data = await _context.TCategorySpecification
                .Where(x => x.FkCategoryId == categoryId)
                .Include(x => x.FkSpec)
                .Select(x=> new CategorySpecificationGetDto(){
                    Gcsid = x.Gcsid,
                    FkCategoryId = x.FkCategoryId,
                    FkSpecId = x.FkSpecId,
                    SpecTitle = JsonExtensions.JsonValue(x.FkSpec.SpecTitle, header.Language),
                    CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    CategoryPath = ""
                })
                .AsNoTracking()
                .ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SpecificationExist(int id)
        {
            try
            {
                var result = await _context.TSpecification.AsNoTracking().AnyAsync(x => x.SpecId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<TSpecification>> SpecificationDeletebyId(int id)
        {
            try
            {
                var data = await _context.TSpecification.FindAsync(id);

                var GroupSpec = await _context.TSpecification
                .Where(x=>x.FkSpecGroupId == data.FkSpecGroupId && x.PriorityNumber>data.PriorityNumber ).ToListAsync();

                foreach (var item in GroupSpec)
                {
                    item.PriorityNumber = item.PriorityNumber - 1;
                }
                var allCategorySpek = await _context.TCategorySpecification.Where(x => x.FkSpecId == id).ToListAsync();
                foreach (var item in allCategorySpek)
                {
                    var ExistSpec = await _context.TSpecification
                    .AnyAsync(x=>x.FkSpecGroupId == data.FkSpecGroupId && x.SpecId != data.SpecId && x.TCategorySpecification.Any(t=>t.FkCategoryId == item.FkCategoryId));
                    if(!ExistSpec)
                    {
                        var xx = await _context.TCategorySpecificationGroup.FirstOrDefaultAsync(x=>x.FkCategoryId == item.FkCategoryId && x.FkSpecGroupId == data.FkSpecGroupId);
                        var tt = await _context.TCategorySpecificationGroup.Where(x=>x.FkCategoryId == item.FkCategoryId && x.PriorityNumber > xx.PriorityNumber).ToListAsync();
                        foreach (var item2 in tt)
                        {
                            item2.PriorityNumber = item2.PriorityNumber - 1;
                        }
                        _context.TCategorySpecificationGroup.Remove(xx);
                        await _context.SaveChangesAsync();
                    }
                }

                _context.TCategorySpecification.RemoveRange(allCategorySpek);
                _context.TSpecification.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TSpecification>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TSpecification>(Message.SpecificationDelete,false,null);
            }
        }

        public async Task<bool> SpecificationExistByCategoryId(int categoryId, int id)
        {
            try
            {
                var result = await _context.TCategorySpecification.AsNoTracking().AnyAsync(x => x.FkSpecId == id && x.FkCategoryId == categoryId);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<TSpecification>> SpecificationDeletebyCategoryId(int categoryId, int id)
        {
            try
            {
                var data = await _context.TCategorySpecification.FirstOrDefaultAsync(x => x.FkSpecId == id && x.FkCategoryId == categoryId);
                var spec = await _context.TSpecification.FindAsync(id);
                
                 var ss = await _context.TSpecification
                    .Include(x=>x.TCategorySpecification)
                    .AsNoTracking()
                    .AnyAsync(x=>x.FkSpecGroupId == spec.FkSpecGroupId && 
                    x.SpecId != id &&
                    x.TCategorySpecification.Any(t=>t.FkCategoryId == categoryId));
                    if(!ss)
                    {
                        var xx = await _context.TCategorySpecificationGroup.FirstOrDefaultAsync(x=>x.FkCategoryId == categoryId && x.FkSpecGroupId == spec.FkSpecGroupId);
                        var tt = await _context.TCategorySpecificationGroup.Where(x=>x.FkCategoryId == categoryId && x.PriorityNumber > xx.PriorityNumber).ToListAsync();
                        foreach (var item2 in tt)
                        {
                            item2.PriorityNumber = item2.PriorityNumber - 1;
                        }
                        _context.TCategorySpecificationGroup.Remove(xx);
                        await _context.SaveChangesAsync();
                    }
                _context.TCategorySpecification.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TSpecification>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TSpecification>(Message.SpecificationDelete,false,null);
            }
        }

        public async Task<SpecificationGetDto> SpecificationGetById(int id)
        {
            try
            {
                var data = await _context.TSpecification
                .Include(x => x.FkSpecGroup)
                .Include(x => x.TSpecificationOptions)
                .Include(x => x.TCategorySpecification).ThenInclude(t => t.FkCategory)
                .Select(x => new SpecificationGetDto()
                {
                    SpecId = x.SpecId,
                    SpecTitle = JsonExtensions.JsonValue(x.SpecTitle, header.Language),
                    FkSpecGroupId = x.FkSpecGroupId,
                    IsMultiSelectInFilter = x.IsMultiSelectInFilter,
                    IsSelectable = x.IsSelectable,
                    IsMultiSelect = x.IsMultiSelect,
                    IsMultiLineText = x.IsMultiLineText,
                    IsKeySpec = x.IsKeySpec,
                    IsRequired = x.IsRequired,
                    Status = x.Status,
                    TSpecificationOptions = x.TSpecificationOptions.OrderBy(c=> c.Priority).Select(t => new SpecificationOptionsDto()
                    {
                        OptionId = t.OptionId,
                        OptionTitle = JsonExtensions.JsonValue(t.OptionTitle, header.Language),
                        FkSpecId = t.FkSpecId,
                        Priority = t.Priority
                    }).ToList(),
                    TCategorySpecification = x.TCategorySpecification.Select(t => new CategorySpecificationGetDto()
                    {
                        Gcsid = t.Gcsid,
                        FkCategoryId = t.FkCategoryId,
                        FkSpecId = t.FkSpecId,
                        SpecTitle = JsonExtensions.JsonValue(x.SpecTitle, header.Language),
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language),
                        CategoryPath = ""
                    }).ToList(),
                    TGoodsSpecification = null
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.SpecId == id);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SpecificationEdit(SpecificationEditDto SpecificationEdit)
        {
            try
            {
                var data = await _context.TSpecification.Include(c=>c.TSpecificationOptions).FirstAsync( x=>x.SpecId == SpecificationEdit.SpecId);

                //
                if(SpecificationEdit.FkSpecGroupId != data.FkSpecGroupId)
                {
                    var oldSpecWhitGroupId = await _context.TSpecification
                    .Where(x=>x.FkSpecGroupId == data.FkSpecGroupId && x.PriorityNumber > data.PriorityNumber).ToListAsync();
                    foreach (var item in oldSpecWhitGroupId)
                    {
                        item.PriorityNumber = item.PriorityNumber - 1;
                    }
                    var newSpecWhitGroup = await _context.TSpecification.AsNoTracking().CountAsync(x=>x.FkSpecGroupId == SpecificationEdit.FkSpecGroupId);
                    data.PriorityNumber = newSpecWhitGroup + 1;


                    var allCatId = await _context.TCategorySpecification
                    .Where(x=>x.FkSpecId == data.SpecId).Select(x=>x.FkCategoryId).ToListAsync();

                    var newSpeccatGroup = new List<TCategorySpecificationGroup>();
                    foreach (var item in allCatId)
                    {
                        var x = await _context.TCategorySpecificationGroup
                        .AsNoTracking()
                        .AnyAsync(g=>g.FkCategoryId == item 
                        && g.FkSpecGroupId == SpecificationEdit.FkSpecGroupId);

                        if(!x)
                        {
                            var CountCatSpecGroup = await _context.TCategorySpecificationGroup.CountAsync(r => r.FkCategoryId == item);
                            var newSpeccatGroupObj = new TCategorySpecificationGroup();
                            newSpeccatGroupObj.PriorityNumber = CountCatSpecGroup + 1;
                            newSpeccatGroupObj.FkSpecGroupId = SpecificationEdit.FkSpecGroupId;
                            newSpeccatGroupObj.FkCategoryId = item;
                            newSpeccatGroup.Add(newSpeccatGroupObj);
                        }
                    }
                    await _context.TCategorySpecificationGroup.AddRangeAsync(newSpeccatGroup);
                    await _context.SaveChangesAsync();


                    foreach (var item in allCatId)
                    {
                        var existSpec  = await _context.TSpecification
                        .Include(x=>x.TCategorySpecification).AsNoTracking()
                        .AnyAsync(x=>x.TCategorySpecification.Any(t=>t.FkCategoryId == item)
                         && x.FkSpecGroupId == data.FkSpecGroupId && x.SpecId != data.SpecId );
                        if(!existSpec)
                        {
                            var catGroup = await _context.TCategorySpecificationGroup
                            .FirstOrDefaultAsync(x=>x.FkCategoryId == item && x.FkSpecGroupId == data.FkSpecGroupId);

                            var catGroupBetween = await _context.TCategorySpecificationGroup
                            .Where(x=>x.PriorityNumber > catGroup.PriorityNumber &&
                             x.FkCategoryId == item && x.FkSpecGroupId == data.FkSpecGroupId).ToListAsync();
                            foreach (var item2 in catGroupBetween)
                            {
                                item2.PriorityNumber = item2.PriorityNumber - 1; 
                            }
                            _context.TCategorySpecificationGroup.Remove(catGroup);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                data.FkSpecGroupId = SpecificationEdit.FkSpecGroupId;
                data.SpecTitle = JsonExtensions.JsonEdit( SpecificationEdit.SpecTitle, data.SpecTitle, header);
                data.IsSelectable = SpecificationEdit.IsSelectable;
                data.IsMultiSelect = SpecificationEdit.IsMultiSelect;
                data.IsMultiLineText = SpecificationEdit.IsMultiLineText;
                data.IsMultiSelectInFilter = SpecificationEdit.IsMultiSelectInFilter;
                data.IsRequired = SpecificationEdit.IsRequired;
                data.IsKeySpec = SpecificationEdit.IsKeySpec;
                data.Status = SpecificationEdit.Status;

                // foreach (var item in data.TSpecificationOptions)
                // {
                //      foreach (var itemEdit in SpecificationEdit.EditOptions)
                //      {
                //          if(item.OptionId == itemEdit.OptionId) 
                //          {
                                  
                //          }
                //      }
                // }


                await _context.SaveChangesAsync();

                return true; 

            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TSpecification> SpecificationGetData(int id)
        {
            try
            {
                var data = await _context.TSpecification.FindAsync(id);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteAllOptionsBySpecId(int id)
        {
            try
            {
                var AllOptions = await _context.TSpecificationOptions.Where(x => x.FkSpecId == id).ToListAsync();
                _context.TSpecificationOptions.RemoveRange(AllOptions);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<bool> DeleteOptionsByIds(List<int> optionIds)
        {
            try
            {
                var OptionsByIds = await _context.TSpecificationOptions.Where(x => optionIds.Contains(x.OptionId)).ToListAsync();
                _context.TSpecificationOptions.RemoveRange(OptionsByIds);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCatSpecByIds(List<int> CatSpecByIds,int specId , int groupId)
        {
            try
            {
                var allCatSpec = await _context.TCategorySpecification.Where(x => CatSpecByIds.Contains(x.FkCategoryId) && x.FkSpecId == specId).ToListAsync();

                foreach (var item in CatSpecByIds)
                {
                    // is a spec in the group that the group is in this catid
                    var ss = await _context.TSpecification
                    .Include(x=>x.TCategorySpecification)
                    .AsNoTracking()
                    .AnyAsync(x=>x.FkSpecGroupId == groupId && 
                    x.SpecId != specId &&
                    x.TCategorySpecification.Any(t=>t.FkCategoryId == item));
                    if(!ss)
                    {
                        var xx = await _context.TCategorySpecificationGroup.FirstOrDefaultAsync(x=>x.FkCategoryId == item && x.FkSpecGroupId == groupId);
                        if(xx == null)
                        {
                            break;
                        }
                        var tt = await _context.TCategorySpecificationGroup.Where(x=>x.FkCategoryId == item && x.PriorityNumber > xx.PriorityNumber).ToListAsync();
                        foreach (var item2 in tt)
                        {
                            item2.PriorityNumber = item2.PriorityNumber - 1;
                        }
                        _context.TCategorySpecificationGroup.Remove(xx);
                        await _context.SaveChangesAsync();
                    }
                }
                _context.TCategorySpecification.RemoveRange(allCatSpec);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }



        public async Task<bool> AddCategorySpecification(List<TCategorySpecification> categorySpecification,int specId , int groupId)
        {
            try
            {

                await _context.TCategorySpecification.AddRangeAsync(categorySpecification);
                var newCatSpecGroupList = new List<TCategorySpecificationGroup>();
                foreach (var item in categorySpecification)
                {
                    var ExistGroupInCat = await _context.TCategorySpecificationGroup.AnyAsync(x => x.FkCategoryId == item.FkCategoryId && x.FkSpecGroupId == groupId);
                    if (!ExistGroupInCat)
                    {
                        var CountCatSpecGroup = await _context.TCategorySpecificationGroup.CountAsync(x => x.FkCategoryId == item.FkCategoryId);
                        var newCatSpecGroup = new TCategorySpecificationGroup();
                        newCatSpecGroup.FkCategoryId = item.FkCategoryId;
                        newCatSpecGroup.FkSpecGroupId = groupId;
                        newCatSpecGroup.PriorityNumber = CountCatSpecGroup + 1;
                        newCatSpecGroupList.Add(newCatSpecGroup);
                    }
                }
                if(newCatSpecGroupList.Count>0)
                {
                    await _context.TCategorySpecificationGroup.AddRangeAsync(newCatSpecGroupList);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<SpecificationFormDto>> GetSpecsByGroupId(int groupId)
        {
            try
            {
                var data = await _context.TSpecification.Where(x=>x.FkSpecGroupId == groupId)
                .OrderBy(x=>x.PriorityNumber)
                .Select(x=>new SpecificationFormDto(){
                    SpecId = x.SpecId,
                    SpecTitle = JsonExtensions.JsonValue(x.SpecTitle, header.Language),
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

        public async Task<List<WebsiteSpecificationDto>> GetSpecsForWebsite(List<int> categoryIds)
        {
            try
            {
                var spec = await _context.TSpecification
                .Include(x=>x.TCategorySpecification)
                .Where(x=>x.IsSelectable == true &&
                x.Status == true &&
                x.TCategorySpecification.Any(i=>categoryIds.Contains(i.FkCategoryId))
                )
                .Include(x=>x.TSpecificationOptions)
                .OrderBy(b=>b.PriorityNumber)
                .Select(x=> new WebsiteSpecificationDto(){
                    SpecId = x.SpecId,
                    SpecTitle = JsonExtensions.JsonValue(x.SpecTitle, header.Language),
                    IsMultiSelectInFilter = x.IsMultiSelectInFilter,
                    Options = x.TSpecificationOptions.OrderBy(c=> c.Priority).Select(i=> new WebsiteSpecificationOptionDto(){
                        OptionId = (int) i.OptionId,
                        Priority = (int) i.Priority,
                        OptionTitle = JsonExtensions.JsonValue(i.OptionTitle, header.Language),
                        FkSpecId = i.FkSpecId
                    }).ToList()
                })
                .ToListAsync();

                return spec;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<SpecificationCatGroupDto>> GetSpecs(SpecPagination pagination)
        {
            try
            {
                return await _context.TSpecification
                .Include(x=>x.TCategorySpecification).ThenInclude(t=>t.FkCategory)
                .Include(x=>x.FkSpecGroup)
                .Where(x=>
                (pagination.CategoryId != 0 ? x.TCategorySpecification.Any(t=>pagination.CatChilds.Contains(t.FkCategoryId)):true)&&
                (pagination.GroupId != 0 ? (x.FkSpecGroupId == pagination.GroupId) : true)&&
                (string.IsNullOrWhiteSpace(pagination.Filter)?true:JsonExtensions.JsonValue(x.SpecTitle,header.Language).Contains(pagination.Filter))
                )
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x=> new SpecificationCatGroupDto(){
                    Category = x.TCategorySpecification.Select(t=>new CategorySpecDto(){
                        CategoryId = t.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle,header.Language)
                    }).ToList(),
                   CategoryTitle =  string.Join(",", x.TCategorySpecification.Select(t => JsonExtensions.JsonValue(t.FkCategory.CategoryTitle,header.Language))),

                    FkSpecGroupId = x.FkSpecGroupId,
                    SpecGroupTitle = JsonExtensions.JsonValue(x.FkSpecGroup.SpecGroupTitle,header.Language),
                    IsKeySpec = x.IsKeySpec,
                    IsMultiLineText = x.IsMultiLineText,
                    IsMultiSelect = x.IsMultiSelect,
                    IsMultiSelectInFilter = x.IsMultiSelectInFilter,
                    IsRequired = x.IsRequired,
                    IsSelectable = x.IsSelectable,
                    SpecId = x.SpecId,
                    SpecTitle = JsonExtensions.JsonValue(x.SpecTitle,header.Language),
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetSpecsCount(SpecPagination pagination)
        {
            try
            {
                return await _context.TSpecification
                .Include(x=>x.TCategorySpecification)
                .AsNoTracking()
                .CountAsync(x=>
                (pagination.CategoryId != 0 ? x.TCategorySpecification.Any(t=>pagination.CatChilds.Contains(t.FkCategoryId)):true)&&
                (pagination.GroupId != 0 ? (x.FkSpecGroupId == pagination.GroupId) : true)&&
                (string.IsNullOrWhiteSpace(pagination.Filter)?true:JsonExtensions.JsonValue(x.SpecTitle,header.Language).Contains(pagination.Filter))
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeAccept(AcceptDto accept)
        {
            try
            {
                var data = await _context.TSpecification.FindAsync(accept.Id);
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddSpecificationOptions(List<TSpecificationOptions> specificationOptions  , int specId)
        {
            try
            {
                var addSpec = specificationOptions.Where(v=>v.OptionId == 0).ToList();
                var editSpec = specificationOptions.Where(v=>v.OptionId != 0).ToList();
                if(addSpec != null)
                {
                    foreach (var item in addSpec)
                    {
                        item.OptionTitle = JsonExtensions.JsonAdd(item.OptionTitle, header);
                    }
                }
                await _context.TSpecificationOptions.AddRangeAsync(addSpec);
                await _context.SaveChangesAsync();

                var specOption = await _context.TSpecificationOptions.Where(c=>c.FkSpecId == specId).ToArrayAsync();
                for (int i = 0; i < specOption.Length; i++)
                {
                    foreach (var item in editSpec)
                    {
                        if(specOption[i].OptionId == item.OptionId) {
                            specOption[i].OptionTitle =  JsonExtensions.JsonEdit(item.OptionTitle,specOption[i].OptionTitle, header);
                            specOption[i].Priority = item.Priority;
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}