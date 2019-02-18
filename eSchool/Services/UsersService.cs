using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace eSchool.Services
{
    public class UsersService : IUsersService 
    {
        private IUnitOfWork db;
        private IEmailsService emailsService;

        public UsersService(IUnitOfWork db, IEmailsService emailsService)
        {
            this.db = db;
            this.emailsService = emailsService;
        }

        public async Task<IdentityResult> RegisterAdmin(RegisterAdminDTO adminDTO)
        {
            ApplicationUser foundJmbg = GetByJmbg(adminDTO.Jmbg);

            if (foundJmbg != null)
            {
                throw new HttpException("A user with JMBG " + adminDTO.Jmbg + " is already in the system.");
            }

            ApplicationUser foundUserName = await FindUserByUserName(adminDTO.UserName);

            if (foundUserName != null)
            {
                throw new HttpException("Username " + adminDTO.UserName + " already exists.");
            }

            Admin user = new Admin
            {

                UserName = adminDTO.UserName,
                FirstName = adminDTO.FirstName,
                LastName = adminDTO.LastName,
                Email = adminDTO.Email,
                Jmbg = adminDTO.Jmbg,
                ShortName = adminDTO.ShortName
            };

            IdentityResult registeredAdmin = await db.AuthRepository.RegisterAdmin(user, adminDTO.Password);
            if (!registeredAdmin.Succeeded)
            {
                throw new HttpException("Failed to register the admin.");
            }

            emailsService.CreateRegistrationMailForAdminOrTeacher(user.Id); 
            return registeredAdmin;
        }

        public async Task<IdentityResult> RegisterStudentAndParent(RegisterStudentDTO studentDTO)
        {
            
            Parent parentInTheSystem = db.ParentsRepository.GetByJmbg(studentDTO.ParentJmbg);
            if (parentInTheSystem != null)
            {
                throw new HttpException("The parent with JMBG " + studentDTO.ParentJmbg + " is already in the system " +
                    "with id: " + parentInTheSystem.Id + " Register student" +
                    " with HttpPost at http://localhost:54164/project/account/register-student");
            }

            ApplicationUser foundJmbg = GetByJmbg(studentDTO.Jmbg);
            if (foundJmbg != null)
            {
                throw new HttpException("The user with JMBG " + studentDTO.Jmbg + " is already in the system with id " + foundJmbg.Id);
            }

            ApplicationUser foundUserName = await FindUserByUserName(studentDTO.UserName);
            if (foundUserName != null)
            {
                throw new HttpException("The username you entered for the student: " + studentDTO.UserName + " already exists.");
            }

            Form form = db.FormsRepository.GetByID(studentDTO.FormId);
            if (form == null)
            {
                throw new HttpException("Form id " + studentDTO.FormId + " was not found.");
            }

            ApplicationUser foundParentUserName = await FindUserByUserName(studentDTO.ParentUserName);
            if (foundParentUserName != null)
            {
                throw new HttpException("Username you entered for the parent " + studentDTO.ParentUserName + " already exists.");
            }

            RegisterParentDTO parentDTO = new RegisterParentDTO
            {
                UserName = studentDTO.ParentUserName,
                FirstName = studentDTO.ParentFirstName,
                LastName = studentDTO.ParentLastName,
                Email = studentDTO.ParentEmail,
                Password = studentDTO.ParentPassword,
                ConfirmPassword = studentDTO.ParentConfirmPassword,
                Jmbg = studentDTO.ParentJmbg,
                MobilePhone = studentDTO.ParentMobilePhone
            };

            IdentityResult registeredParent = await RegisterParent(parentDTO);
            if (!registeredParent.Succeeded)
            {
                throw new HttpException("Failed to register the parent.");
            }

            Parent newParent = db.ParentsRepository.GetByUserName(studentDTO.ParentUserName);

            if (newParent == null)
            {
                throw new HttpException("Something went wrong.");
            }

            Student newUser = new Student
            {
                UserName = studentDTO.UserName,
                FirstName = studentDTO.FirstName,
                LastName = studentDTO.LastName,
                Email = studentDTO.Email,
                Jmbg = studentDTO.Jmbg,
                DayOfBirth = studentDTO.DayOfBirth,
                IsActive = true,
                Parent = newParent,
                Form = form
            };

            IdentityResult regStudent = await db.AuthRepository.RegisterStudent(newUser, studentDTO.Password);
            if (!regStudent.Succeeded)
            {
                throw new HttpException("Failed to register the student.");
            }

            emailsService.CreateRegistrationMail(newUser.Id, studentDTO.Password);
            emailsService.CreateRegistrationMail(newUser.Parent.Id, studentDTO.ParentPassword);
            emailsService.CreateMailForParentNewStudentAssigned(newUser.Parent.Id);
            return regStudent;
        }


        public async Task<IdentityResult> RegisterParent(RegisterParentDTO parentDTO)
        {
            Parent found = db.ParentsRepository.GetByUserName(parentDTO.UserName);
            if (found != null)
            {
                return null;
            }

            Parent user = new Parent
            {
                UserName = parentDTO.UserName,
                FirstName = parentDTO.FirstName,
                LastName = parentDTO.LastName,
                Email = parentDTO.Email,
                Jmbg = parentDTO.Jmbg,
                MobilePhone = parentDTO.MobilePhone
            };

            return await db.AuthRepository.RegisterParent(user, parentDTO.Password);
        }

        public async Task<IdentityResult> RegisterStudent(RegisterStudentDTOAlone studentDTOalone)
        {
            Parent parentInTheSystem = db.ParentsRepository.GetByJmbg(studentDTOalone.ParentJmbg);
            if (parentInTheSystem == null)
            {
                throw new HttpException("The parent with JMBG " + studentDTOalone.ParentJmbg + " is not in the system." +
                    "Register student with parent with HttpPost at http://localhost:54164/project/account/register-student-and-parent");
            }

            ApplicationUser foundJmbg = GetByJmbg(studentDTOalone.Jmbg);
            if (foundJmbg != null)
            {
                throw new HttpException("The user with JMBG " + studentDTOalone.Jmbg + " is already in the system with id " + foundJmbg.Id);
            }

            ApplicationUser foundUserName = await FindUserByUserName(studentDTOalone.UserName);
            if (foundUserName != null)
            {
                throw new HttpException("The username you entered for the student: " + studentDTOalone.UserName + " already exists.");
            }

            Form form = db.FormsRepository.GetByID(studentDTOalone.FormId);
            if (form == null)
            {
                throw new HttpException("Form id " + studentDTOalone.FormId + " was not found.");
            }

            Student user = new Student
            {
                UserName = studentDTOalone.UserName,
                FirstName = studentDTOalone.FirstName,
                LastName = studentDTOalone.LastName,
                Email = studentDTOalone.Email,
                Jmbg = studentDTOalone.Jmbg,
                DayOfBirth = studentDTOalone.DayOfBirth,
                IsActive = true, 
                Parent = parentInTheSystem, 
                Form = form
            };

            IdentityResult registeredStudent = await db.AuthRepository.RegisterStudent(user, studentDTOalone.Password);

            if (!registeredStudent.Succeeded)
            {
                throw new HttpException("Failed to register the student.");
            }

            emailsService.CreateRegistrationMail(user.Id, studentDTOalone.Password);
            emailsService.CreateMailForParentNewStudentAssigned(user.Parent.Id); 
            return registeredStudent;
        }


        public async Task<IdentityResult> RegisterTeacher(RegisterTeacherDTO teacherDTO)
        {
            ApplicationUser foundJmbg = GetByJmbg(teacherDTO.Jmbg);
            if (foundJmbg != null)
            {
                throw new HttpException("The user with JMBG " + teacherDTO.Jmbg + " is already in the system with id " + foundJmbg.Id);
            }

            ApplicationUser foundUserName = await FindUserByUserName(teacherDTO.UserName);
            if (foundUserName != null)
            {
                throw new HttpException("The username you entered for the teacher: " + teacherDTO.UserName + " already exists.");
            }

            Teacher user = new Teacher
            {
                UserName = teacherDTO.UserName,
                FirstName = teacherDTO.FirstName,
                LastName = teacherDTO.LastName,
                Email = teacherDTO.Email,
                PhoneNumber = teacherDTO.PhoneNumber,
                PhoneNumberConfirmed = teacherDTO.PhoneNumberConfirmed,
                Jmbg = teacherDTO.Jmbg,              
                Gender = teacherDTO.Gender,
                IsStillWorking = true 
            };

            IdentityResult registeredTeacher = await db.AuthRepository.RegisterTeacher(user, teacherDTO.Password);
            if (!registeredTeacher.Succeeded)
            {
                throw new HttpException("Failed to register the teacher.");
            }

            emailsService.CreateRegistrationMailForAdminOrTeacher(user.Id); 
            return registeredTeacher;
        }

        public async Task<IdentityResult> ChangePasswordFromUriAsync(string id, string oldPassword, string newPassword) 
        {
            var foundUser = GetById(id);

            if (foundUser == null)
            {
                throw new HttpException("The user by id " + id + " was not found.");
            }

            var authUser = await db.AuthRepository.FindUser(foundUser.UserName, oldPassword);


            if (authUser == null)
            {
                throw new HttpException("The username and the old password do not match.");
            }

            var result = await db.AuthRepository.ChangePasswordAsync(id, oldPassword, newPassword);

            if (!result.Succeeded)
            {
                throw new HttpException("Failed to change password!");
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordFromBodyAsync(string id, ChangePassDTO updated)
        {
            var foundUser = GetById(id);
            if (foundUser == null)
            {
                throw new HttpException("The user by id " + id + " was not found.");
            }

            
            var authUser = await db.AuthRepository.FindUser(foundUser.UserName, updated.OldPassword); 


            if (authUser == null)
            {
                throw new HttpException("The username and the old password do not match.");
            }


            var result = await db.AuthRepository.ChangePasswordAsync(id, updated.OldPassword, updated.NewPassword); 

            if (!result.Succeeded)
            {
                throw new HttpException("Failed to change password!");
            }

            return result;
        }

        public ApplicationUser GetByJmbg(string jmbg)
        {
            return db.UsersRepository.Get(x => x.Jmbg == jmbg).FirstOrDefault();
        }

        public ApplicationUser GetById(string id)
        {
            return db.UsersRepository.GetByID(id);
        }

        public async Task<ApplicationUser> FindUserByUserName(string userName)
        {
            ApplicationUser user = await db.AuthRepository.FindUserByUserName(userName);
            return user;
        }

        public async Task<IList<string>> GetAllRolesByUserId(string userId)
        {
            return await db.AuthRepository.FindRoles(userId);
        }

        
    }
}
