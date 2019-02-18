using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Support
{
    public class StudentToStudentDTO
    {
        private ParentToParentDTO parentToDTO;
        private FormToFormDTO formToDTO;

        public StudentToStudentDTO(ParentToParentDTO parentToDTO, FormToFormDTO formToDTO)
        {
            this.parentToDTO = parentToDTO;
            this.formToDTO = formToDTO; 
        }

        public StudentDTOForStudentAndParent ConvertToStudentDTOForStudentAndParent(Student x)
        {

            StudentDTOForStudentAndParent dto = new StudentDTOForStudentAndParent
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Parent = parentToDTO.ConvertToParentDTOForStudentAndParent(x.Parent), 
                Form = formToDTO.ConvertToFormDTOForStudentAndParent(x.Form)
            };

            return dto;

        }
        public StudentDTOForTeacher ConvertToStudentDTOForTeacher(Student x)
        {

            StudentDTOForTeacher dto = new StudentDTOForTeacher
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Jmbg = x.Jmbg,
                DayOfBirth = x.DayOfBirth,
                ImagePath = x.ImagePath,
                IsActive = x.IsActive,
                Parent = parentToDTO.ConvertToParentDTOForTeacher(x.Parent),
                Form = formToDTO.ConvertToFormDTOForTeacher(x.Form) 

               
            };

            return dto;

        }


        public StudentDTOForAdmin ConvertToStudentDTOForAdmin(Student x, IList<IdentityUserRole> roles)  
        {

            IList<string> rolesIds = new List<string>();
            foreach (var role in roles)
            {
                rolesIds.Add(role.RoleId);
            }
            StudentDTOForAdmin dto = new StudentDTOForAdmin
            {
                Id = x.Id,
                Roles = rolesIds,  
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                PhoneNumber = x.PhoneNumber,
                PhoneNumberConfirmed = x.PhoneNumberConfirmed,
                Jmbg = x.Jmbg,
                DayOfBirth = x.DayOfBirth,
                ImagePath = x.ImagePath,
                IsActive = x.IsActive,
                Parent = parentToDTO.ConvertToParentDTOForAdmin(x.Parent, (List<IdentityUserRole>)x.Parent.Roles),
                Form = formToDTO.ConvertToFormDTOForAdmin(x.Form)

               
            };

            return dto;
        }

        public StudentDTOForList ConvertToStudentDTOForList(Student x)
        {

            StudentDTOForList dto = new StudentDTOForList
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Jmbg = x.Jmbg,
                DayOfBirth = x.DayOfBirth, 
                ParentID = x.Parent.Id, 
                FormID = x.Form.Id
            };

            return dto;

        }

        public IList<StudentDTOForStudentAndParent> ConvertToStudentDTOListForStudentAndParent(List<Student> students)
        {
            IList<StudentDTOForStudentAndParent> dtos = new List<StudentDTOForStudentAndParent>();
            foreach (var student in students)
            {
                StudentDTOForStudentAndParent dto = ConvertToStudentDTOForStudentAndParent(student);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<StudentDTOForTeacher> ConvertToStudentDTOListForTeacher(List<Student> students)
        {
            IList<StudentDTOForTeacher> dtos = new List<StudentDTOForTeacher>();
            foreach (var student in students)
            {
                StudentDTOForTeacher dto = ConvertToStudentDTOForTeacher(student);
                dtos.Add(dto);
            }

            return dtos;
        }


        public IList<StudentDTOForAdmin> ConvertToStudentDTOListForAdmin(List<Student> students)
        {
            IList<StudentDTOForAdmin> dtos = new List<StudentDTOForAdmin>();
            foreach (var student in students)
            {
                StudentDTOForAdmin dto = ConvertToStudentDTOForAdmin(student, (IList<IdentityUserRole>)student.Roles);
                dtos.Add(dto);
            }
            return dtos; 
        }

        public IList<StudentDTOForList> ConvertToStudentDTOListForList(List<Student> students)
        {
            IList<StudentDTOForList> dtos = new List<StudentDTOForList>();
            foreach (var student in students)
            {
                StudentDTOForList dto = ConvertToStudentDTOForList(student);
                dtos.Add(dto);
            }
             
            return dtos;
        }

    }
}