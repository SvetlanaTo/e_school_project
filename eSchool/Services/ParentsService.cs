using eSchool.Infrastructure;
using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using eSchool.Support;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Services
{
    public class ParentsService : IParentsService 
    {
        private IUnitOfWork db;
        private ParentToParentDTO toDTO;
        private IUsersService usersService;
        private IEmailsService emailsService;

        public ParentsService(IUnitOfWork db, ParentToParentDTO toDTO, IUsersService usersService, IEmailsService emailsService)
        {
            this.db = db;
            this.toDTO = toDTO;
            this.usersService = usersService;
            this.emailsService = emailsService;
        }

        public Parent GetByJmbg(string jmbg)
        {
            ApplicationUser user = db.UsersRepository.Get(x => x.Jmbg == jmbg).FirstOrDefault();
            if (user == null)
            {
                throw new HttpException("The user by JMBG " + jmbg + " is not in the system.");
            }

            Parent parent = db.ParentsRepository.GetByJmbg(jmbg); 
            if (parent == null)
            {
                throw new HttpException("The user by JMBG " + jmbg + " is in the sistem (user id: " + user.Id + "), " +
                    "but the entity type is not - Parent.");
            }
            return parent;
        }

        public IEnumerable<Parent> GetAll()
        {
            return db.ParentsRepository.Get();
        }

        public Parent GetByID(string id)
        {
            return db.ParentsRepository.GetByID(id);
        }

        public Parent GetByUserName(string userName)
        {
            return db.ParentsRepository.GetByUserName(userName);
        }

        public IEnumerable<ParentDTOForAdmin> GetAllForAdmin()
        {
            IEnumerable<Parent> parents = GetAll();

            if (parents != null)
            {
                IList<ParentDTOForAdmin> dtos = toDTO.ConvertToParentDTOListForAdmin((List<Parent>)parents);
                return dtos;
            }
            return null;
        }

        public IEnumerable<ParentDTOForTeacher> GetAllForTeacher()
        {
            IEnumerable<Parent> parents = GetAll();

            if (parents != null)
            {
                IList<ParentDTOForTeacher> dtos = toDTO.ConvertToParentDTOListForTeacher((List<Parent>)parents);
                return dtos;
            }
            return null;
        }

        public IEnumerable<ParentDTOForStudentAndParents> GetAllForStudentFromStudentForm(string userId)
        {
            Student foundUser = db.StudentsRepository.GetByID(userId);
            if (foundUser == null)
            {
                return null;
            }
            Form usersForm = db.FormsRepository.GetByID(foundUser.Form.Id);

            if (usersForm == null)
            {
                return null;
            }

            List<Parent> parents = new List<Parent>();
            IEnumerable<Student> usersFormsStudents = usersForm.Students;
            if (usersFormsStudents != null)
            {
                foreach (var s in usersFormsStudents)
                {
                    if (!parents.Contains(s.Parent))
                    {
                        parents.Add(s.Parent);
                    }
                }
            }

            IList<ParentDTOForStudentAndParents> dtos = toDTO.ConvertToParentDTOListForStudentAndParent(parents);
            return dtos;
        }

        public IEnumerable<ParentDTOForStudentAndParents> GetAllForParentFromStudentsForms(string userId)
        {
            Parent foundUser = GetByID(userId);
            if (foundUser == null)
            {
                return null;
            }

            IEnumerable<Student> children = foundUser.Students;
            if (children == null)
            {
                return null;
            }

            IList<Student> uniqueStudents = new List<Student>();
            foreach (var child in children)
            {
                Form childsForm = db.FormsRepository.GetByID(child.Form.Id);

                if (childsForm == null)
                {
                    return null;
                }

                IList<Student> childFormsStudents = (List<Student>)childsForm.Students;

                if (childFormsStudents.Count > 0)
                {
                    foreach (var s in childFormsStudents)
                    {
                        if (!uniqueStudents.Contains(s))
                        {
                            uniqueStudents.Add(s);
                        }
                    }
                }
            }
            List<Parent> parents = new List<Parent>();
            foreach (var stud in uniqueStudents)
            {
                if (!parents.Contains(stud.Parent))
                {
                    parents.Add(stud.Parent);
                }
            }

            IList<ParentDTOForStudentAndParents> dtos = toDTO.ConvertToParentDTOListForStudentAndParent(parents);
            return dtos;
        }

        public async Task<ParentDTOForAdmin> Update(string id, PutParentDTO updated)
        {
            Parent found = db.ParentsRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The parent with id: " + id + " was not found.");
            }
            if (updated.UserName != null)
            {
                ApplicationUser foundByUserName = await usersService.FindUserByUserName(updated.UserName);
                if (foundByUserName != null && foundByUserName.Id != found.Id)
                {
                    throw new HttpException("The username " + updated.UserName + " already exists."); 
                }
                found.UserName = updated.UserName;
            }
            if (updated.Jmbg != null)
            {
                ApplicationUser foundByJmbg = usersService.GetByJmbg(updated.Jmbg);
                if (foundByJmbg != null && foundByJmbg.Id != found.Id)
                {
                    throw new HttpException("The user with JMBG: " + updated.Jmbg + " is already in the sistem.");
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
            if (updated.MobilePhone != null)
                found.MobilePhone = updated.MobilePhone;

            db.ParentsRepository.Update(found);
            db.Save();

            emailsService.CreateMailForUserUpdate(found.Id);

            ParentDTOForAdmin updatedDTO = new ParentDTOForAdmin();
            updatedDTO = toDTO.ConvertToParentDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);

            return updatedDTO;
        }

        public Parent Delete(string id)
        {
            Parent user = db.ParentsRepository.GetByID(id);
            if (user == null)
            {
                throw new HttpException("The Parent with id: " + id + " was not found.");
            }

            List<Student> parentStudents = (List<Student>)user.Students;

            if (parentStudents.Count != 0)
            {
                throw new HttpException("The Parent id: " + id + " has " + parentStudents.Count + " student/students assigned " +
                    "to his name. For more info go to HttpGet at route: " +
                    "http://localhost:54164/project/students/assigned-to-parent/" + user.Id + " . To delete this Parent, " +
                    "you need to assign student/students to a different guardian with HttpPut and route: " +
                    "http://localhost:54164/project/students/{id}/assign-to-parent/{parentId} " +
                    " or delete parent with student. Thank you for your cooperation.");
            }

            db.ParentsRepository.Delete(user);
            db.Save();

            return user;
        }

       
    }
}

