using DataAccess.EntityFramework;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserAccountService : BaseService
    {
        private readonly FileCryptoContext Context;

        public UserAccountService(ServiceDependencies dependencies)
            : base(dependencies)
        {
            Context = new FileCryptoContext();
        }

        public async Task<bool> CheckEmailAvailability(string email)
        {
            return await Context.Users.FirstOrDefaultAsync(c => c.Email == email) == null;
        }

        public async Task<bool> CheckUsernameAvailability(string userName) => await Context.Users.FirstOrDefaultAsync(c => c.UserName == userName) == null;

        public async Task RegisterNewUser(RegisterModel model)
        {

            ExecuteInTransaction(uow =>
            {
                using MemoryStream memory = new();
                var user = Mapper.Map<User>(model);
                user.UserId = Guid.NewGuid();
                user.AuthToken = Encrypter_Decrypter.EncodePasswordToBase64(user.Email) + Encrypter_Decrypter.EncodePasswordToBase64(user.UserName);
                user.IsValidated = false;

                uow.Users.Insert(user);
                uow.SaveChanges();
            });
        }

        public object GenerateEmailConfirmationToken(RegisterModel model)
        {
            return Encrypter_Decrypter.EncodePasswordToBase64(model.Email) + Encrypter_Decrypter.EncodePasswordToBase64(model.UserName);
        }

        public async Task<bool> CheckEmail(string email) => await Context.Users.AnyAsync(x => x.Email == email);

        public async Task<bool> CheckUserCredentials(string email, string password) => await Context.Users.AnyAsync(x => x.Email == email && x.Password == password);

        public async Task<bool> CheckValidity(string email)
        {
            return await Context.Users.AnyAsync(x => x.Email == email && x.IsValidated == true);
        }

        public async Task<CurrentUserDto> LoginAsync(string email, string password)
        {
            var user = await UnitOfWork.Users.Get().FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                return new CurrentUserDto { IsAuthenticated = false };
            }
            var userDto = Mapper.Map<CurrentUserDto>(user);
            userDto.IsAuthenticated = true;
            return userDto;
        }

        public async Task<UserProfileModel> GetUserProfileModel(Guid id)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            var usermodel = new UserProfileModel();
            Mapper.Map<User, UserProfileModel>(user, usermodel);
            return usermodel;
        }
        public async Task EditUser(UserProfileModel model)
        {
            var userUpdate = await Context.Users
              .FirstOrDefaultAsync(x => x.UserId == model.UserId);
            ExecuteInTransaction(uow =>
            {
                if (userUpdate != null)
                {
                    Mapper.Map<UserProfileModel, User>(model, userUpdate);
                    uow.Users.Update(userUpdate);
                }
                uow.SaveChanges();
            });
        }

        public async Task<bool> CheckEmailAvailability(string email1, string email2) => email1 == email2 || !(await Context.Users.AnyAsync(x => x.Email == email1));

        public async Task<bool> CheckUsernameAvailability(string userName1, string userName2) => userName1 == userName2 || !(await Context.Users.AnyAsync(x => x.UserName == userName1));

        public async Task<List<SelectListItem>> ReturnUsersForFileUpload(string search)
        {
            return await Context.Users.Where(x => x.UserName != null && x.UserId != CurrentUser.UserId && x.UserName.StartsWith(search)).Take(10).Select(x => new SelectListItem()
            {
                Text = x.UserName,
                Value = x.UserId.ToString()
            }).Distinct().ToListAsync();
        }

        public async Task<User> GetUserEmailById(Guid id)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }
    }
}
