using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IFormsService
    {
        IEnumerable<Form> GetAll();
        Form GetByID(int id);

        IEnumerable<FormDTOForAdmin> GetAllForAdmin();
        IEnumerable<FormDTOForTeacher> GetAllForTeacher();
        IEnumerable<FormDTOForStudentAndParents> GetAllForStudentFromStudentForm(string userId);
        IEnumerable<FormDTOForStudentAndParents> GetAllForParentFromStudentsForms(string userId);

        FormDTOForAdmin Update(int id, PutFormDTO updated);
        FormDTOForAdmin Create(PostFormDTO newForm);
        FormDTOForAdmin ChangeAttendingTeacher(int id, string teacherId);

        Form AddStudent(int id, string studentId);
        Form Delete(int id);
        FormIdStudentsDTO GetSortedStudentsNamesByFormId(int id);
        FormIdStudentsDTO GetSortedStudentsNamesByFormIdForParent(int formId, string parentId);
        IEnumerable<FormDTOForAdmin> GetSortedFormsByGradeByYear(int grade, int year);
        FormDTOForAdmin GetFormByAttendingTeacherLastName(string teacherLastName);  
    }
}
