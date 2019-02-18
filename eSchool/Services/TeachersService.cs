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
    public class TeachersService : ITeachersService
    {
        private IUnitOfWork db;
        private IEmailsService emailsService;
        private IUsersService usersService;
        private TeacherToTeacherDTO toDTO;

        public TeachersService(IUnitOfWork db, IEmailsService emailsService, IUsersService usersService, TeacherToTeacherDTO toDTO) : this(db)
        {
            this.emailsService = emailsService;
            this.usersService = usersService;
            this.toDTO = toDTO;
        }

        public TeachersService(IUnitOfWork db)
        {
            this.db = db;
        }

        public IEnumerable<Teacher> GetAll()
        {
            return db.TeachersRepository.Get();
        }

        public Teacher GetById(string id)
        {
            return db.TeachersRepository.GetByID(id);
        }

        public Teacher GetByUserName(string userName)
        {
            return db.TeachersRepository.GetByUserName(userName);
        }

        public IEnumerable<TeacherDTOForAdmin> GetAllForAdmin()
        {
            IEnumerable<Teacher> teachers = GetAll();

            if (teachers != null)
            {
                IList<TeacherDTOForAdmin> dtos = toDTO.ConvertToTeacherDTOListForAdmin((List<Teacher>)teachers);
                return dtos;
            }
            return null;
        }

        public IEnumerable<TeacherDTOForTeacher> GetAllForTeacher()
        {
            IEnumerable<Teacher> teachers = db.TeachersRepository.GetAllStillWorkingTeachers();

            if (teachers != null)
            {
                IList<TeacherDTOForTeacher> dtos = toDTO.ConvertToTeacherDTOListForTeacher((List<Teacher>)teachers);
                return dtos;
            }
            return null;
        }

        public IEnumerable<TeacherDTOForStudentAndParent> GetAllForStudentAndParent()
        {
            IEnumerable<Teacher> teachers = db.TeachersRepository.GetAllStillWorkingTeachers();

            if (teachers != null)
            {
                IList<TeacherDTOForStudentAndParent> dtos = toDTO.ConvertToTeacherDTOListForStudentAndParent((List<Teacher>)teachers);
                return dtos;
            }
            return null;
        }

        public async Task<TeacherDTOForAdmin> Update(string id, PutTeacherDTO updated)
        {
            Teacher found = db.TeachersRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Teacher with id: " + id + " was not found.");
            }
            if (updated.UserName != null)
            {
                ApplicationUser foundByUserName = await usersService.FindUserByUserName(updated.UserName);
                if (foundByUserName != null && foundByUserName.Id != found.Id)
                {
                    throw new HttpException("The username " + updated.UserName + " already exists. " +
                        "Leave blank if you don't want to change the user name.");
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
            if (updated.Gender != null)
                found.Gender = (Genders)updated.Gender;

            db.TeachersRepository.Update(found);
            db.Save();

            emailsService.CreateMailForUserUpdate(found.Id);

            TeacherDTOForAdmin updatedDTO = new TeacherDTOForAdmin();
            updatedDTO = toDTO.ConvertToTeacherDTOForAdmin(found, (List<IdentityUserRole>)found.Roles);

            return updatedDTO;
        }


        /*NAPOMENA: BRISANJE NASTAVNIKA JE MOGUCE SAMO AKO NIJE DAO OCENU, nije AKTIVNO ANGAZOVAN NA PREDMETU i ako nije razredni staresina*/
        public Teacher Delete(string id)
        {
            Teacher user = db.TeachersRepository.GetByID(id);
            if (user == null)
            {
                throw new HttpException("The teacher with id: " + id + " was not found.");
            }

            IEnumerable<Mark> marksByTeacherId = db.MarksRepository.GetAllMarksByTeacherId(user.Id);
            if (marksByTeacherId.Count() != 0)
            {
                throw new HttpException("The Teacher with id: " + id + " has a list of marks in database. It can not be deleted." +
                   " If you want to stop the teacher's engagements you need to " +
                   "update Teacher's property IsStillWorking to false with HttpPut at http://localhost:54164/project/teachers/" + id + "/stopped-working");
            }

            if (user.FormAttending != null)
            {
                throw new HttpException("The teacher id: " + id + " is assigned to the form " +
                    "with id: " + user.FormAttending.Id + ". To delete this teacher, first you need to " +
                    "assign a new Attending Teacher to that Form with HttpPut and route: " +
                    "http://localhost:54164/project/forms/" + user.FormAttending.Id + "/attending-teacher/{newAttendingTeacherId} ");
            }

            IEnumerable<TeacherToSubject> teacherToSubjectByTeacherId = user.TeachersSubjects;
            List<TeacherToSubject> ongoingEngagementsByTeacherId = new List<TeacherToSubject>();

            if (teacherToSubjectByTeacherId != null)
            {
                foreach (var ts in teacherToSubjectByTeacherId)
                {
                    if (ts.StoppedTeaching == null || ts.StoppedTeaching > DateTime.UtcNow)
                    {
                        ongoingEngagementsByTeacherId.Add(ts);
                    }
                }

                if (ongoingEngagementsByTeacherId.Count != 0)
                {
                    throw new HttpException("The teacher with id: " + id + " has ongoing teaching engagements. " +
                        "To delete this teacher you need to change StoppedTeaching property for every subject that he is teaching " +
                        "in TeacherToSubject table with HttpPut at route: " +
                        "http://localhost:54164/project/teachers-to-subjects/by-teacher/" + id + "/stopped-teaching-to-now");
                }

                db.TeachersRepository.Delete(user);
                db.Save();
            }

            return user;
        }

        public IList<Teacher> GetByLastNameSorted(string lastName)
        {
            IEnumerable<Teacher> teachers = GetAll();
            List<Teacher> filtered = new List<Teacher>();

            if (teachers == null)
            {
                return null;
            }

            foreach (var teacher in teachers)
            {
                if (teacher.LastName.ToLower().Equals(lastName.ToLower()))
                {
                    filtered.Add(teacher);
                }
            }

            filtered = filtered.OrderBy(x => x.FirstName).ThenBy(x => x.UserName).ToList();
            return filtered;
        }

        public IList<Teacher> GetWorkingTeachersByLastNameSorted(string lastName)
        {
            IEnumerable<Teacher> teachers = db.TeachersRepository.GetAllStillWorkingTeachers();
            List<Teacher> filtered = new List<Teacher>();

            if (teachers == null)
            {
                return null;
            }

            foreach (var teacher in teachers)
            {
                if (teacher.LastName.ToLower().Equals(lastName.ToLower()))
                {
                    filtered.Add(teacher);
                }
            }

            filtered = filtered.OrderBy(x => x.FirstName).ThenBy(x => x.UserName).ToList();
            return filtered;
        }

        public TeacherDTOForAdmin StoppedWorkingAndEndedAllEngagementsInTSAndFTSByTeacherId(string id)
        {
            Teacher user = db.TeachersRepository.GetByID(id);
            if (user == null)
            {
                throw new HttpException("The teacher with id: " + id + " was not found.");
            }

            if (user.FormAttending != null)
            {
                throw new HttpException("The teacher id: " + id + " is assigned to the form " +
                    "with id: " + user.FormAttending.Id + ". To archive this teacher, first you need to " +
                    "assign a new Attending Teacher to that Form with HttpPut and route: " +
                    "http://localhost:54164/project/forms/" + user.FormAttending.Id + "/attending-teacher/{newAttendingTeacherId} ");
            }

            IEnumerable<TeacherToSubject> teacherToSubjectByTeacherId = user.TeachersSubjects;

            if (teacherToSubjectByTeacherId != null)
            {
                foreach (var ts in teacherToSubjectByTeacherId)
                {
                    if (ts.StoppedTeaching == null || ts.StoppedTeaching > DateTime.UtcNow)
                    {
                        ts.StoppedTeaching = DateTime.UtcNow;
                        db.TeachersToSubjectsRepository.Update(ts);
                    }

                    IEnumerable<FormToTeacherSubject> ftsBytsId = ts.TeacherSubjectForms;
                    if (ftsBytsId != null)
                    {
                        foreach (var fts in ftsBytsId)
                        {
                            if (fts.Stopped == null)
                            {
                                fts.Stopped = DateTime.UtcNow;
                                db.FormsToTeacherSubjectsRepository.Update(fts);
                            }
                        }
                    }
                }
            }

            user.IsStillWorking = false;
            db.TeachersRepository.Update(user);
            db.Save();

            TeacherDTOForAdmin updatedDTO = new TeacherDTOForAdmin();
            updatedDTO = toDTO.ConvertToTeacherDTOForAdmin(user, (List<IdentityUserRole>)user.Roles);

            return updatedDTO;
        }

        public StudentTeacherDTOItems GetTeachersByStudentUserName(string studentUserName)
        {
            Student foundStudent = db.StudentsRepository.GetByUserName(studentUserName);
            if (foundStudent == null)
            {
                throw new HttpException("The student with username: " + studentUserName + " was not found.");
            }

            IEnumerable<FormToTeacherSubject> studentTeachers = db.FormsToTeacherSubjectsRepository.GetAllByFormId(foundStudent.Form.Id);

            if (studentTeachers.Count() == 0)
            {
                throw new HttpException("Teachers list for student " + studentUserName + " is empty.");
            }

            StudentTeacherDTOItems dto = new StudentTeacherDTOItems
            {
                Id = foundStudent.Id,
                UserName = foundStudent.UserName,
                Student = foundStudent.FirstName + " " + foundStudent.LastName,
                Form = foundStudent.Form.Grade + "-" + foundStudent.Form.Tag,
                NumberOfTeachers = 0,
                Teachers = new List<TeacherDTOItem>()
            };

            foreach (var fts in studentTeachers)
            {
                TeacherDTOItem teacherDTO = ConvertToTeacherDTOItem(fts);
                dto.Teachers.Add(teacherDTO);
                dto.NumberOfTeachers++;
            }

            dto.Teachers = dto.Teachers.OrderBy(x => x.Teacher).ToList();
            return dto;
        }

        private TeacherDTOItem ConvertToTeacherDTOItem(FormToTeacherSubject fts)
        {
            if (fts == null)
            {
                return null;
            }

            TeacherDTOItem dto = new TeacherDTOItem
            {
                Id = fts.TeacherToSubject.Teacher.Id,
                Teacher = fts.TeacherToSubject.Teacher.LastName + " " + fts.TeacherToSubject.Teacher.FirstName
            };

            return dto;
        }

        public StudentTeacherDTOItems GetTeachersByStudentUserNameForParent(string studentUserName, string parentId)
        {
            Parent foundParent = db.ParentsRepository.GetByID(parentId);
            if (foundParent == null)
            {
                //ovo je nemoguce
                throw new HttpException("The parent with id: " + parentId + " was not found.");
            }

            Student foundStudent = db.StudentsRepository.GetByUserName(studentUserName);
            if (foundStudent == null)
            {
                throw new HttpException("The student with username: " + studentUserName + " was not found.");
            }

            if (foundStudent.Parent.Id != foundParent.Id)
            {
                throw new HttpException("Access Denied. We’re sorry, but you are not authorized to perform the requested operation.");
            }

            StudentTeacherDTOItems dto = GetTeachersByStudentUserName(studentUserName);
            return dto;
        }

        public IEnumerable<TeacherDTOItem> GetWorkingTeachersByFirstThreeLetters(string startsWith)
        {
            IEnumerable<Teacher> workingTeachers = db.TeachersRepository.GetWorkingTeachersByFirstThreeLetters(startsWith);
            
            if (workingTeachers.Count() == 0)
            {
                throw new HttpException("Your search didn't match any teachers.");
            }

            IList<TeacherDTOItem> dtos = new List<TeacherDTOItem>();
            foreach (var teacher in workingTeachers)
            {
                TeacherDTOItem dto = new TeacherDTOItem
                {
                    Id = teacher.Id,
                    Teacher = teacher.LastName + " " + teacher.FirstName
                };

                dtos.Add(dto);
            }

            dtos = dtos.OrderBy(x => x.Teacher).ToList();
            return dtos; 

        }
    }
}



