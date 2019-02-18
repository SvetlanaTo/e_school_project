using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Services
{
    public class AdminsService : IAdminsService   
    {
        private IUnitOfWork db;
        private IUsersService usersService;
        private IEmailsService emailsService;

        public AdminsService(IUnitOfWork db, IUsersService usersService, IEmailsService emailsService)
        {
            this.db = db;
            this.usersService = usersService;
            this.emailsService = emailsService;
        }

        public IEnumerable<Admin> GetAll()
        {
            return db.AdminsRepository.Get();
        }

        public Admin GetByID(string id)
        {
            return db.AdminsRepository.GetByID(id);
        }
        
        public Admin GetByUserName(string userName)
        {
            return db.AdminsRepository.Get(x => x.UserName.ToLower() == userName.ToLower()).FirstOrDefault(); 
        }

        public Admin GetByJmbg(string jmbg)
        {
            return db.AdminsRepository.Get(x => x.Jmbg == jmbg).FirstOrDefault();
        }

        public async Task<Admin> Update(string id, PutAdminDTO updated) 
        {
            Admin found = db.AdminsRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Admin with id: " + id + " was not found.");
            }
            if (updated.UserName != null)
            {
                ApplicationUser foundByUserName = await usersService.FindUserByUserName(updated.UserName);
                if (foundByUserName != null && foundByUserName.Id != found.Id)
                {
                    throw new HttpException("The username " + foundByUserName.UserName + " already exists. " +
                        "(The user with id: " + foundByUserName.Id + ")");
                }
                found.UserName = updated.UserName;
            }
            if (updated.Jmbg != null)
            {
                ApplicationUser foundByJmbg = usersService.GetByJmbg(updated.Jmbg);
                if (foundByJmbg != null && foundByJmbg.Id != found.Id)
                {
                    throw new HttpException("The user with JMBG: " + updated.Jmbg + " is already in the sistem." +
                    "(The user with id: " + foundByJmbg.Id + ")");
                }
            }
            if (updated.FirstName != null)
                found.FirstName = updated.FirstName;
            if (updated.LastName != null)
                found.LastName = updated.LastName;
            if (updated.Email != null)
                found.Email = updated.Email;
            if (updated.EmailConfirmed != null)
                found.EmailConfirmed = (bool)updated.EmailConfirmed;
            if (updated.PhoneNumber != null)
                found.PhoneNumber = updated.PhoneNumber;
            if (updated.PhoneNumberConfirmed != null)
                found.PhoneNumberConfirmed = (bool)updated.PhoneNumberConfirmed;           

            db.AdminsRepository.Update(found);
            db.Save();

            emailsService.CreateMailForUserUpdate(found.Id);

            return found; 
        }

        public Admin Delete(string id, string userId) 
        {
            Admin found = GetByID(id);
            if(found == null)
            {
                throw new HttpException("The admin by id " + id + " was not found.");
            }

            db.AdminsRepository.Delete(found);
            db.Save();

            return found;
        }

        
    }
}