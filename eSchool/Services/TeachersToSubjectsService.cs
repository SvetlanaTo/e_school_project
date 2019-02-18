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
    public class TeachersToSubjectsService : ITeachersToSubjectsService
    {
        private IUnitOfWork db;
        private ITeachersService teachersService;
        private ISubjectsService subjectsService;
        private TeacherSubjectToDTO toDTO;

        public TeachersToSubjectsService(IUnitOfWork db, ITeachersService teachersService, ISubjectsService subjectsService, TeacherSubjectToDTO toDTO)
        {
            this.db = db;
            this.teachersService = teachersService;
            this.subjectsService = subjectsService;
            this.toDTO = toDTO;
        }

        public IEnumerable<TeacherToSubject> GetAll()
        {
            return db.TeachersToSubjectsRepository.Get();
        }

        public TeacherToSubject GetByID(int id)
        {
            return db.TeachersToSubjectsRepository.GetByID(id);
        }

        public TeacherToSubject GetTeacherToSubjectByTeacherIdAndSubjectId(string teacherId, int subjectId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("The teacher with id: " + teacherId + " was not found");
            }

            Subject foundSubject = db.SubjectsRepository.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " was found");
            }

            TeacherToSubject found = db.TeachersToSubjectsRepository.GetByTeacherIdAndSubjectId(foundTeacher.Id, foundSubject.Id);

            if (found == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " is not assigned to " +
                    "the teacher with id: " + teacherId + ".");
            }

            return found;
        }

        public TeacherToSubject GetActiveTeacherToSubjectByTeacherIdAndSubjectId(string teacherId, int subjectId)
        {
            TeacherToSubject found = GetTeacherToSubjectByTeacherIdAndSubjectId(teacherId, subjectId);

            if (found.StoppedTeaching != null)
            {
                throw new HttpException("The teacher with id " + teacherId + " has stopped teaching the subject " +
                    "with id: " + subjectId + ". You need to renew the teaching contract.");
            }

            return found;
        }


        public TeacherToSubjectDTOForAdmin Create(string teacherId, int subjectId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("The teacher with id: " + teacherId + " was not found");
            }
            if (foundTeacher.IsStillWorking == false)
            {
                throw new HttpException("The teacher with id: " + teacherId + " is currently not an active faculty member. " +
                    "You cannot assign this teacher to a subject.");
            }

            Subject foundSubject = subjectsService.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " was found");
            }

            TeacherToSubject found = db.TeachersToSubjectsRepository.GetByTeacherIdAndSubjectId(foundTeacher.Id, foundSubject.Id);

            if (found != null)
            {
                throw new HttpException("The subject with id: " + subjectId + " is already assign to " +
                    "the teacher with id: " + teacherId + ". You can find the existing Teacher-Subject combination (" + found.Id +
                    ") with HttpGet at route: http://localhost:54164/project/teachers-to-subjects/" + found.Id);
            }

            TeacherToSubject ts = new TeacherToSubject
            {
                Teacher = foundTeacher,
                Subject = foundSubject,

                StartedTeaching = DateTime.UtcNow,
                StoppedTeaching = null
            };

            db.TeachersToSubjectsRepository.Insert(ts);
            db.Save();

            TeacherToSubjectDTOForAdmin dto = toDTO.ConvertToTeacherToSubjectDTOForAdmin(ts);
            return dto;
        }

        public TeacherToSubjectDTOForAdmin UpdateStartedTeaching(int id, DateTime updated)
        {
            TeacherToSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Teacher-Subject combination with id: " + id + " was not found.");
            }

            if (found.StoppedTeaching != null && found.StoppedTeaching < updated)
            {
                throw new HttpException("The StartedTeaching date must be before StoppedTeaching Date: " + found.StoppedTeaching);
            }

            found.StartedTeaching = updated;
            db.TeachersToSubjectsRepository.Update(found);
            db.Save();

            TeacherToSubjectDTOForAdmin dto = toDTO.ConvertToTeacherToSubjectDTOForAdmin(found);
            return dto;
        }

        public TeacherToSubjectDTOForAdmin UpdateStoppedTeaching(int id, DateTime updated)
        {
            TeacherToSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Teacher-Subject combination with id: " + id + " was not found.");
            }

            if (found.StartedTeaching > updated)
            {
                throw new HttpException("The StoppedTeaching date must be after StartedTeaching Date: " + found.StartedTeaching);
            }

            found.StoppedTeaching = updated;
            db.TeachersToSubjectsRepository.Update(found);
            db.Save();

            TeacherToSubjectDTOForAdmin dto = toDTO.ConvertToTeacherToSubjectDTOForAdmin(found);
            return dto;
        }

        public IEnumerable<TeacherToSubject> GetSubjectsByTeacherId(string teacherId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);

            if (foundTeacher == null)
            {
                throw new HttpException("Teacher with id: " + teacherId + " was not found");
            }

            IEnumerable<TeacherToSubject> teacherSubjects = foundTeacher.TeachersSubjects;

            return teacherSubjects;

        }

        public IEnumerable<TeacherToSubject> GetTeachersBySubjectId(int subjectId)
        {
            Subject foundSubject = subjectsService.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("Subject with id: " + subjectId + " was not found");
            }

            IEnumerable<TeacherToSubject> subjectsTeachers = foundSubject.SubjectsTeachers;
            List<TeacherToSubject> sorted = subjectsTeachers.OrderBy(x => x.Teacher.LastName).ThenBy(x => x.Teacher.FirstName).ToList();

            return sorted;
        }

        public IEnumerable<TeacherToSubject> UpdateStoppedTeachingToNowForAllSubjectsByTeacherId(string teacherId)
        {
            IEnumerable<TeacherToSubject> teachersSubs = db.TeachersToSubjectsRepository.GetOngoingEngagementsByTeacherId(teacherId);

            if (teachersSubs.Count() == 0)
            {
                throw new HttpException("The teacher with id: " + teacherId + " doesn't have assigned subjects.");
            }

            foreach (var ts in teachersSubs)
            {
                ts.StoppedTeaching = DateTime.UtcNow;
                db.TeachersToSubjectsRepository.Update(ts);

                IEnumerable<FormToTeacherSubject> ftsBytsId = db.FormsToTeacherSubjectsRepository.GetAllActiveByTSId(ts.Id);
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

            db.Save();
            return teachersSubs;
        }

        public IEnumerable<TeacherToSubject> UpdateStoppedTeachingToNowForAllTeachersBySubjectId(int subjectId)
        {
            IEnumerable<TeacherToSubject> subjectTeachers = db.TeachersToSubjectsRepository.GetOngoingEngagementsBySubjectId(subjectId);

            if (subjectTeachers.Count() == 0)
            {
                throw new HttpException("The subject with id: " + subjectId + " is not assigned to a teacher.");
            }

            foreach (var ts in subjectTeachers)
            {
                ts.StoppedTeaching = DateTime.UtcNow;
                db.TeachersToSubjectsRepository.Update(ts);

                IEnumerable<FormToTeacherSubject> ftsBytsId = db.FormsToTeacherSubjectsRepository.GetAllActiveByTSId(ts.Id);
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

            db.Save();
            return subjectTeachers;
        }

        public TeacherToSubjectDTOForAdmin PutStoppedTeachingNowByTSId(int id)
        {
            TeacherToSubject found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Teacher-Subject combination with id: " + id + " was not found.");
            }

            found.StoppedTeaching = DateTime.UtcNow;
            db.TeachersToSubjectsRepository.Update(found);

            IEnumerable<FormToTeacherSubject> ftsByFoundTSId = found.TeacherSubjectForms;
            if (ftsByFoundTSId != null)
            {
                foreach (var fts in ftsByFoundTSId)
                {
                    if (fts.Stopped == null)
                    {
                        fts.Stopped = DateTime.UtcNow;
                        db.FormsToTeacherSubjectsRepository.Update(fts);
                    }
                }
            }

            db.Save();

            TeacherToSubjectDTOForAdmin dto = toDTO.ConvertToTeacherToSubjectDTOForAdmin(found);
            return dto;
        }


        public TeacherToSubject Delete(int id)
        {
            TeacherToSubject found = GetByID(id);

            if (found == null)
            {
                throw new HttpException("The TeacherToSubject with id: " + id + " was not found.");
            }

            IEnumerable<Mark> marksByFoundTS = db.MarksRepository.GetAllByTSId(id);

            if (marksByFoundTS.Count() != 0)
            {
                throw new HttpException("The TeacherSubject with id: " + id + " has a list of marks in database. It can not be deleted." +
                   " If you want to stop the teacher's engagement in teaching the subject (and with that, stop his teaching engagemets in all forms)" +
                   " you need to update Teacher-Subject's property StoppedTeaching with HttpPut " +
                   "at http://localhost:54164/project/teachers-to-subjects/" + found.Id + "/stopped-teaching-now");
            }


            db.TeachersToSubjectsRepository.Delete(found);
            db.Save();
            return found;

        }

        public IEnumerable<TeacherToSubject> GetSubjectsByActiveOrInactiveTeacherId(string teacherId, string active)
        {
            Teacher found = db.TeachersRepository.GetByID(teacherId);
            if (found == null)
            {
                throw new HttpException("Teacher with id: " + teacherId + " was not found");
            }

            if (active.ToLower().Equals("active"))
            {
                return db.TeachersToSubjectsRepository.GetOngoingEngagementsByTeacherId(found.Id);

            }
            else //if (active.ToLower().Equals("inactive"))
            {
                return db.TeachersToSubjectsRepository.GetStoppedEngagementsByTeacherId(found.Id);
            }

        }

        public IEnumerable<TeacherToSubject> GetActiveOrInactiveTeachersBySubjectId(int subjectId, string active)
        {
            Subject found = db.SubjectsRepository.GetByID(subjectId);
            if (found == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " was not found");
            }

            if (active.ToLower().Equals("active"))
            {
                return db.TeachersToSubjectsRepository.GetOngoingEngagementsBySubjectId(found.Id);
            }
            else //if (active.ToLower().Equals("inactive"))
            {
                return db.TeachersToSubjectsRepository.GetStoppedEngagementsBySubjectId(found.Id);
            }

        }

        public IEnumerable<TeacherToSubject> GetActiveTeachersForGrade(int grade)
        {
            return db.TeachersToSubjectsRepository.GetActiveTeachersByGrade(grade);

        }

        public IEnumerable<TeacherToSubject> GetTeachersForSubjectByDate(int subjectId, DateTime startDate, DateTime endDate)
        {
            Subject found = db.SubjectsRepository.GetByID(subjectId);
            if (found == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " was not found");
            }

            return db.TeachersToSubjectsRepository.GetTeachersForSubjectByDatePeriod(found.Id, startDate, endDate);

        }

        public SubjectTeacherSubjectDTOItems GetTeacherDTOListBySubjectId(int subjectId)
        {
            Subject foundSubject = db.SubjectsRepository.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("Subject with id: " + subjectId + " was not found");
            }

            IEnumerable<TeacherToSubject> subjectsTeachers = foundSubject.SubjectsTeachers;
           
            SubjectTeacherSubjectDTOItems dto = new SubjectTeacherSubjectDTOItems
            {
                SubjectId = foundSubject.Id,
                Name = foundSubject.Name,
                Grade = foundSubject.Grade,
                NumberOfClassesPerWeek = foundSubject.NumberOfClassesPerWeek,
                Teachers = new List<TeacherSubjectDTOItemForSubject>()
            };

            foreach (var ts in subjectsTeachers)
            {
                TeacherSubjectDTOItemForSubject teacherDTO = ConvertToTeacherSubjectDTOItemForSubject(ts);
                dto.Teachers.Add(teacherDTO);
            }

            dto.Teachers = dto.Teachers.OrderBy(x => x.Teacher).ToList();
            return dto;

        }

        private TeacherSubjectDTOItemForSubject ConvertToTeacherSubjectDTOItemForSubject(TeacherToSubject ts)
        {
            if (ts == null)
            {
                return null;
            }

            TeacherSubjectDTOItemForSubject dto = new TeacherSubjectDTOItemForSubject
            {
                TeacherId = ts.Teacher.Id,
                Teacher = ts.Teacher.LastName + " " + ts.Teacher.FirstName,
                StartedTeaching = ts.StartedTeaching,
                StoppedTeaching = ts.StoppedTeaching
            };

            return dto;
        }

        public TeacherTeacherSubjectDTOItems GetSubjectDTOListByTeacherId(string teacherId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("Teacher with id: " + teacherId + " was not found"); 
            }

            IEnumerable<TeacherToSubject> teacherSubjects = foundTeacher.TeachersSubjects;

            TeacherTeacherSubjectDTOItems dto = new TeacherTeacherSubjectDTOItems
            {
                TeacherId = foundTeacher.Id,
                Teacher = foundTeacher.LastName + " " + foundTeacher.FirstName,
                Subjects = new List<TeacherSubjectDTOItemForTeacher>() 
            };

            foreach (var ts in teacherSubjects)
            {
                TeacherSubjectDTOItemForTeacher subjectDTO = ConvertToTeacherSubjectDTOItemForTeacher(ts);
                dto.Subjects.Add(subjectDTO);
            }

            dto.Subjects = dto.Subjects.OrderBy(x => x.Subject).ToList();
            return dto;

        }

        private TeacherSubjectDTOItemForTeacher ConvertToTeacherSubjectDTOItemForTeacher(TeacherToSubject ts)
        {
            if (ts == null)
            {
                return null;
            }

            TeacherSubjectDTOItemForTeacher dto = new TeacherSubjectDTOItemForTeacher
            {
                SubjectId = ts.Subject.Id,
                Subject = ts.Subject.Name,
                StartedTeaching = ts.StartedTeaching,
                StoppedTeaching = ts.StoppedTeaching 
            };

            return dto;
        }
    }
}








