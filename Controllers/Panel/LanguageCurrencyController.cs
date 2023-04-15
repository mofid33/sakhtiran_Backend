using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.Currency;
using MarketPlace.API.Data.Dtos.Language;
using Microsoft.AspNetCore.Authorization;
using MarketPlace.API.Data.Enums;
using System.Collections.Generic;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LanguageCurrencyController : ControllerBase
    {
        public ILanguageCurrencyService _languageCurrencyService { get; }

        public LanguageCurrencyController(ILanguageCurrencyService languageCurrencyService)
        {
            this._languageCurrencyService = languageCurrencyService;
        }


        [Authorize(Roles = "Admin")]

        [HttpGet("Language")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<LanguageDto>>))]
        public async Task<IActionResult> GetAllLanguage()
        {
            var result = await _languageCurrencyService.GetAllLanguage();
            return new Response<List<LanguageDto>>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin,Seller")]

        [HttpGet("Currency")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CurrencyDto>>))]
        public async Task<IActionResult> GetAllCurrency()
        {
            var result = await _languageCurrencyService.GetAllCurrency();
            return new Response<List<CurrencyDto>>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("ChangeDefaultLanguage/{languageId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeDefaultLanguage([FromRoute] int languageId)
        {
            var result = await _languageCurrencyService.ChangeDefaultLanguage(languageId);
            return new Response<bool>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("ChangeDefaultCurrency/{currencyId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeDefaultCurrency([FromRoute] int currencyId)
        {
            var result = await _languageCurrencyService.ChangeDefaultCurrency(currencyId);
            return new Response<bool>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("RatesAgainstOneDollar/{currencyId}/{rate}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> RatesAgainstOneDollar([FromRoute] int currencyId, [FromRoute] double rate)
        {
            var result = await _languageCurrencyService.RatesAgainstOneDollar(currencyId, rate);
            return new Response<bool>().ResponseSending(result);
        }
    }
}