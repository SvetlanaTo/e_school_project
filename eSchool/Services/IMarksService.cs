using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IMarksService
    {
        MarkDTO ConvertToMarkDTO(int id);
        IEnumerable<Mark> GetAll();
        IEnumerable<MarkDTO> GetAllDTOs();
        IEnumerable<MarkDTO> GetAllDTOsFromService();
        Mark GetByID(int id);
        MarkDTO GetDTOByID(int id);
        MarkDTO GetByIDDTOFromService(int id);
        MarkDTO Create(PostMarkDTO postDTO, string teacherId, int subjectId, int formId, string studentId);
        MarkDTO Update(int id, string teacherId, int value);
        Mark Delete(int id, string teacherId);
        IEnumerable<Mark> GetMarksByStudentId(string studentId);
        IEnumerable<MarkDTO> GetMarksDTOByStudentId(string studentId);
        IEnumerable<MarkDTO> GetMarksDTOByStudentIdForTeacher(string studentId, string userId);
        IEnumerable<MarkDTO> GetMarksByStudentIdForSubjectId(string studentId, int subjectId);
        MarkValuesListDTO GetMarkValuesListByStudentIdForSubjectId(string studentId, int subjectId);
        IEnumerable<MarkDTO> GetMarksDTOByTeacherId(string teacherId);
        MarkValuesListDTO GetMarkValuesListByStudentIdFromTeacherId(string studentId, string teacherId);
        IEnumerable<MarkValuesListDTO> GetMarksByFormIdFromTeacherIdForSubjectId(int formId, string teacherId, int subjectId);
        IEnumerable<Mark> GetMarksByFormIdForAttendingTeacher(int formId);
        IEnumerable<MarkValuesListDTO> ConvertToMarkValuesListDTOList(int formId);
        Mark GetFirstMarkByFormIdForAttendingTeacherValidation(int formId);
        ReportCardDTO GetReportCardForStudentId(string studentId);


    }
}