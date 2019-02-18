using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using eSchool.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eSchool.Services
{
    public class FormsToTeacherSubjectsService : IFormsToTeacherSubjectsService
    {
        private IUnitOfWork db;
        private ITeachersService teachersService;
        private ISubjectsService subjectsService;
        private ITeachersToSubjectsService teachersToSubjectsService;
        private IFormsService formsService;
        private IEmailsService emailsService; 
        private FTSToDTO toDTO;

        public FormsToTeacherSubjectsService(IUnitOfWork db, ITeachersService teachersService, ISubjectsService subjectsService, ITeachersToSubjectsService teachersToSubjectsService, IFormsService formsService, IEmailsService emailsService, FTSToDTO toDTO)
        {
            this.db = db;
            this.teachersService = teachersService;
            this.subjectsService = subjectsService;
            this.teachersToSubjectsService = teachersToSubjectsService;
            this.formsService = formsService;
            this.emailsService = emailsService;
            this.toDTO = toDTO;
        }

        public IEnumerable<FormToTeacherSubject> GetAll()
        {
            return db.FormsToTeacherSubjectsRepository.Get();
        }

        public FormToTeacherSubject GetByID(int id)
        {
            FormToTeacherSubject found = db.FormsToTeacherSubjectsRepository.GetByID(id);
            return found;
        }

        public IEnumerable<FTSDTOForAdmin> GetAllForAdmin()
        {
            IEnumerable<FormToTeacherSubject> allFTSs = GetAll();

            if (allFTSs != null)
            {
                IList<FTSDTOForAdmin> dtos = toDTO.ConvertToFTSDTOListForAdmin((List<FormToTeacherSubject>)allFTSs);
                return dtos;
            }
            return null;
        }

        public FTSDTOForAdmin Create(int formId, string teacherId, int subjectId)
        {
            Form foundForm = formsService.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The Form with id: " + formId + " was not found");
            }

            if (foundForm.Started.AddDays(360).CompareTo(DateTime.UtcNow) < 0)
            {
                throw new HttpException("The Form with id: " + formId + " was not created for this shool year. " +
                    "This form is from: " + foundForm.Started.Year + ". Classes must be assign to a form from this school year.");
            }

            TeacherToSubject foundTeacherToSubject = teachersToSubjectsService.GetActiveTeacherToSubjectByTeacherIdAndSubjectId(teacherId, subjectId);

            if (foundForm.Grade != foundTeacherToSubject.Subject.Grade)
            {
                throw new HttpException("The subject and teacher combination with id: " + foundTeacherToSubject.Id + " has " +
                    "the subject that is taught in grade " + foundTeacherToSubject.Subject.Grade +
                    "and it can not be assigned to the grade " + foundForm.Grade + " in form " + foundForm.Id);
            }
 
            FormToTeacherSubject duplicate = db.FormsToTeacherSubjectsRepository.GetDuplicate(foundForm.Id, foundTeacherToSubject.Id);
            if (duplicate != null)
            {
                throw new HttpException("The combination form-teacher-subject you are trying to create already exists - FTS Id: " + duplicate.Id);
            }

            FormToTeacherSubject fts = new FormToTeacherSubject
            {
                Form = foundForm,
                TeacherToSubject = foundTeacherToSubject,
                Started = DateTime.UtcNow,
                Stopped = null
            };

            db.FormsToTeacherSubjectsRepository.Insert(fts);
            db.Save();

            FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(fts);
            return dto;
        }

        public FTSDTOForAdmin Update(int id, PutFormToTeacherSubjectDTO updated)
        {
            FormToTeacherSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The FormToTeacherSubject with id: " + id + " was not found");
            }

            Form foundForm = formsService.GetByID(updated.FormId);
            found.Form = foundForm ?? throw new HttpException("The Form with id: " + updated.FormId + " was not found");

            TeacherToSubject foundTS = teachersToSubjectsService.GetActiveTeacherToSubjectByTeacherIdAndSubjectId(updated.TeacherId, updated.SubjectId);
            found.TeacherToSubject = foundTS;

            found.Started = DateTime.UtcNow;
            found.Stopped = null;
          
            FormToTeacherSubject duplicate = db.FormsToTeacherSubjectsRepository.GetDuplicate(updated.FormId, foundTS.Id);

            if (duplicate != null)
            {
                throw new HttpException("The combination form-teacher-subject you are trying to create already exists - FTS Id: " + duplicate.Id);
            }
            if (foundForm.Grade != foundTS.Subject.Grade)
            {
                throw new HttpException("The subject and teacher combination with id: " + foundTS.Id + " has " +
                   "the subject that is taught in grade " + foundTS.Subject.Grade +
                   "and it can not be assigned to the grade " + foundForm.Grade + " in form " + foundForm.Id);
            }

            db.FormsToTeacherSubjectsRepository.Update(found);
            db.Save();

            FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(found);
            return dto;
        }


        public FormToTeacherSubject Delete(int id)
        {
            FormToTeacherSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form To TeacherSubject with id: " + id + " was not found.");
            }

            if (found.Marks.Count != 0)
            {
                throw new HttpException("The Form To TeacherSubject with id: " + id + " has a list of marks in database. It can not be deleted." +
                    " If you want to stop the teacher's engagement in teaching the subject in the selected form you need to " +
                    "update Form-Teacher-Subject's property Stopped with HttpPut at http://localhost:54164/project/forms-to-teachers-subjects/" + found.Id + "/stopped-to-now");
            }

            db.FormsToTeacherSubjectsRepository.Delete(found);
            db.Save();
            return found;
        }


        public FTSDTOForAdmin UpdateStarted(int id, DateTime updated)
        {
            FormToTeacherSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form To TeacherSubject with id: " + id + " was not found.");
            }

            found.Started = updated;
            db.FormsToTeacherSubjectsRepository.Update(found);
            db.Save();

            FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(found);
            return dto;
        }

        public FTSDTOForAdmin UpdateStopped(int id, DateTime updated)
        {
            FormToTeacherSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form To TeacherSubject with id: " + id + " was not found.");
            }

            found.Stopped = updated;
            db.FormsToTeacherSubjectsRepository.Update(found);
            db.Save();

            FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(found);
            return dto;
        }

        public FTSDTOForAdmin UpdateStoppedNowByFTSId(int id)
        {
            FormToTeacherSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form To TeacherSubject with id: " + id + " was not found.");
            }

            found.Stopped = DateTime.UtcNow;
            db.FormsToTeacherSubjectsRepository.Update(found);
            db.Save();

            FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(found);
            return dto;
        }

     
        public FormToTeacherSubject GetActiveFTS(int formId, int tsId)
        {
            Form foundForm = db.FormsRepository.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The Form with id: " + formId + " was not found");
            }

            TeacherToSubject foundTS = db.TeachersToSubjectsRepository.GetByID(tsId);
            if (foundTS == null)
            {
                throw new HttpException("The Teacher-Subject with id: " + tsId + " was not found");
            }

            FormToTeacherSubject foundActiveFTS = db.FormsToTeacherSubjectsRepository.Get(x => x.Form.Id == foundForm.Id && x.TeacherToSubject.Id ==
            foundTS.Id).FirstOrDefault();

            if (foundActiveFTS == null)
            {
                throw new HttpException("The teacher id: " + foundTS.Teacher.Id + " and the subject id " + foundTS.Subject.Id + " are " +
                   "not assigned to the form id: " + formId + ".");
            }
            if (foundActiveFTS.Stopped != null)
            {
                throw new HttpException("The teacher id: " + foundTS.Teacher.Id + " is no longer teaching the subject id " + foundTS.Subject.Id +
                   " to the form id: " + formId + ". The teaching engagement has stopped at: " + foundActiveFTS.Stopped);
            }
            return foundActiveFTS;

        }

        public FormToTeacherSubject FindFTSForMark(int formId, string teacherId, int subjectId)
        {
            TeacherToSubject foundTeacherToSubject = teachersToSubjectsService.GetActiveTeacherToSubjectByTeacherIdAndSubjectId(teacherId, subjectId);

            FormToTeacherSubject foundActiveFTS = GetActiveFTS(formId, foundTeacherToSubject.Id);

            return foundActiveFTS;
        }

        public IEnumerable<FTSDTOForAdmin> GetAllByTeacherId(string teacherId)
        {
            IEnumerable<FormToTeacherSubject> foundByTeacherId = db.FormsToTeacherSubjectsRepository.GetAllByTeacherId(teacherId);

            if (foundByTeacherId != null)
            {
                IList<FTSDTOForAdmin> dtos = toDTO.ConvertToFTSDTOListForAdmin((List<FormToTeacherSubject>)foundByTeacherId);
                return dtos;
            }
            return null;
        }

        public IEnumerable<FTSDTOForAdmin> GetAllByTeacherSubjectId(int tsId)
        {
            IEnumerable<FormToTeacherSubject> foundByTeacherSubjectId = db.FormsToTeacherSubjectsRepository.GetAllByTeacherSubjectId(tsId);

            if (foundByTeacherSubjectId != null)
            {
                IList<FTSDTOForAdmin> dtos = toDTO.ConvertToFTSDTOListForAdmin((List<FormToTeacherSubject>)foundByTeacherSubjectId);
                return dtos;
            }
            return null;
        }

        public TeachersWeeklyEngagementsDTO GetWorkingTeachersWeeklyEngagements(string adminId)
        {         
            Admin admin = db.AdminsRepository.GetByID(adminId);
            //sanse za ovo su nepostojece
            if (admin.Email == null)
            {
                throw new HttpException("Please register you email, so that we can send you the report.");
            }

            IEnumerable<FormToTeacherSubject> activeFTS = db.FormsToTeacherSubjectsRepository.GetAllActive();
            if (activeFTS == null)
            {
                return null;
            }

            IEnumerable<Teacher> workingTeachers = db.TeachersRepository.GetAllStillWorkingTeachers();
            if (workingTeachers == null)
            {
                return null;
            }

            IList<TeacherIDWeeklyEngagementsDTO> weeklyEngagementsByTeachers = new List<TeacherIDWeeklyEngagementsDTO>();

            foreach (var teacher in workingTeachers)
            {
                TeacherIDWeeklyEngagementsDTO reportByTeacherId = new TeacherIDWeeklyEngagementsDTO(); 
                reportByTeacherId.TeacherID = teacher.Id;
                reportByTeacherId.Teacher = teacher.FirstName + " " + teacher.LastName;

                int sumEngagements = 0; 
                foreach (var fts in activeFTS)
                {
                    if (fts.TeacherToSubject.Teacher.Id == teacher.Id)
                    {
                        sumEngagements += fts.TeacherToSubject.Subject.NumberOfClassesPerWeek;
                    }  
                }
                reportByTeacherId.WeeklyEngagements = sumEngagements;

                weeklyEngagementsByTeachers.Add(reportByTeacherId);
            }

            TeachersWeeklyEngagementsDTO report = new TeachersWeeklyEngagementsDTO
            {
                OnDate = DateTime.UtcNow,
                WeeklyEngagementsByTeachers = weeklyEngagementsByTeachers
            };

            emailsService.CreateMailForAdminTeachersEngagementsReport(report, admin.Id);    
            return report;

        }

    }
}

