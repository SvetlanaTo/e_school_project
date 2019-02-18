using eSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Repositories
{
    public class AuthRepository : IAuthRepository, IDisposable 
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AuthRepository(DbContext context)
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        }

        public async Task<IdentityResult> RegisterAdmin(Admin admin, string password)
        {
            var result = await _userManager.CreateAsync(admin, password);
            _userManager.AddToRole(admin.Id, "admin");
            return result;
        }

        public async Task<IdentityResult> RegisterStudent(Student student, string password)
        {
            var result = await _userManager.CreateAsync(student, password);
            _userManager.AddToRole(student.Id, "student"); 
            return result;
        }

        public async Task<IdentityResult> RegisterParent(Parent parent, string password)
        {
            var result = await _userManager.CreateAsync(parent, password);
            _userManager.AddToRole(parent.Id, "parent");
            return result;
        }

        public async Task<IdentityResult> RegisterTeacher(Teacher teacher, string password)
        {
            var result = await _userManager.CreateAsync(teacher, password);
            _userManager.AddToRole(teacher.Id, "teacher");
            return result;
        }


        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public async Task<IList<string>> FindRoles(string userId)
        {
            return await _userManager.GetRolesAsync(userId); 
        }

        public void Dispose()
        {
            if (_userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string id)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(id);   
        }


        public async Task<IdentityUser> FindIdentityUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id); 
            return user; 
        }

        public async Task<ApplicationUser> FindUserById(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<ApplicationUser> FindUserByUserName(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<IdentityRole> FindByIdAsync(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            return role;
        }
        
        public async Task<IdentityResult> RemoveFromRoles(string id, string[] userRoles)
        {
            var result = await _userManager.RemoveFromRolesAsync(id, userRoles);
            return result;
        }

        public async Task<IdentityResult> AddToRoleAsync(string id, string roleToAssign)
        {
            var result = await _userManager.AddToRoleAsync(id, roleToAssign);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string id, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(id, oldPassword, newPassword);  
            return result; 
        }
        

        public IEnumerable<IdentityRole> GetRoles()
        {
            var roles = _roleManager.Roles;
            return roles;
        }

        public async Task<IdentityRole> CreateRoleAsync(string roleName)
        {
            var role = new IdentityRole { Name = roleName };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return null;
            }

            return role;

        }

        public async Task<IdentityResult> DeleteRoleAsync(IdentityRole role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

    }

}


