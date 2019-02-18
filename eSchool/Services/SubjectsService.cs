using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Services
{
    public class SubjectsService : ISubjectsService
    {
        private IUnitOfWork db;

        public SubjectsService(IUnitOfWork db)
        {
            this.db = db;
        }

        public IEnumerable<Subject> GetAll()
        {
            return db.SubjectsRepository.Get();
        }

        public Subject GetByID(int id)
        {
            return db.SubjectsRepository.GetByID(id);
        }

        public Subject Update(int id, PutSubjectDTO updated)
        {
            Subject found = GetByID(id);

            if (found == null)
            {
                throw new HttpException("The subject with id: " + updated.Id + " was not found.");
            }

            if (updated.Name != null)
                found.Name = updated.Name;
            if (updated.Grade != null)
                found.Grade = (int)updated.Grade;
            if (updated.NumberOfClassesPerWeek != null)
                found.NumberOfClassesPerWeek = (int)updated.NumberOfClassesPerWeek;

            db.SubjectsRepository.Update(found);

            Subject duplicate = db.SubjectsRepository.Duplicate(found.Name, found.Grade, found.NumberOfClassesPerWeek);

            if (duplicate != null && duplicate.Id != found.Id)
            {
                throw new HttpException("The subject you are creating by this update is already in the system. " +
                    "The subject id:" + duplicate.Id);
            }

            db.Save();
            return found;
        }

        public Subject Create(PostSubjectDTO newSubject)
        {
            Subject duplicate = db.SubjectsRepository.Duplicate(newSubject.Name, newSubject.Grade, newSubject.NumberOfClassesPerWeek);

            if (duplicate != null)
            {
                throw new HttpException("The Subject is already in the system with id: " + duplicate.Id + ".");
            }

            Subject subject = ConvertFromDTO(newSubject);
            db.SubjectsRepository.Insert(subject);
            db.Save();

            return subject;

        }

        private Subject ConvertFromDTO(PostSubjectDTO newSubject)
        {
            Subject x = new Subject
            {
                Name = newSubject.Name,
                Grade = newSubject.Grade,
                NumberOfClassesPerWeek = newSubject.NumberOfClassesPerWeek
            };

            return x;
        }

        /*NAPOMENA: BRISANJE PREDMETA JE MOGUCE SAMO AKO NE POSTOJI OCENA U TRENUTNOJ SKOLSKOJ GODINI IZ OVOG PREDMETA
         * AKO NASTAVNIK NIJE AKTIVNO ANGAZOVAN NA TOM PREDMETU */
        public Subject Delete(int id)
        {
            Subject found = GetByID(id);

            if (found == null)
            {
                throw new HttpException("The Subject with id: " + id + " was not found.");
            }

            Mark foundMark = db.MarksRepository.GetFirstMarkByActiveSubject(found.Id);
            if (foundMark != null)
            {
                throw new HttpException("The Subject with id: " + id + " has a list of marks in database. It can not be deleted in this school year. " +
                    "You can change StoppedTeaching property in TeacherToSubject table" +
                    " for every teacher that is still teaching this subject with HttpPut at route: " +
                    "http://localhost:54164/project/teachers-to-subjects/by-subject/" + id + "/stopped-teaching-to-now"); 
            }

            IEnumerable<TeacherToSubject> ongoingEngagementsBySubjectId = db.TeachersToSubjectsRepository.GetOngoingEngagementsBySubjectId(found.Id);

            if (ongoingEngagementsBySubjectId.Count() != 0)
            {
                throw new HttpException("The subject id " + found.Id + " is still assign to a teacher. " +
                    "To delete this subject you need to change StoppedTeaching property in TeacherToSubject table" +
                    " for every teacher that is still teaching this subject with HttpPut at route: " +
                    "http://localhost:54164/project/teachers-to-subjects/by-subject/" + id + "/stopped-teaching-to-now");

            }

            db.SubjectsRepository.Delete(found);
            db.Save();

            return found;
        }


        public IEnumerable<Subject> GetSortedSubjectsByFirstLetter(string firstLetter)
        {
            IEnumerable<Subject> filtered = db.SubjectsRepository.GetAllByFirstLetter(firstLetter);

            filtered = filtered.OrderBy(x => x.Name).ThenBy(x => x.Grade).ThenBy(x => x.NumberOfClassesPerWeek).ToList();
            return filtered;
        }

        public IEnumerable<Subject> GetSortedSubjectsByGrade(int grade)
        {
            IEnumerable<Subject> filtered = db.SubjectsRepository.GetAllByGrade(grade);

            filtered = filtered.OrderBy(x => x.Name).ThenBy(x => x.Grade).ThenBy(x => x.NumberOfClassesPerWeek).ToList();
            return filtered;
        }


    }

}

