using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public DocumentTypeRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<TDocumentType> DocumentTypeAdd(TDocumentType DocumentType)
        {
            try
            {
                DocumentType.DocumentTitle = JsonExtensions.JsonAdd(DocumentType.DocumentTitle, header);
                DocumentType.Status = true;
                await _context.TDocumentType.AddAsync(DocumentType);
                await _context.SaveChangesAsync();
                DocumentType.DocumentTitle = JsonExtensions.JsonGet(DocumentType.DocumentTitle, header);
                return DocumentType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TDocumentType> DocumentTypeEdit(TDocumentType DocumentType)
        {
            try
            {
                var data = await _context.TDocumentType.FirstOrDefaultAsync(x => x.DocumentTypeId == DocumentType.DocumentTypeId);
                DocumentType.DocumentTitle = JsonExtensions.JsonEdit(DocumentType.DocumentTitle, data.DocumentTitle, header);
                _context.Entry(data).CurrentValues.SetValues(DocumentType);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                await _context.SaveChangesAsync();
                DocumentType.DocumentTitle = JsonExtensions.JsonGet(DocumentType.DocumentTitle, header);
                return DocumentType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TDocumentType>> DocumentTypeDelete(int id)
        {
            try
            {
                var data = await _context.TDocumentType.FirstOrDefaultAsync(x => x.DocumentTypeId == id);
                if (data == null)
                {
                    return new RepRes<TDocumentType>(Message.DocumentTypeNotFoundById, false, null);
                }

                var hasRelation = await _context.TShopFiles.AsNoTracking().AnyAsync(x => x.FkDocumentTypeId == id);
                if (hasRelation)
                {
                    return new RepRes<TDocumentType>(Message.DocumentTypeCantDelete, false, null);
                }
                _context.TDocumentType.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TDocumentType>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> DocumentTypeExist(int id)
        {
            try
            {
                var result = await _context.TDocumentType.AsNoTracking().AnyAsync(x => x.DocumentTypeId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<DocumentTypeGetDto>> DocumentTypeGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TDocumentType
                    .Where(x => (pagination.Id == 0 ? true : (x.FkGroupd == pagination.Id)) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.DocumentTitle, header.Language).Contains(pagination.Filter))))
                    .Include(c=>c.FkGroupdNavigation)
                    .Include(c=>c.FkPerson)
                    .OrderByDescending(x => x.DocumentTypeId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new DocumentTypeGetDto()
                    {
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentTitle = JsonExtensions.JsonValue(x.DocumentTitle, header.Language),
                        FkGroupd = x.FkGroupd,
                        GroupTitle = JsonExtensions.JsonValue(x.FkGroupdNavigation.DocumentTypeTitle, header.Language),
                        FkPersonId = x.FkPersonId,
                        PersonTitle = x.FkPersonId == null ? null : JsonExtensions.JsonValue(x.FkPerson.PersonTypeTitle, header.Language),
                        Status = x.Status
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> DocumentTypeGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TDocumentType
                    .AsNoTracking()
                    .CountAsync(x => (pagination.Id == 0 ? true : (x.FkGroupd == pagination.Id)) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.DocumentTitle, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<DocumentTypeGetDto> GetDocumentTypeById(int DocumentTypeId)
        {
            try
            {
                var data = await _context.TDocumentType
                .Include(x => x.FkGroupd)
                .Include(x => x.FkPerson)
                .Select(x => new DocumentTypeGetDto()
                {
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentTitle = JsonExtensions.JsonValue(x.DocumentTitle, header.Language),
                    FkGroupd = x.FkGroupd,
                    GroupTitle = JsonExtensions.JsonValue(x.FkGroupdNavigation.DocumentTypeTitle, header.Language),
                    FkPersonId = x.FkPersonId,
                    PersonTitle = x.FkPersonId == null ? null : JsonExtensions.JsonValue(x.FkPerson.PersonTypeTitle, header.Language),
                    Status = x.Status
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DocumentTypeId == DocumentTypeId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeAccept(AcceptDto accept)
        {
            try
            {
                var data =  await _context.TDocumentType.FindAsync(accept.Id);
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<int> DocumentTypeCountWithGroupId(int groupId , int personId)
        {
            try
            {
                return await _context.TDocumentType
                    .AsNoTracking()
                    .CountAsync(x => x.FkGroupd == groupId && x.FkPersonId == personId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



    }
}