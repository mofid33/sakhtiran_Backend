using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos;
using System.Linq;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using System;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Token;
namespace MarketPlace.API.Data.Repositories.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }

        public AuthRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);

        }

        public async Task<TUser> UserGetByUsername(string username, int type)
        {
            try
            {
                var group = "";
                var checkSeller = false;

                if (type == (int)UserGroupEnum.Customer)
                {
                    group = GroupTypes.Customer;
                }
                if (type == (int)UserGroupEnum.Admin)
                {
                    group = GroupTypes.Admin;
                    checkSeller = true;
                }
                if (type == (int)UserGroupEnum.Seller)
                {
                    group = GroupTypes.Seller;
                }
                var user = await _context.TUser.AsNoTracking()
               .Include(c => c.FkCustumer)
               .FirstOrDefaultAsync(x =>
               x.UserName == username &&
               x.FkUserGroupId.ToString() == group);

                if (user != null)
                {
                    return user;
                }
                else
                {
                    if (checkSeller)
                    {
                    return await _context.TUser.AsNoTracking()
                   .Include(c => c.FkShop)
                   .FirstOrDefaultAsync(x =>
                   x.UserName == username &&
                   x.FkUserGroupId.ToString() == GroupTypes.Seller);
                    }
                    else {
                    return user;
                    }
                }

            }
            catch (System.Exception)
            {
                return null;
            }

        }



        public async Task<bool> UpdateLoginDate(Guid userId)
        {
            try
            {
                var user = await _context.TUser.FindAsync(userId);
                user.LastLoginDatetime = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TUser> GetUserByUserId(Guid userId)
        {
            try
            {
                return await _context.TUser
                .Include(x => x.FkCustumer)
                .Include(x => x.FkShop)
                .Include(x => x.FkUserGroup)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TUser> GetUserByOtherId(int id, UserGroupEnum type)
        {
            try
            {
                return await _context.TUser
                .Include(x => x.FkCustumer)
                .Include(x => x.FkShop)
                .Include(x => x.FkUserGroup)
                .FirstOrDefaultAsync(x =>
                (type == UserGroupEnum.Customer ? x.FkCustumerId == id : true) &&
                (type == UserGroupEnum.Seller ? x.FkShopId == id : true)
                );
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> UserExistByUsername(string username, Guid userId , Guid userGroup)
        {
            try
            {
                return await _context.TUser.AsNoTracking().AnyAsync(x => x.UserId != userId && x.UserName == username && x.FkUserGroupId == userGroup);
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        public async Task<bool> ChangeUserPassword(TUser user)
        {
            try
            {
                var data = await _context.TUser.FindAsync(user.UserId);
                data.UserName = user.UserName;
                data.PasswordHash = user.PasswordHash;
                data.PasswordSalt = user.PasswordSalt;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TUser> UserAdd(TUser user)
        {
            try
            {
                await _context.TUser.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TUser> UpdateUser(TUser user, bool changePass)
        {
            try
            {
                var data = await _context.TUser.FindAsync(user.UserId);
                if (data == null)
                {
                    return null;
                }
                var access = await _context.TUserAccessControl.Where(x => x.FkUserId == user.UserId).ToListAsync();
                _context.TUserAccessControl.RemoveRange(access);
                await _context.SaveChangesAsync();
                await _context.TUserAccessControl.AddRangeAsync(user.TUserAccessControl);
                data.UserName = user.UserName;
                if (changePass == true)
                {
                    data.PasswordSalt = user.PasswordSalt;
                    data.PasswordHash = user.PasswordHash;
                }
                await _context.SaveChangesAsync();

                return user;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<UserAccessDto>> GetEmployeeUsers(PaginationFormDto pagination)
        {
            try
            {
                var user = await _context.TUser
                .Where(x => x.FkUserGroupId.ToString() == GroupTypes.Admin)
                .Include(x => x.TUserAccessControl)
                .Select(x => new UserAccessDto()
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    Active = x.Active,
                    IsAdmin = x.UserId == Guid.Parse(UserAdminId.ID) ? true : false,
                    TUserAccessControl = x.TUserAccessControl.Select(t => new AccessControlDto()
                    {
                        UserAccessControlId = t.UserAccessControlId,
                        FkMenuItemId = t.FkMenuItemId,
                        FkUserId = t.FkUserId,
                        MenuItem = JsonExtensions.JsonValue(t.FkMenuItem.Title, header.Language),
                    }).ToList()
                })
                .OrderByDescending(x => x.UserId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .AsNoTracking().ToListAsync();
                return user;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetEmployeeUsersCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TUser
                .AsNoTracking()
                .CountAsync(x => x.FkUserGroupId.ToString() == GroupTypes.Admin);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeActiveEmployeUser(UserAcceptDto userAccept)
        {
            try
            {
                var Accept = await _context.TUser.FirstOrDefaultAsync(x => x.FkUserGroupId.ToString() == GroupTypes.Admin && x.UserId == userAccept.Id);
                if (Accept == null)
                {
                    return false;
                }
                Accept.Active = userAccept.Active;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<List<UserMenuDto>> GetMenu()
        {
            try
            {
                var currentUser = await _context.TUser.FirstAsync(x => x.UserId == token.UserId);
                bool isAdmin = false;
                if (currentUser.UserId == Guid.Parse(UserAdminId.ID))
                {
                    isAdmin = true;
                }
                var menuid = await _context.TUserAccessControl.Where(x => x.FkUserId == token.UserId)
                .AsNoTracking()
                .Select(x => x.FkMenuItemId).ToListAsync();
                var data = await _context.TMenuItem.Include(x => x.InverseParent)
                .Where(x => x.ParentId == null && (isAdmin ? true : (menuid.Contains(x.MenuId) || x.InverseParent.Any(t => menuid.Contains(t.MenuId)))))
                .AsNoTracking()
                .Select(x => new UserMenuDto()
                {
                    MenuId = x.MenuId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    Url = x.Url,
                    Expanded = x.Expanded,
                    ParentId = x.ParentId,
                    Child = x.InverseParent.Where(t => isAdmin ? true : (menuid.Contains(t.MenuId) || menuid.Contains((int)t.ParentId)))
                    .Select(t => new UserMenuDto()
                    {
                        MenuId = t.MenuId,
                        Title = JsonExtensions.JsonValue(t.Title, header.Language),
                        Url = t.Url,
                        Expanded = t.Expanded,
                        ParentId = x.ParentId,
                        Child = null
                    }).ToList()
                }).ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AccessMenu(string path)
        {
            try
            {
                var currentUser = await _context.TUser.FirstAsync(x => x.UserId == token.UserId);
                if (currentUser.UserId == Guid.Parse(UserAdminId.ID))
                {
                    return true;
                }
                else
                {
                    var data = await _context.TUserAccessControl.Include(u => u.FkMenuItem).FirstOrDefaultAsync(x => x.FkUserId == token.UserId && x.FkMenuItem.Url == "/" + path);
                    if (data == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
            catch (System.Exception)
            {
                return false;

            }
        }

        public async Task<AuthWebDetailsDto> GetWebSiteAuthDetials()
        {
            try
            {

                var data = await _context.TSetting.FirstOrDefaultAsync();
                var result = new AuthWebDetailsDto();
                if (data != null)
                {
                    result.BackgroundImage = data.CustomerLoginPageBackgroundImage;
                    result.Logo = data.LogoUrlLoginPage;
                }

                return result;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<string> GetCurrentUserUserName()
        {
            try
            {
                var currentUser = await _context.TUser.FirstAsync(x => x.UserId == token.UserId);
                return currentUser.UserName;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<bool> UpdateUserNotificationKey(UpdateUserNotificationKeyDto model, Guid userId)
        {
            try
            {
                var currentUser = await _context.TUser.FirstAsync(x => x.UserId == userId);
                switch (model.Type)
                {
                    case (int)UpdateUserNotificationKeyTypeEnum.ClientWebFirebasePushNotificationKey:
                        currentUser.ClientWebFirebasePushNotificationKey = model.NotificationKey;
                        break;
                    case (int)UpdateUserNotificationKeyTypeEnum.ClientMobileFirebasePushNotificationKey:
                        currentUser.ClientMobileFirebasePushNotificationKey = model.NotificationKey;
                        break;
                    case (int)UpdateUserNotificationKeyTypeEnum.ProviderFirebasePushNotificationKey:
                        currentUser.ProviderFirebasePushNotificationKey = model.NotificationKey;
                        break;
                    default:
                        break;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}