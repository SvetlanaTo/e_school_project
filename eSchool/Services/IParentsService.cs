using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IParentsService
    {
        Parent GetByJmbg(string jmbg);
        IEnumerable<Parent> GetAll();
        Parent GetByID(string id);
        Parent GetByUserName(string userName);
        IEnumerable<ParentDTOForAdmin> GetAllForAdmin();
        IEnumerable<ParentDTOForTeacher> GetAllForTeacher();
        IEnumerable<ParentDTOForStudentAndParents> GetAllForStudentFromStudentForm(string userId);
        IEnumerable<ParentDTOForStudentAndParents> GetAllForParentFromStudentsForms(string userId); 
        Task<ParentDTOForAdmin> Update(string id, PutParentDTO updated);
        Parent Delete(string id);
        
    }
}
