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
    public class StudentsService : IStudentsService 
    {
        private IUnitOfWork db;
        private IUsersService usersService;
        private IParentsService parentsService;
        private IFormsService formsService;
        private StudentToStudentDTO toDTO;
        private IEmailsService emailsService;

        public StudentsService(IUnitOfWork db, IUsersService usersService, IParentsService parentsService, IFormsService formsService, StudentToStudentDTO toDTO, IEmailsService emailsService)
        {
            this.db = db;
            this.usersService = usersService;
            this.parentsService = parentsService;
            this.formsService = formsService;
            this.toDTO = toDTO;
            this.emailsService = emailsService;
        }

        public IEnumerable<Student> GetAll()
        {
            return db.StudentsRepository.Get();
        }

        public Student GetByID(string id)
        {
            return db.StudentsRepository.GetByID(id);
        }

        public Student GetByUserName(string userName)
        {
            return db.StudentsRepository.GetByUserName(userName);
        }

        public IEnumerable<StudentDTOForAdmin> GetAllForAdmin()
        {
            IEnumerable<Student> students = GetAll();

            if (students.Count() != 0) 
            {
                IList<StudentDTOForAdmin> dtos = toDTO.ConvertToStudentDTOListForAdmin((List<Student>)students);
                return dtos;
            }
            return null;
        }

        public IEnumerable<StudentDTOForTeacher> GetAllForTeacher()
        {
            IEnumerable<Student> students = GetAll();

            if (students.Count() != 0)
            {
                IList<StudentDTOForTeacher> dtos = toDTO.ConvertToStudentDTOListForTeacher((List<Student>)students);
                return dtos;
            }
            return null;
        }

        public IEnumerable<StudentDTOForStudentAndParent> GetAllForStudentFromStudentForm(string userId)
        {
            Student foundUser = GetByID(userId);
            if (foundUser == null)
            {
                return null; 
            }
            Form usersForm = db.FormsRepository.GetByID(foundUser.Form.Id);

            if (usersForm == null)
            {
                return null;
            }

            IEnumerable<Student> usersFormsStudents = usersForm.Students;
            if (usersFormsStudents != null)
            {
                IList<StudentDTOForStudentAndParent> dtos = toDTO.ConvertToStudentDTOListForStudentAndParent((List<Student>)usersFormsStudents);
                return dtos;
            }
            return null;
        }

        public IEnumerable<StudentDTOForStudentAndParent> GetAllForParentFromStudentsForms(string userId)
        {
            Parent foundUser = db.ParentsRepository.GetByID(userId);
            //ovo je nemoguce
            if (foundUser == null)
            {
                return null;
            }

            IEnumerable<Student> children = foundUser.Students;
            if (children == null)
            {
                return null;
            }

            List<Student> uniqueStudents = new List<Student>();

            foreach (var child in children)
            {
                Form childsForm = db.FormsRepository.GetByID(child.Form.Id);

                if (childsForm == null)
                {
                    return null;
                }

                List<Student> childFormsStudents = (List<Student>)childsForm.Students;

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

            IList<StudentDTOForStudentAndParent> dtos = toDTO.ConvertToStudentDTOListForStudentAndParent(uniqueStudents);
            return dtos;
        }

        public async Task<StudentDTOForAdmin> Update(string id, PutStudentDTO updated)
        {
            Student found = db.StudentsRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The student with id: " + id + " was not found.");
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
                    throw new HttpException("The user with JMBG: " + updated.Jmbg + " is already in the sistem." +
                    "Leave blank if you don't want to change the JMBG.");
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
            if (updated.DayOfBirth != null)
                found.DayOfBirth = (DateTime)updated.DayOfBirth;
            if (updated.IsActive != null)
                found.IsActive = (bool)updated.IsActive;
            if (updated.FormId != null)
            {
                Form foundForm = formsService.GetByID((int)updated.FormId);

                if (foundForm == null)
                {
                    throw new HttpException("The Form with id: " + updated.FormId + " was not found.");
                }

                if (foundForm.Started.AddDays(360).CompareTo(DateTime.UtcNow) < 0)
                {
                    throw new HttpException("The Form with id: " + id + " was not created for this shool year. " +
                        "This form is from: " + foundForm.Started.Year + ". Students must be assign to a form from this school year.");
                }

                found.Form = foundForm;
            }

            db.StudentsRepository.Update(found);
            db.Save();

            emailsService.CreateMailForUserUpdate(found.Id);
            emailsService.CreateMailForParentForStudentUpdate(found.Id);  

            StudentDTOForAdmin updatedDTO = new StudentDTOForAdmin();
            updatedDTO = toDTO.ConvertToStudentDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);

            return updatedDTO;
        }

        public Student Delete(string id)
        {
            Student found = db.StudentsRepository.GetByID(id);
            if (found != null)
            {
                Parent foundParent = db.ParentsRepository.GetByID(found.Parent.Id);
                if (foundParent != null)
                {
                    List<Student> children = (List<Student>)foundParent.Students;
                    if (children.Count == 1)
                    {
                        db.ParentsRepository.Delete(foundParent);
                    }
                }

                IEnumerable<Mark> foundStudentMarks = db.MarksRepository.GetAllMarksByStudentId(found.Id);
                foreach (var mark in foundStudentMarks)
                {
                    db.MarksRepository.Delete(mark);
                }

                db.StudentsRepository.Delete(found);
                db.Save();
            }
            return found;
        }

        public StudentDTOForAdmin UpdateStudentWithImage(string id, string localFileName)
        {
            Student found = db.StudentsRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The student with id: " + id + " was not found.");
            }

            found.ImagePath = localFileName;
            db.StudentsRepository.Update(found);
            db.Save();

            StudentDTOForAdmin updatedDTO = new StudentDTOForAdmin();
            updatedDTO = toDTO.ConvertToStudentDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);

            return updatedDTO;
        }


        public StudentDTOForAdmin UpdateStudentParent(string id, string parentId)
        {
            Student found = db.StudentsRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The student with id: " + id + " was not found.");
            }

            Parent oldParent = db.ParentsRepository.GetByID(found.Parent.Id);
            if (oldParent == null)
            {
                throw new HttpException("The student with id: " + id + " dont have a gardian - not possible.");
            }

            Parent foundParent = db.ParentsRepository.GetByID(parentId);
            found.Parent = foundParent ?? throw new HttpException("The parent with id: " + parentId + " was not found.");

            db.StudentsRepository.Update(found);

            IEnumerable<Student> oldParentStudents = oldParent.Students;
            if (oldParentStudents.Count() == 0)
            {
                db.ParentsRepository.Delete(oldParent); 
            }

            db.Save();

            emailsService.CreateMailForParentNewStudentAssigned(found.Parent.Id);

            StudentDTOForAdmin updatedDTO = new StudentDTOForAdmin();
            updatedDTO = toDTO.ConvertToStudentDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);

            return updatedDTO;
        }

        public IEnumerable<Student> GetStudentsAssignedToParentId(string parentId)
        {
            Parent foundParent = db.ParentsRepository.GetByID(parentId);
            if (foundParent == null)
            {
                throw new HttpException("The parent with id: " + parentId + " was not found.");
            }

            IEnumerable<Student> parentStudents = foundParent.Students;
            return parentStudents;
        }

        public StudentDTOForAdmin UpdateStudentForm(string id, int formId)
        {
            Student found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The student by id " + id + " was not found.");
            }
            if (found.IsActive == false)
            {
                throw new HttpException("The student by id " + id + " is no longer actively enrolled in this school.");
            }

            Form foundForm = db.FormsRepository.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The form with id: " + formId + " was not found.");
            }

            if (foundForm.Started.AddDays(360).CompareTo(DateTime.UtcNow) < 0)
            {
                throw new HttpException("The Form with id: " + formId + " was not created for this shool year. " +
                    "This form is from: " + foundForm.Started.Year + ". Students must be assign to a form from this school year.");
            }

            found.Form = foundForm;
            db.StudentsRepository.Update(found);
            db.Save();

            StudentDTOForAdmin dto = toDTO.ConvertToStudentDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);
            return dto;
        }

        public IEnumerable<Student> GetStudentsByFormId(int formId)
        {
            Form found = db.FormsRepository.GetByID(formId);

            if (found == null)
            {
                throw new HttpException("The Form with id: " + formId + " was not found.");
            }

            IEnumerable<Student> students = found.Students;
            IList<Student> sorted = students.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return sorted;
        }

        public IEnumerable<FormStudentDTO> GetStudentsByLastNameSorted(string lastName)
        {
            IEnumerable<Student> studentsByLastName = db.StudentsRepository.GetAllByLastName(lastName);

            IList<FormStudentDTO> dtos = new List<FormStudentDTO>();
            foreach(var student in studentsByLastName)
            {
                FormStudentDTO dto = ConvertToFormStudentDTO(student);
                dtos.Add(dto);
            }

            dtos = dtos.OrderBy(x => x.Student).ThenBy(x => x.Id).ToList();
            return dtos; 
        }

        private FormStudentDTO ConvertToFormStudentDTO(Student student)
        {
            if (student == null)
            {
                return null;
            }

            FormStudentDTO dto = new FormStudentDTO
            {
                Id = student.Id,
                Student = student.LastName + " " + student.FirstName
            };

            return dto;
        }
    }
} 




