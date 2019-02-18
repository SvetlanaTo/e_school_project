using eSchool.Models;
using eSchool.Models.DTOs;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Support
{
    public class FormToFormDTO
    {
        private TeacherToTeacherDTO teacherToDTO;

        public FormToFormDTO(TeacherToTeacherDTO teacherToDTO)
        {
            this.teacherToDTO = teacherToDTO;
        }

        public FormDTOForStudentAndParents ConvertToFormDTOForStudentAndParent(Form x)
        {
            FormDTOForStudentAndParents dto = new FormDTOForStudentAndParents
            {
                Id = x.Id,
                Grade = x.Grade,
                Tag = x.Tag,
                Started = x.Started,
                AttendingTeacher = teacherToDTO.ConvertToTeacherDTOForStudentAndParent(x.AttendingTeacher)
            };
            return dto;
        }

        public FormDTOForTeacher ConvertToFormDTOForTeacher(Form x)
        {
            FormDTOForTeacher dto = new FormDTOForTeacher
            {
                Id = x.Id,
                Grade = x.Grade,
                Tag = x.Tag,
                Started = x.Started,
                AttendingTeacher = teacherToDTO.ConvertToTeacherDTOForTeacher(x.AttendingTeacher)
            };
            return dto;
        }

        public FormDTOForAdmin ConvertToFormDTOForAdmin(Form x)
        {
            FormDTOForAdmin dto = new FormDTOForAdmin
            {
                Id = x.Id,
                Grade = x.Grade,
                Tag = x.Tag,
                Started = x.Started,
                AttendingTeacher = teacherToDTO.ConvertToTeacherDTOForAdmin(x.AttendingTeacher, (List<IdentityUserRole>)x.AttendingTeacher.Roles),
            };
            return dto;
        }

        public IList<FormDTOForStudentAndParents> ConvertToFormDTOListForStudentAndParent(List<Form> forms)
        {
            IList<FormDTOForStudentAndParents> dtos = new List<FormDTOForStudentAndParents>();
            foreach (var form in forms)
            {
                FormDTOForStudentAndParents dto = ConvertToFormDTOForStudentAndParent(form);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<FormDTOForTeacher> ConvertToFormDTOListForTeacher(List<Form> forms)
        {
            IList<FormDTOForTeacher> dtos = new List<FormDTOForTeacher>();
            foreach (var form in forms)
            {
                FormDTOForTeacher dto = ConvertToFormDTOForTeacher(form);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<FormDTOForAdmin> ConvertToFormDTOListForAdmin(List<Form> forms)
        {
            IList<FormDTOForAdmin> dtos = new List<FormDTOForAdmin>();
            foreach (var form in forms)
            {
                FormDTOForAdmin dto = ConvertToFormDTOForAdmin(form);
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}