using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IFormsToTeacherSubjectsService
    {
        IEnumerable<FormToTeacherSubject> GetAll();
        IEnumerable<FTSDTOForAdmin> GetAllForAdmin();
        FormToTeacherSubject GetByID(int id);
        FTSDTOForAdmin Create(int formId, string teacherId, int subjectId);
        FTSDTOForAdmin Update(int id, PutFormToTeacherSubjectDTO updated);
        FormToTeacherSubject Delete(int id);
        FTSDTOForAdmin UpdateStarted(int id, DateTime updated);
        FTSDTOForAdmin UpdateStopped(int id, DateTime updated);
        FTSDTOForAdmin UpdateStoppedNowByFTSId(int id);

        FormToTeacherSubject FindFTSForMark(int formId, string teacherId, int subjectId);

        FormToTeacherSubject GetActiveFTS(int formId, int tsId);
        IEnumerable<FTSDTOForAdmin> GetAllByTeacherId(string teacherId);
        IEnumerable<FTSDTOForAdmin> GetAllByTeacherSubjectId(int tsId);

        TeachersWeeklyEngagementsDTO GetWorkingTeachersWeeklyEngagements(string adminId); 


    }
}

