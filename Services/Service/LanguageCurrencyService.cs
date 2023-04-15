using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Currency;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Language;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class LanguageCurrencyService : ILanguageCurrencyService
    {
        public ILanguageCurrencyRepository _languageCurrencyRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public LanguageCurrencyService(
        IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, ILanguageCurrencyRepository languageCurrencyRepository)
        {
            this._languageCurrencyRepository = languageCurrencyRepository;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<bool>> ChangeDefaultCurrency(int currencyId)
        {
            var result = await _languageCurrencyRepository.ChangeDefaultCurrency(currencyId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.ChangeDefaultCurrency));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> ChangeDefaultLanguage(int languageId)
        {
            var result = await _languageCurrencyRepository.ChangeDefaultLanguage(languageId);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.ChangeDefaultLanguage));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<List<CurrencyDto>>> GetAllCurrency()
        {
            var data = await _languageCurrencyRepository.GetAllCurrency();
            if (data == null)
            {
                return new ApiResponse<List<CurrencyDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CurrencyGetting));
            }
            return new ApiResponse<List<CurrencyDto>>(ResponseStatusEnum.Success,data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<LanguageDto>>> GetAllLanguage()
        {
            var data = await _languageCurrencyRepository.GetAllLanguage();
            if (data == null)
            {
                return new ApiResponse<List<LanguageDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.LanguageGetting));
            }
            return new ApiResponse<List<LanguageDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> RatesAgainstOneDollar(int currencyId, double rate)
        {
            var result = await _languageCurrencyRepository.RatesAgainstOneDollar(currencyId, rate);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.RatesAgainstOneDollar));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }
    }
}