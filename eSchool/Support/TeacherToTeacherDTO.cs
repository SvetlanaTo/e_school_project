using eSchool.Models;
using eSchool.Models.DTOs;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Support
{
    public class TeacherToTeacherDTO
    {
        public TeacherDTOForStudentAndParent ConvertToTeacherDTOForStudentAndParent(Teacher x)
        {
            TeacherDTOForStudentAndParent dto = new TeacherDTOForStudentAndParent
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Gender = x.Gender,
                IsStillWorking = x.IsStillWorking
            };

            return dto;
        }

        public TeacherDTOForTeacher ConvertToTeacherDTOForTeacher(Teacher x)
        {
            TeacherDTOForTeacher dto = new TeacherDTOForTeacher
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Jmbg = x.Jmbg,
                Gender = x.Gender,
                IsStillWorking = x.IsStillWorking
            };
            return dto;
        }

        public TeacherDTOForAdmin ConvertToTeacherDTOForAdmin(Teacher x, IList<IdentityUserRole> roles)
        {
            IList<string> rolesIds = new List<string>();
            foreach (var role in roles)
            {
                rolesIds.Add(role.RoleId);
            }
            TeacherDTOForAdmin dto = new TeacherDTOForAdmin
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
                Gender = x.Gender,
                IsStillWorking = x.IsStillWorking
            };
            return dto;
        }

        public IList<TeacherDTOForStudentAndParent> ConvertToTeacherDTOListForStudentAndParent(List<Teacher> teachers)
        {
            IList<TeacherDTOForStudentAndParent> dtos = new List<TeacherDTOForStudentAndParent>();
            foreach (var teacher in teachers)
            {
                TeacherDTOForStudentAndParent dto = ConvertToTeacherDTOForStudentAndParent(teacher);
                dtos.Add(dto);
            }
            return dtos;
        }

        public IList<TeacherDTOForTeacher> ConvertToTeacherDTOListForTeacher(List<Teacher> teachers)
        {
            IList<TeacherDTOForTeacher> dtos = new List<TeacherDTOForTeacher>();
            foreach (var teacher in teachers)
            {
                TeacherDTOForTeacher dto = ConvertToTeacherDTOForTeacher(teacher);
                dtos.Add(dto);
            }
            return dtos;
        }

        public IList<TeacherDTOForAdmin> ConvertToTeacherDTOListForAdmin(List<Teacher> teachers)
        {
            IList<TeacherDTOForAdmin> dtos = new List<TeacherDTOForAdmin>();
            foreach (var teacher in teachers)
            {

                TeacherDTOForAdmin dto = ConvertToTeacherDTOForAdmin(teacher, (IList<IdentityUserRole>)teacher.Roles);
                dtos.Add(dto);
            }
            return dtos;
        }

        
    }
}