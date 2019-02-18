using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IEmailsService
    {
        
        void CreateRegistrationMail(string id, string password);
        void CreateMailForParentNewStudentAssigned(string id);

        void CreateMailForUserUpdate(string id);
        void CreateMailForParentForStudentUpdate(string studentId);  

        void NewMarkMailForParent(int markId);
        void NewMarkMailForStudent(int markId); 

        void MarkUpdateMailForParent(int markId, int oldMarkValue);
        void MarkUpdateMailForStudent(int markId, int oldMarkValue);

        void CreateMailForAdminTeachersEngagementsReport(TeachersWeeklyEngagementsDTO report, string adminId);
        void CreateRegistrationMailForAdminOrTeacher(string id);
        void CreateReportCardMail(string studentId, ReportCardDTO reportCard); 
 
    }
}
