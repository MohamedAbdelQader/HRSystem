using HRSystem.Models;
using HRSystem.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class HRRepository : GeneralRepositories<User>
    {
        private readonly UserManager<User> UserManager;
        private readonly SignInManager<User> SignInManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        

        public HRRepository(RoleManager<IdentityRole> _RoleManager, HRContext Context, UserManager<User> userManager
            , SignInManager<User> signInManager) : base(Context)
        {
            RoleManager = _RoleManager;
            UserManager = userManager;
            SignInManager = signInManager;
         
        }
        
        public User GetByEmail(string email)
        {
            var filter = PredicateBuilder.New<User>();
            if (email != null)
            {
                filter = filter.Or(u => u.Email.Contains(email));
                return base.GetByID(filter);
            }
            else return null;
        }
        public async Task<bool> Password(User user,string password)
        {
            await UserManager.RemovePasswordAsync(user);
            await UserManager.AddPasswordAsync(user, password);
            return true;
        }
        public User GetByID(string ID)
        {
            var filter = PredicateBuilder.New<User>();
            if (ID != null)
                filter = filter.Or(u => u.Id == ID);

            var query = GetByID(filter);

            return query;
        }
        public async Task<IdentityResult> AddHR(User obj) { 
           var user = await UserManager.CreateAsync(obj,obj.PasswordHash);
            if (user.Succeeded == true)
            {
                var GetUser = GetByEmail(obj.Email);
                var GetRole = RoleManager.Roles.Select(r => r.Name).Where(r=>r == "HR").FirstOrDefault();
                if (GetRole == null) { 
                var role = await RoleManager.CreateAsync(new IdentityRole
                {
                    Name="hr",
                    NormalizedName="HR"
                });
                    var addedwithrole = await UserManager.AddToRoleAsync(GetUser, "HR");
                    return user;
                }
                else { 
                var addedwithrole = await UserManager.AddToRoleAsync(GetUser, "HR");
                    return user;
                }
            }
            else return user;
        }
        public async Task<SignInResult> SignIn(LoginViewModel obj)
        {
            return await SignInManager.PasswordSignInAsync(obj.Email,
                   obj.Password, obj.RememberMe
                   , true);
        }
        public async Task SignOut() =>
            await SignInManager.SignOutAsync();

        public new async Task<User> Update(User obj)
        {
            var user = GetByID(obj.Id);
            if (obj.Email != null && obj.Email.Length > 7) { 
                user.Email = obj.Email;
                user.UserName = obj.Email;
            }
            if (obj.PhoneNumber != null && obj.PhoneNumber.Length == 11)
            {
                user.PhoneNumber = obj.PhoneNumber;
            }
            if (obj.BirthDate.Year > 1960) { 
                user.BirthDate = obj.BirthDate;
            }
            if (obj.PasswordHash != null && obj.PasswordHash.Length > 7)
            {
                var ChangePass = await Password(user, obj.PasswordHash);
            }
             await UserManager.UpdateAsync(user);
            return user;
        }
        public async Task<IdentityResult> Delete(string obj)
        {
            var filter = PredicateBuilder.New<User>();
            filter = filter.Or(p => p.Id == obj);
            var last = GetByID(filter);
            if (last.IsDeleted == false)
            {
                last.IsDeleted = true;
            }
            else last.IsDeleted = false;
            return await UserManager.UpdateAsync(last);
        }
        
    }
}
