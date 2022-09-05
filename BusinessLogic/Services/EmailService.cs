using DataAccess.EntityFramework;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class EmailService : BaseService
    {
        private readonly FileCryptoContext Context;
        public EmailService(ServiceDependencies dependencies)
           : base(dependencies)
        {
            Context = new FileCryptoContext();
        }

        public async Task<User> FindByEmailAsync(string email) => await Context.Users.FirstAsync(x => x.Email == email);

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {

            var user = await Context.Users.FirstOrDefaultAsync(x => x.Email == email && x.AuthToken == token);
            if (user == null)
            {
                return false;
            }
            user.IsValidated = true;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<RegisterModel?> ConfirmEmailPasswordAsync(string email, string token)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Email == email && x.PasswordToken == token && x.IsValidated == true);
            if (user == null)
            {
                return null;
            }
            ExecuteInTransaction(uow =>
            {
                user.PasswordToken = String.Empty;
                Context.Users.Update(user);
                uow.SaveChanges();
            });
            return new RegisterModel
            {
                UserId = user.UserId

            };
        }
    }
}
