using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class AccountingService : IAccountingService
    {
        public IMapper _mapper { get; }
        public IAccountingRepository _accountingRepository { get; }
        public ICustomerRepository _customerRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public AccountingService(IMapper mapper,
         IAccountingRepository accountingRepository,
         ICustomerRepository customerRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        IFileUploadService fileUploadService
         )
        {
            _fileUploadService =fileUploadService;
            this._accountingRepository = accountingRepository;
            this._customerRepository = customerRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._ms = ms;
        }
        public async Task<ApiResponse<AccountingGetDto>> GetAccountingList(AccountingListPaginationDto pagination)
        {
            var data = new AccountingGetDto();
            if(token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            data.Transaction = await _accountingRepository.GetAccountingList(pagination);
            if (data.Transaction == null)
            {
                return new ApiResponse<AccountingGetDto>(ResponseStatusEnum.BadRequest, data, _ms.MessageService(Message.AccountingGetting));
            }
            data.Count = await _accountingRepository.GetAccountingListCount(pagination);
            var total = await _accountingRepository.GetAccountingListTotal(pagination);
            data.Total = (double) total;
            return new ApiResponse<AccountingGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<ShopWithdrawalRequestGetDto>>> GetShopWithdrawalRequestList(ShopWithdrawalRequestPaginationDto pagination)
        {
            if (token.Rule == UserGroupEnum.Seller)
            {
                pagination.ShopId = token.Id;
            }
            var data = await _accountingRepository.GetShopWithdrawalRequestList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopWithdrawalRequestGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopWithdrawalRequestGetting));
            }
            var count = await _accountingRepository.GetShopWithdrawalRequestListCount(pagination);
            return new ApiResponse<Pagination<ShopWithdrawalRequestGetDto>>(ResponseStatusEnum.Success,new Pagination<ShopWithdrawalRequestGetDto>(count,data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopWithdrawalRequestDto>> EditShopWithdrawalRequest(ShopWithdrawalSerializeDto serializeDto)
        {
           var ShopWithdrawalObj = Extentions.Deserialize<ShopWithdrawalRequestDto>(serializeDto.ShopWithdrawal);
            if (ShopWithdrawalObj == null)
            {
                return new ApiResponse<ShopWithdrawalRequestDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.BrandDeserialize));
            }
            var FileName = "";
            if (serializeDto.Document != null)
            {
                FileName = _fileUploadService.UploadImage(serializeDto.Document, Pathes.ShopWithdrawal + ShopWithdrawalObj.RequestId + "/");
                if (FileName == null)
                {
                    return new ApiResponse<ShopWithdrawalRequestDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.UploadFile));
                }
                ShopWithdrawalObj.DocumentUrl = FileName;
            }
            var Data = await _accountingRepository.EditShopWithdrawalRequest(ShopWithdrawalObj);
            if (Data.Result == false)
            {
                if (serializeDto.Document != null)
                {
                    _fileUploadService.DeleteImage(FileName,Pathes.ShopWithdrawal + ShopWithdrawalObj.RequestId + "/");
                }
                return new ApiResponse<ShopWithdrawalRequestDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Data.Message));
            }
            return new ApiResponse<ShopWithdrawalRequestDto>(ResponseStatusEnum.Success, ShopWithdrawalObj,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> AddShopWithdrawalRequest(ShopAddWithdrawalRequestDto request)
        {
            var data = await _accountingRepository.AddShopWithdrawalRequest(request);
            if(data.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(data.Message));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(data.Message));
        }

        public async Task<ApiResponse<bool>> AddCustomerWithdrawalRequest(CustomerAddWithdrawalRequestDto request)
        {
            var customer = await _customerRepository.GetCustomerGeneralDetail(request.CustomerId);
            if(customer.Credit < request.Amount) {
               return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.CustomerWithdrawalRequestEdit));
            }
            var data = await _accountingRepository.AddTransaction((int) TransactionTypeEnum.Withdraw , customer.UserId , null , null , null , (int)TransactionStatusEnum.Completed , request.Amount , request.RequestText);
            if(data == false)
            {
               return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.CustomerWithdrawalRequestEdit));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<decimal>> GetShopBalance()
        {
            var data = await _accountingRepository.GetShopBalance();
            return new ApiResponse<decimal>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}