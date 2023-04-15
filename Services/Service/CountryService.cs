using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class CountryService : ICountryService
    {
        public IMapper _mapper { get; }
        public ICountryRepository _countryRepository { get; }
        public IFileUploadService _fileUploadService { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }


        public CountryService(
        IMapper mapper,
        ICountryRepository countryRepository,
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms,
        IFileUploadService fileUploadService)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._countryRepository = countryRepository;
            this._mapper = mapper;
            _ms = ms;
            this._fileUploadService = fileUploadService;
        }
        public async Task<ApiResponse<CountryDto>> CountryAdd(CountrySerializeDto CountryDto)
        {
            var CountryObj = Extentions.Deserialize<CountryDto>(CountryDto.Country);
            if (CountryObj == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryDeserialize));
            }
            if (CountryDto.Image == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            var CountryFileName = "";

            CountryFileName = _fileUploadService.UploadImage(CountryDto.Image, Pathes.CountryImgTemp);
            if (CountryFileName == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
            }
            CountryObj.FlagUrl = CountryFileName;

            var mapCountry = _mapper.Map<TCountry>(CountryObj);
            var craetedCountry = await _countryRepository.CountryAdd(mapCountry);
            if (craetedCountry == null)
            {
                if (CountryDto.Image != null)
                {
                    _fileUploadService.DeleteImage(CountryFileName, Pathes.CountryImgTemp);
                }
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryAdding));
            }
            if (CountryDto.Image != null)
            {
                var isMoved = _fileUploadService.ChangeDestOfFile(CountryFileName, Pathes.CountryImgTemp, Pathes.Country + craetedCountry.CountryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(CountryFileName, Pathes.CountryImgTemp);
                }
            }
            var mapCraetedCountry = _mapper.Map<CountryDto>(craetedCountry);
            return new ApiResponse<CountryDto>(ResponseStatusEnum.Success, mapCraetedCountry, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> CountryDelete(int id)
        {
            var result = await _countryRepository.CountryDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                _fileUploadService.DeleteDirectory(Pathes.Country + result.Data.CountryId);
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<CountryDto>> CountryEdit(CountrySerializeDto CountryDto)
        {
            var CountryObj = Extentions.Deserialize<CountryDto>(CountryDto.Country);
            if (CountryObj == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryDeserialize));
            }
            var exist = await this.CountryExist(CountryObj.CountryId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<CountryDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var fileName = "";
            if (CountryDto.Image != null)
            {
                fileName = _fileUploadService.UploadImage(CountryDto.Image, Pathes.CountryImgTemp);
                if (fileName == null)
                {
                    return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UploadFile));
                }
                var isMoved = _fileUploadService.ChangeDestOfFile(fileName, Pathes.CountryImgTemp, Pathes.Country + CountryObj.CountryId + "/");
                if (!isMoved)
                {
                    _fileUploadService.DeleteImage(fileName, Pathes.CountryImgTemp);
                }
                _fileUploadService.DeleteImage(CountryObj.FlagUrl, Pathes.Country + CountryObj.CountryId + "/");
                CountryObj.FlagUrl = fileName;
            }
            var mapCountry = _mapper.Map<TCountry>(CountryObj);
            var editedCountry = await _countryRepository.CountryEdit(mapCountry);
            if (editedCountry == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryEditing));
            }
            var mapEditedCountry = _mapper.Map<CountryDto>(editedCountry);
            return new ApiResponse<CountryDto>(ResponseStatusEnum.Success, mapEditedCountry, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> CountryExist(int id)
        {
            var result = await _countryRepository.CountryExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.CountryNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<CountryDto>>> CountryGetAll(PaginationDto pagination)
        {
            var data = await _countryRepository.CountryGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CountryDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryGetting));
            }
            var count = await _countryRepository.CountryGetAllCount(pagination);
            return new ApiResponse<Pagination<CountryDto>>(ResponseStatusEnum.Success, new Pagination<CountryDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CountryDto>> GetCountryById(int CountryId)
        {
            var Country = await _countryRepository.GetCountryById(CountryId);
            if (Country == null)
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryGetting));
            }
            else
            {
                return new ApiResponse<CountryDto>(ResponseStatusEnum.Success, Country, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept)
        {
            var data = await _countryRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.CountryChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}