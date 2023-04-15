using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class WebSliderRepository : IWebSliderRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public WebSliderRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<WebSlider> WebSliderAdd(WebSlider webSliderAdd)
        {
            try
            {
                var count = await _context.WebSlider.AsNoTracking().CountAsync(x => x.FkCategoryId == webSliderAdd.FkCategoryId);
                webSliderAdd.SequenceNumber = count + 1;
                webSliderAdd.ImageUrl = JsonExtensions.JsonAdd(webSliderAdd.ImageUrl, header);
                webSliderAdd.ExternalLinkUrl = JsonExtensions.JsonAdd(webSliderAdd.ExternalLinkUrl, header);
                webSliderAdd.ResponsiveImageUrl = JsonExtensions.JsonAdd(webSliderAdd.ResponsiveImageUrl, header);
                await _context.WebSlider.AddAsync(webSliderAdd);
                await _context.SaveChangesAsync();
                webSliderAdd.ImageUrl = JsonExtensions.JsonGet(webSliderAdd.ImageUrl, header);
                webSliderAdd.ExternalLinkUrl = JsonExtensions.JsonGet(webSliderAdd.ExternalLinkUrl, header);
                webSliderAdd.ResponsiveImageUrl = JsonExtensions.JsonGet(webSliderAdd.ResponsiveImageUrl, header);
                return webSliderAdd;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SliderExist(int id)
        {
            try
            {
                var exist = await _context.WebSlider.AsNoTracking().AnyAsync(x => x.SliderId == id);
                return exist;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<WebSlider>> SliderDelete(int id)
        {
            try
            {
                var data = await _context.WebSlider.FindAsync(id);
                var other = await _context.WebSlider.Where(x => x.FkCategoryId == data.FkCategoryId && x.SequenceNumber > data.SequenceNumber).ToListAsync();
                foreach (var item in other)
                {
                    item.SequenceNumber = item.SequenceNumber - 1;
                }
                await _context.SaveChangesAsync();
                _context.WebSlider.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<WebSlider>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return new RepRes<WebSlider>(Message.SliderDelete, false, null);
            }
        }

        public async Task<bool> ChangePrioritySlider(ChangePriorityDto changePriority)
        {
            try
            {
                var data = await _context.WebSlider.FirstAsync(x => x.SliderId == changePriority.Id);
                if (data.SequenceNumber > changePriority.PriorityNumber)
                {
                    var allSlider = await _context.WebSlider
                    .Where(x => x.FkCategoryId == data.FkCategoryId && x.SequenceNumber < data.SequenceNumber && x.SequenceNumber >= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allSlider)
                    {
                        item.SequenceNumber = item.SequenceNumber + 1;
                    }
                }
                else if (data.SequenceNumber < changePriority.PriorityNumber)
                {
                    var allSlider = await _context.WebSlider
                    .Where(x => x.FkCategoryId == data.FkCategoryId && x.SequenceNumber > data.SequenceNumber && x.SequenceNumber <= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allSlider)
                    {
                        item.SequenceNumber = item.SequenceNumber - 1;
                    }
                }
                else
                {

                }
                data.SequenceNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<WebSlider> SliderEdit(WebSlider webSlider)
        {
            try
            {
                var data = await _context.WebSlider.FindAsync(webSlider.SliderId);
                webSlider.ImageUrl = JsonExtensions.JsonEdit(webSlider.ImageUrl, data.ImageUrl, header);
                webSlider.ExternalLinkUrl = JsonExtensions.JsonEdit(webSlider.ExternalLinkUrl, data.ExternalLinkUrl, header);
                webSlider.ResponsiveImageUrl = JsonExtensions.JsonEdit(webSlider.ResponsiveImageUrl, data.ResponsiveImageUrl, header);
                _context.Entry(data).CurrentValues.SetValues(webSlider);
                await _context.SaveChangesAsync();
                webSlider.ImageUrl = JsonExtensions.JsonGet(webSlider.ImageUrl, header);
                webSlider.ExternalLinkUrl = JsonExtensions.JsonGet(webSlider.ExternalLinkUrl, header);
                webSlider.ResponsiveImageUrl = JsonExtensions.JsonGet(webSlider.ResponsiveImageUrl, header);
                return webSlider;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebSliderAddDto> UploadSliderImage(string fileName, string ResponsiveImageUrl, int id)
        {
            try
            {
                var data = await _context.WebSlider.FindAsync(id);
                var oldData = await _context.WebSlider.Select(x => new WebSliderAddDto()
                {
                    SliderId = x.SliderId,
                    ImageUrl = JsonExtensions.JsonValue( x.ImageUrl,header.Language),
                    ResponsiveImageUrl = JsonExtensions.JsonValue( x.ResponsiveImageUrl,header.Language) 
                }).FirstOrDefaultAsync(x => x.SliderId == id);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    data.ImageUrl = JsonExtensions.JsonEdit(fileName,data.ImageUrl, header);
                }
                if (!string.IsNullOrWhiteSpace(ResponsiveImageUrl))
                {
                    data.ResponsiveImageUrl = JsonExtensions.JsonEdit(ResponsiveImageUrl,data.ResponsiveImageUrl, header);
                }
                await _context.SaveChangesAsync();
                return oldData;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebSliderGetDto> SliderGetById(int id)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.WebSlider
                .Include(x => x.FkCollectionType)
                .AsNoTracking()
                .Select(x => new WebSliderGetDto()
                {
                    SliderId = x.SliderId,
                    SequenceNumber = x.SequenceNumber,
                    ImageUrl = JsonExtensions.JsonValue(x.ImageUrl,header.Language),
                    ExternalLinkUrl = JsonExtensions.JsonValue(x.ExternalLinkUrl,header.Language),
                    FkCollectionTypeId = x.FkCollectionTypeId,
                    CriteriaFrom = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaFrom  / rate) : x.CriteriaFrom,
                    CriteriaTo = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaTo  / rate) : x.CriteriaTo,
                    CriteriaType = x.CriteriaType,
                    FkCollectionType = x.FkCollectionTypeId != null ? new Dtos.WebModule.WebCollectionTypeDto(x.FkCollectionType) : null,
                    ResponsiveImageUrl = JsonExtensions.JsonValue(x.ResponsiveImageUrl,header.Language),
                    XitemIds = x.XitemIds,
                    FkCategoryId = x.FkCategoryId,
                    HaveLink = x.HaveLink
                })
                .FirstOrDefaultAsync(x => x.SliderId == id);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<WebSliderGetListDto>> SliderGet(int getType, decimal rate, int? categoryId)
        {
            // getType 1 = for website
            // getType 2 = for adminpanel
            // getType 2 = for category
            try
            {
                if (getType != 1)
                {
                    rate = (decimal)1.0;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal) currency.RatesAgainstOneDollar;
                    }
                }
                return await _context.WebSlider
                .Where(x => x.FkCategoryId == categoryId)
                .OrderBy(x => x.SequenceNumber)
                .Select(x => new WebSliderGetListDto()
                {
                    SliderId = x.SliderId,
                    SequenceNumber = x.SequenceNumber,
                    ImageUrl = JsonExtensions.JsonValue(x.ImageUrl,header.Language),
                    ExternalLinkUrl = JsonExtensions.JsonValue(x.ExternalLinkUrl,header.Language),
                    FkCollectionTypeId = x.FkCollectionTypeId,
                    CriteriaFrom = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaFrom  / rate) : x.CriteriaFrom,
                    CriteriaTo = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaTo  / rate) : x.CriteriaTo,
                    CriteriaType = x.CriteriaType,
                    ResponsiveImageUrl = JsonExtensions.JsonValue(x.ResponsiveImageUrl,header.Language),
                    XitemIds = x.XitemIds,
                    FkCategoryId = x.FkCategoryId,
                    HaveLink = x.HaveLink
                })
                .AsNoTracking()
                .ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> SliderCount(int? categotyId)
        {
            try
            {
                var count = await _context.WebSlider.AsNoTracking().CountAsync(x => x.FkCategoryId == categotyId);
                return count;
            }
            catch (System.Exception)
            {
                return 6;
            }
        }

    }
}