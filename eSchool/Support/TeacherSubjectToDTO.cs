using eSchool.Models;
using eSchool.Models.DTOs;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Support
{
    public class TeacherSubjectToDTO
    {
        private TeacherToTeacherDTO teacherToDTO;

        public TeacherSubjectToDTO(TeacherToTeacherDTO teacherToDTO)
        {
            this.teacherToDTO = teacherToDTO;
        }

        public TeacherToSubjectDTOForAdmin ConvertToTeacherToSubjectDTOForAdmin(TeacherToSubject x)
        {
            TeacherToSubjectDTOForAdmin dto = new TeacherToSubjectDTOForAdmin
            {
                Id = x.Id,
                Teacher = teacherToDTO.ConvertToTeacherDTOForAdmin(x.Teacher, (List<IdentityUserRole>)x.Teacher.Roles),
                Subject = x.Subject,
                StartedTeaching = x.StartedTeaching,
                StoppedTeaching = x.StoppedTeaching
            };
            return dto;
        }

        public TeacherToSubjectDTOForTeacher ConvertToTeacherToSubjectDTOForTeacher(TeacherToSubject x)
        {
            TeacherToSubjectDTOForTeacher dto = new TeacherToSubjectDTOForTeacher
            {
                Id = x.Id,
                Teacher = teacherToDTO.ConvertToTeacherDTOForTeacher(x.Teacher),
                Subject = x.Subject,
                StartedTeaching = x.StartedTeaching,
                StoppedTeaching = x.StoppedTeaching
            };
            return dto;
        }

        public TeacherToSubjectDTOForStudentAndParent ConvertToTeacherToSubjectDTOForStudentAndParent(TeacherToSubject x)
        {
            TeacherToSubjectDTOForStudentAndParent dto = new TeacherToSubjectDTOForStudentAndParent
            {
                Id = x.Id,
                Teacher = teacherToDTO.ConvertToTeacherDTOForStudentAndParent(x.Teacher),
                Subject = x.Subject,
                StartedTeaching = x.StartedTeaching,
                StoppedTeaching = x.StoppedTeaching
            };
            return dto;
        }

        public IList<TeacherToSubjectDTOForStudentAndParent> ConvertToTSDTOListForStudentAndParent(List<TeacherToSubject> tTSs)
        {
            IList<TeacherToSubjectDTOForStudentAndParent> dtos = new List<TeacherToSubjectDTOForStudentAndParent>();
            foreach (var ts in tTSs)
            {
                TeacherToSubjectDTOForStudentAndParent dto = ConvertToTeacherToSubjectDTOForStudentAndParent(ts);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<TeacherToSubjectDTOForTeacher> ConvertToTSDTOListForTeacher(List<TeacherToSubject> tTSs)
        {
            IList<TeacherToSubjectDTOForTeacher> dtos = new List<TeacherToSubjectDTOForTeacher>();
            foreach (var ts in tTSs)
            {
                TeacherToSubjectDTOForTeacher dto = ConvertToTeacherToSubjectDTOForTeacher(ts);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<TeacherToSubjectDTOForAdmin> ConvertToTSDTOListForAdmin(List<TeacherToSubject> tTSs)
        {
            IList<TeacherToSubjectDTOForAdmin> dtos = new List<TeacherToSubjectDTOForAdmin>();
            foreach (var ts in tTSs)
            {
                TeacherToSubjectDTOForAdmin dto = ConvertToTeacherToSubjectDTOForAdmin(ts);
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}