using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IAuthRepository
    {
        Task<TUser> UserGetByUsername(string username , int type);
        Task<string> GetCurrentUserUserName();
        Task<bool> UpdateLoginDate(Guid userId);
        Task<TUser> GetUserByUserId(Guid userId);
        Task<TUser> GetUserByOtherId(int id, UserGroupEnum type);
        Task<bool> UserExistByUsername(string username, Guid userId, Guid userGroup);
        Task<bool> ChangeUserPassword(TUser user);
        Task<TUser> UserAdd(TUser user);
        Task<TUser> UpdateUser(TUser user , bool changePass);

        Task<List<UserAccessDto>> GetEmployeeUsers(PaginationFormDto pagination);
        Task<int> GetEmployeeUsersCount(PaginationFormDto pagination);
        Task<bool> ChangeActiveEmployeUser(UserAcceptDto userAccept);
        Task<List<UserMenuDto>> GetMenu();
        Task<bool> AccessMenu(string path);
        Task<AuthWebDetailsDto> GetWebSiteAuthDetials();

        Task<bool> UpdateUserNotificationKey(UpdateUserNotificationKeyDto model, Guid userId);
        
    }
}