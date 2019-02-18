using eSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IAuthRepository : IDisposable
    {
        Task<IdentityResult> RegisterAdmin(Admin admin, string password);
        Task<IdentityResult> RegisterStudent(Student student, string password);
        Task<IdentityResult> RegisterParent(Parent parent, string password);
        Task<IdentityResult> RegisterTeacher(Teacher teacher, string password);      
        Task<ApplicationUser> FindUser(string userName, string password);
        Task<IdentityRole> FindByIdAsync(string id);
        Task<IList<string>> FindRoles(string userId);
        Task<IdentityUser> FindIdentityUserById(string id);
        Task<ApplicationUser> FindUserById(string userId);
        Task<ApplicationUser> FindUserByUserName(string userName);
        Task<IdentityResult> ChangePasswordAsync(string id, string oldPassword, string newPassword);
        IEnumerable<IdentityRole> GetRoles();
        Task<IdentityResult> RemoveFromRoles(string id, string[] userRoles);
        Task<IdentityResult> AddToRoleAsync(string id, string roleToAssign);
        Task<IdentityRole> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
        Task<string> GenerateEmailConfirmationTokenAsync(string id);
    }
}