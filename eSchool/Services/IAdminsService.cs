using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Services
{
    public interface IAdminsService
    {
        IEnumerable<Admin> GetAll();
        Admin GetByID(string id);
        Admin GetByUserName(string userName);
        Admin GetByJmbg(string jmbg);
        Task<Admin> Update(string id, PutAdminDTO updated);
        Admin Delete(string id, string userId); 
        
    }
}
