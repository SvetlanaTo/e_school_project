using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Support
{
    public class FTSToDTO
    {
        private FormToFormDTO formToDTO;
        private TeacherSubjectToDTO tsToDTO;

        public FTSToDTO(FormToFormDTO formToDTO, TeacherSubjectToDTO tsToDTO)
        {
            this.formToDTO = formToDTO;
            this.tsToDTO = tsToDTO;
        }

        public FTSDTOForAdmin ConvertToFTSDTOForAdmin(FormToTeacherSubject x)
        {
            FTSDTOForAdmin dto = new FTSDTOForAdmin
            {
                Id = x.Id,
                Form = formToDTO.ConvertToFormDTOForAdmin(x.Form),
                TeacherToSubject = tsToDTO.ConvertToTeacherToSubjectDTOForAdmin(x.TeacherToSubject),
                Started = x.Started,
                Stopped = x.Stopped
            };
            return dto;
        }

        public FTSDTOForTeacher ConvertToFTSDTOForTeacher(FormToTeacherSubject x)
        {
            FTSDTOForAdmin dto = new FTSDTOForAdmin
            {
                Id = x.Id,
                Form = formToDTO.ConvertToFormDTOForTeacher(x.Form),
                TeacherToSubject = tsToDTO.ConvertToTeacherToSubjectDTOForTeacher(x.TeacherToSubject),
                Started = x.Started,
                Stopped = x.Stopped
            };
            return dto;
        }

        public FTSDTOForUser ConvertToFTSDTOForUser(FormToTeacherSubject x)
        {
            FTSDTOForAdmin dto = new FTSDTOForAdmin
            {
                Id = x.Id,
                Form = formToDTO.ConvertToFormDTOForStudentAndParent(x.Form),
                TeacherToSubject = tsToDTO.ConvertToTeacherToSubjectDTOForStudentAndParent(x.TeacherToSubject),
                Started = x.Started,
                Stopped = x.Stopped 
            };
            return dto;
        }

        public IList<FTSDTOForUser> ConvertToFTSDtoListForUser(List<FormToTeacherSubject> fTSs)
        {
            IList<FTSDTOForUser> dtos = new List<FTSDTOForUser>();
            foreach (var fts in fTSs)
            {
                FTSDTOForUser dto = ConvertToFTSDTOForUser(fts);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<FTSDTOForTeacher> ConvertToFTSDtoListForTeacher(List<FormToTeacherSubject> fTSs)
        {
            IList<FTSDTOForTeacher> dtos = new List<FTSDTOForTeacher>();
            foreach (var fts in fTSs)
            {
                FTSDTOForTeacher dto = ConvertToFTSDTOForTeacher(fts);
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<FTSDTOForAdmin> ConvertToFTSDTOListForAdmin(List<FormToTeacherSubject> fTSs) 
        {
            IList<FTSDTOForAdmin> dtos = new List<FTSDTOForAdmin>();
            foreach (var fts in fTSs)
            {
                FTSDTOForAdmin dto = ConvertToFTSDTOForAdmin(fts); 
                dtos.Add(dto);
            }
            return dtos;
        }
    }
}