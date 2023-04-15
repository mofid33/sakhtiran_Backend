using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;
namespace MarketPlace.API.Services.IService
{
    public interface IDocumentTypeService
    {
        Task<ApiResponse<DocumentTypeDto>> DocumentTypeAdd(DocumentTypeDto documentType);
        Task<ApiResponse<DocumentTypeDto>> DocumentTypeEdit(DocumentTypeDto documentType);
        Task<ApiResponse<bool>> DocumentTypeDelete(int id);
        Task<ApiResponse<bool>> DocumentTypeExist(int id);
        Task<ApiResponse<Pagination<DocumentTypeGetDto>>> DocumentTypeGetAll(PaginationDto pagination);
        Task<ApiResponse<DocumentTypeGetDto>> GetDocumentTypeById(int documentTypeId);
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept);
    }
}