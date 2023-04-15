using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IDocumentTypeRepository
    {
        Task<TDocumentType> DocumentTypeAdd(TDocumentType DocumentType);
        Task<TDocumentType> DocumentTypeEdit(TDocumentType DocumentType);
        Task<RepRes<TDocumentType>> DocumentTypeDelete(int id);
        Task<bool> DocumentTypeExist(int id);
        Task<List<DocumentTypeGetDto>> DocumentTypeGetAll(PaginationDto pagination);
        Task<int> DocumentTypeGetAllCount(PaginationDto pagination);
        Task<DocumentTypeGetDto> GetDocumentTypeById(int DocumentTypeId);
        Task<bool> ChangeAccept(AcceptDto accept);
        Task<int> DocumentTypeCountWithGroupId(int groupId , int personId);
    }
}