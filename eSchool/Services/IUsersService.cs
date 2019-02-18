using eSchool.Models;
using eSchool.Models.DTOs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IUsersService
    {
        Task<IdentityResult> RegisterAdmin(RegisterAdminDTO adminDTO);
        Task<IdentityResult> RegisterStudentAndParent(RegisterStudentDTO studentDTO);
        Task<IdentityResult> RegisterStudent(RegisterStudentDTOAlone studentDTOalone);
        Task<IdentityResult> RegisterParent(RegisterParentDTO parentDTO);
        Task<IdentityResult> RegisterTeacher(RegisterTeacherDTO teacherDTO);
        Task<IdentityResult> ChangePasswordFromUriAsync(string id, string oldPassword, string newPassword);
        Task<IdentityResult> ChangePasswordFromBodyAsync(string id, ChangePassDTO updated);
       
        ApplicationUser GetByJmbg(string jmbg);
        ApplicationUser GetById(string id);
        Task<ApplicationUser> FindUserByUserName(string userName);
        Task<IList<string>> GetAllRolesByUserId(string userId);


    }
}