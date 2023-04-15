using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class HelpRepository : IHelpRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public HelpRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<THelpTopic> AddHelpTopic(THelpTopic helpTopicDto)
        {
            try
            {
                helpTopicDto.Title = JsonExtensions.JsonAdd(helpTopicDto.Title, header);
            
                helpTopicDto.Description = JsonExtensions.JsonAdd(helpTopicDto.Description, header);
                
               

                await _context.THelpTopic.AddAsync(helpTopicDto);
                await _context.SaveChangesAsync();
                return helpTopicDto;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> EditHelpTopic(THelpTopic helpTopicDto)
        {
            try
            {
                var data = await _context.THelpTopic.Include(x => x.InverseFkTopic).FirstAsync(x => x.TopicId == helpTopicDto.TopicId);
                if (data.FkTopicId == null && data.InverseFkTopic.Count > 0 && helpTopicDto.FkTopicId != null)
                {
                    return false;
                }

                helpTopicDto.Title = JsonExtensions.JsonEdit(helpTopicDto.Title, data.Title, header);
                if(helpTopicDto.FkTopicId == null)
                {
                    helpTopicDto.Description = JsonExtensions.JsonEdit(helpTopicDto.Description, data.Description, header);
                }
                else
                {
                    helpTopicDto.Description = null;
                }

                _context.Entry(data).CurrentValues.SetValues(helpTopicDto);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<HelpTopicListDto>> GetHelpTopicList(PaginationFormDto pagination)
        {
            try
            {
                // hatmn baresi shavad
                //  x.InverseFkTopic.sum(t=>t.THelpArticle.count())
                return await _context.THelpTopic
                .Include(x => x.FkTopic)
                .Include(x => x.THelpArticle)
                .Include(x => x.InverseFkTopic).ThenInclude(p => p.THelpArticle)
                .OrderByDescending(x => x.TopicId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new HelpTopicListDto()
                {
                    ArticlesCount = x.THelpArticle.Count + x.InverseFkTopic.SelectMany(h => h.THelpArticle).Count(),
                    FkTopicId = x.FkTopicId,
                    IconUrl = x.IconUrl,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    TopicId = x.TopicId,
                    Location = x.FkTopic == null ? null : JsonExtensions.JsonValue(x.FkTopic.Title, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetHelpTopicListCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.THelpTopic.AsNoTracking().CountAsync();
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<HelpTopicGetDto> GetHelpTopicById(int topicId)
        {
            try
            {
                var data =  await _context.THelpTopic
                .Include(x => x.FkTopic)
                .Select(x => new HelpTopicGetDto()
                {
                    FkTopicId = x.FkTopicId,
                    IconUrl = x.IconUrl,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    TopicId = x.TopicId,
                    FkTopicTitle = x.FkTopic == null ? null : JsonExtensions.JsonValue(x.FkTopic.Title, header.Language),
                    Description = x.Description,
                    Status = x.Status
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.TopicId == topicId);
                data.Description = JsonExtensions.JsonGet(data.Description , header) ;
                return data ;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddHelpArticle(THelpArticle helpArticleDto)
        {
            try
            {
                if(await _context.THelpTopic.AnyAsync(x=>x.FkTopicId== null && x.TopicId == helpArticleDto.FkTopicId))
                {
                    return false;
                }
                helpArticleDto.Subject = JsonExtensions.JsonAdd(helpArticleDto.Subject, header);
                helpArticleDto.Description = JsonExtensions.JsonAdd(helpArticleDto.Description, header);
                helpArticleDto.LastUpdateDateTime = System.DateTime.Now;

                await _context.THelpArticle.AddAsync(helpArticleDto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }





        public async Task<bool> EditHelpArticle(THelpArticle helpArticleDto)
        {
            try
            {
                if(await _context.THelpTopic.AnyAsync(x=>x.FkTopicId== null && x.TopicId == helpArticleDto.FkTopicId))
                {
                    return false;
                }
                var data = await _context.THelpArticle.FirstAsync(x => x.ArticleId == helpArticleDto.ArticleId);

                helpArticleDto.Subject = JsonExtensions.JsonEdit(helpArticleDto.Subject, data.Subject, header);
                helpArticleDto.Description = JsonExtensions.JsonEdit(helpArticleDto.Description, data.Description, header);
                helpArticleDto.LastUpdateDateTime = System.DateTime.Now;
                _context.Entry(data).CurrentValues.SetValues(helpArticleDto);
                _context.Entry(data).Property(x => x.HelpfulCount).IsModified = false;
                _context.Entry(data).Property(x => x.UnhelpfulCount).IsModified = false;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<HelpArticleListDto>> GetHelpArticleList(PaginationFormDto pagination)
        {
            try
            {
                return await _context.THelpArticle
                .Include(x => x.FkTopic)
                .OrderByDescending(x => x.LastUpdateDateTime)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new HelpArticleListDto()
                {
                    FkTopicId = x.FkTopicId,
                    Subject = JsonExtensions.JsonValue(x.Subject, header.Language),
                    ArticleId = x.ArticleId,
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    FkTopicTitle = JsonExtensions.JsonValue(x.FkTopic.Title, header.Language),
                    HelpfulCount = x.HelpfulCount,
                    LastUpdateDateTime = Extentions.PersianDateString(x.LastUpdateDateTime),
                    Status = x.Status,
                    UnhelpfulCount = x.UnhelpfulCount
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetHelpArticleListCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.THelpArticle.AsNoTracking().CountAsync();
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<HelpArticleGetDto> GetHelpArticleById(int articleId)
        {
            try
            {
                var data =  await _context.THelpArticle
                .Include(x => x.FkTopic)
                .Select(x => new HelpArticleGetDto()
                {
                    FkTopicId = x.FkTopicId,
                    Subject = JsonExtensions.JsonValue(x.Subject, header.Language),
                    ArticleId = x.ArticleId,
                    Description = x.Description,
                    FkTopicTitle = JsonExtensions.JsonValue(x.FkTopic.Title, header.Language),
                    Status = x.Status,
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ArticleId == articleId);

                data.Description = JsonExtensions.JsonGet(data.Description , header) ;

                return data ;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<HelpArticleFormDto>> HelpAutoComplete(string search)
        {
            try
            {
                return await _context.THelpArticle.Where(x=>x.Status == true && JsonExtensions.JsonValue(x.Subject,header.Language).Contains(search))
                .OrderByDescending(x=>x.HelpfulCount)
                .Take(6)
                .Include(x=>x.FkTopic).ThenInclude(p=>p.FkTopic)
                .Select(x=> new HelpArticleFormDto(){
                    ArticleId = x.ArticleId,
                    Subject = JsonExtensions.JsonValue(x.Subject,header.Language),
                    Topic = JsonExtensions.JsonValue(x.FkTopic.FkTopic.Title,header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<HomeHelpTopicListDto>> GetHomeHelpTopic()
        {
            try
            {
                return await _context.THelpTopic.Where(x=>x.Status == true && x.FkTopicId == null )
                
                .Include(x=>x.InverseFkTopic).ThenInclude(p=>p.THelpArticle)
                .Select(x=> new HomeHelpTopicListDto(){
                    ArticleCount =  x.InverseFkTopic.Where(t=>t.Status == true).SelectMany(h => h.THelpArticle.Where(p=>p.Status == true)).Count(),
                    Description = JsonExtensions.JsonValue(x.Description,header.Language),
                    IconUrl = x.IconUrl,
                    Title = JsonExtensions.JsonValue(x.Title,header.Language),
                    TopicId = x.TopicId
                })
                .AsNoTracking().OrderBy(o=>o.Title).ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<HomeHelpTopicChildDto> GetHelpTopic(int topicId)
        {
            try
            {
                return await _context.THelpTopic
                .Include(x=>x.FkTopic)
                .Where(x=>x.Status == true && x.TopicId == topicId && x.FkTopicId != null && x.FkTopic.Status == true)
                .Include(x=>x.THelpArticle)
                .Select(x=> new HomeHelpTopicChildDto(){
                    ArticleCount = x.THelpArticle.Where(x=>x.Status == true).Count(),
                    Articles = x.THelpArticle.Where(x=>x.Status == true).Select(t=> new HelpArticleFormDto(){
                        ArticleId = t.ArticleId,
                        Subject = JsonExtensions.JsonValue(t.Subject,header.Language),
                        Topic = JsonExtensions.JsonValue(x.Title,header.Language),
                    }).ToList(),
                    Title = JsonExtensions.JsonValue(x.Title,header.Language),
                    TopicId = x.TopicId,
                    TopicParentId = (int)x.FkTopicId,
                    TopicParentTitle = JsonExtensions.JsonValue(x.FkTopic.Title,header.Language)

                })
                .AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }        
        
        public async Task<HomeHelpTopicDto> GetHelpParentTopic(int topicId)
        {
            try
            {
                var data = await _context.THelpTopic.Where(x=>x.Status == true && x.TopicId == topicId )
                .Include(x=>x.InverseFkTopic).ThenInclude(t=>t.THelpArticle)
                .Select(l=> new HomeHelpTopicDto(){
                    ArticleCount =  l.InverseFkTopic.Where(t=>t.Status == true).SelectMany(h => h.THelpArticle.Where(p=>p.Status == true)).Count(),
                    Description = l.Description,
                    IconUrl = l.IconUrl,
                    Title = JsonExtensions.JsonValue(l.Title,header.Language),
                    TopicId = l.TopicId,
                    Childs = l.InverseFkTopic.Where(t => t.Status == true).Select(x => new HomeHelpTopicChildDto()
                    {
                        ArticleCount = x.THelpArticle.Where(x => x.Status == true).Count(),
                        Articles = x.THelpArticle.Where(x => x.Status == true).Select(t => new HelpArticleFormDto()
                        {
                            ArticleId = t.ArticleId,
                            Subject = JsonExtensions.JsonValue(t.Subject, header.Language),
                            Topic = JsonExtensions.JsonValue(x.Title, header.Language),
                        }).ToList(),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        TopicId = x.TopicId,
                        TopicParentId = l.TopicId,
                        TopicParentTitle = JsonExtensions.JsonValue(l.Title,header.Language),

                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync();

                data.Description = JsonExtensions.JsonGet(data.Description , header) ;

                return data ;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<HomeHelpArticleDto> GetHelpArticle(int articleId)
        {
            try
            {
                var data = await _context.THelpArticle
                .Include(x=>x.FkTopic).ThenInclude(t=>t.FkTopic)
                .Where(x=>x.Status == true && x.FkTopic.Status == true && x.FkTopic.FkTopic.Status == true && x.ArticleId == articleId)
                .Include(x=>x.FkTopic).ThenInclude(t=>t.THelpArticle)
                .Select(x=> new HomeHelpArticleDto(){
                    ArticleId = x.ArticleId,
                    Articles = x.FkTopic.THelpArticle.Where(t=>t.Status == true).Select(t=>new HelpArticleFormDto(){
                        ArticleId = t.ArticleId,
                        Subject = JsonExtensions.JsonValue(t.Subject,header.Language),
                        Topic = JsonExtensions.JsonValue(x.FkTopic.Title,header.Language),
                    }).ToList(),
                    Description = x.Description,
                    ParentTopicId = x.FkTopic.FkTopicId,
                    ParentTopicTitle = JsonExtensions.JsonValue(x.FkTopic.FkTopic.Title,header.Language),
                    Subject = JsonExtensions.JsonValue(x.Subject,header.Language),
                    TopicId = x.FkTopicId,
                    TopicTitle = JsonExtensions.JsonValue(x.FkTopic.Title,header.Language),

                })
                .AsNoTracking().FirstOrDefaultAsync();

                data.Description = JsonExtensions.JsonGet(data.Description , header) ;

                return data ;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddHelpFul(AcceptDto accept)
        {
            try
            {
                var data = await _context.THelpArticle.FirstAsync(x=>x.Status == true && x.ArticleId == accept.Id);
                data.LastUpdateDateTime = System.DateTime.Now;
                if(accept.Accept == true)
                {
                    data.HelpfulCount = data.HelpfulCount + 1;
                }
                else
                {
                     data.UnhelpfulCount = data.UnhelpfulCount + 1;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    
    
        public async Task<RepRes<THelpTopic>> TopicDelete(int id)
        {
            try
            {
                var data = await _context.THelpTopic.FirstOrDefaultAsync(x => x.TopicId == id);
                if (data == null)
                {
                    return new RepRes<THelpTopic>(Message.HelpTopicGetting, false, null);
                }
                var hasRelation = await _context.THelpArticle.AsNoTracking().AnyAsync(x => x.FkTopicId == id);
                if (hasRelation)
                {
                    return new RepRes<THelpTopic>(Message.HelpTopicCantDelete, false, null);
                }
                 var hasRelation2 = await _context.THelpTopic.AsNoTracking().AnyAsync(x => x.FkTopicId == id);
                if (hasRelation2)
                {
                    return new RepRes<THelpTopic>(Message.HelpTopicCantDelete, false, null);
                }

                _context.THelpTopic.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<THelpTopic>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }
         public async Task<RepRes<THelpArticle>> ArticleDelete(int id)
        {
            try
            {
                var data = await _context.THelpArticle.FirstOrDefaultAsync(x => x.ArticleId == id);
                if (data == null)
                {
                    return new RepRes<THelpArticle>(Message.HelpArticleGetting, false, null);
                }
              
                _context.THelpArticle.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<THelpArticle>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<string> GetHelpImage()
        {
            try
            {
                var data = await _context.TSetting.FirstAsync();
            
              
                return data.HelpPageBackgroundImage;
            }
            catch (System.Exception)
            {
                return null;
            }        
        }
    }
}