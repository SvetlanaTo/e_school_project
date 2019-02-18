using eSchool.Models;
using eSchool.Models.DTOs;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Support
{
    public class ParentToParentDTO
    {

        public ParentDTOForStudentAndParents ConvertToParentDTOForStudentAndParent(Parent x)
        {
            ParentDTOForStudentAndParents dto = new ParentDTOForStudentAndParents
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
            };
            return dto;
        }

        public ParentDTOForTeacher ConvertToParentDTOForTeacher(Parent x)
        {

            ParentDTOForTeacher dto = new ParentDTOForTeacher
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Jmbg = x.Jmbg,
                MobilePhone = x.MobilePhone
            };
            return dto;
        }

        public ParentDTOForAdmin ConvertToParentDTOForAdmin(Parent x, IList<IdentityUserRole> roles)
        {
            IList<string> rolesIds = new List<string>();
            foreach (var role in roles)
            {
                rolesIds.Add(role.RoleId);
            }

            ParentDTOForAdmin dto = new ParentDTOForAdmin
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
                MobilePhone = x.MobilePhone
            };
            return dto;
        }


        public IList<ParentDTOForAdmin> ConvertToParentDTOListForAdmin(List<Parent> parents)
        {
            IList<ParentDTOForAdmin> dtos = new List<ParentDTOForAdmin>();

            foreach (var parent in parents)
            {
                ParentDTOForAdmin dto = ConvertToParentDTOForAdmin(parent, (IList<IdentityUserRole>)parent.Roles);  
                dtos.Add(dto);
            }
            return dtos;
        }

        public IList<ParentDTOForTeacher> ConvertToParentDTOListForTeacher(List<Parent> parents)
        {
            IList<ParentDTOForTeacher> dtos = new List<ParentDTOForTeacher>();

            foreach (var parent in parents)
            {
                ParentDTOForTeacher dto = ConvertToParentDTOForTeacher(parent);
                dtos.Add(dto);
            }
            return dtos;
        }

        public IList<ParentDTOForStudentAndParents> ConvertToParentDTOListForStudentAndParent(List<Parent> parents)
        {
            IList<ParentDTOForStudentAndParents> dtos = new List<ParentDTOForStudentAndParents>();
            foreach (var parent in parents)
            {
                ParentDTOForStudentAndParents dto = ConvertToParentDTOForStudentAndParent(parent);
                dtos.Add(dto);
            }
            return dtos;
        }

    }
}