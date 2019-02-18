using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using eSchool.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace eSchool.Services
{
    public class MarksService : IMarksService
    {
        private IUnitOfWork db;
        private IStudentsService studentsService;
        private ITeachersToSubjectsService teachersToSubjectsService;
        private IFormsToTeacherSubjectsService formsToTeacherSubjectsService;
        private IEmailsService emailsService;
        private MarkToMarkDTO toDTO;

        public MarksService(IUnitOfWork db, IStudentsService studentsService, ITeachersToSubjectsService teachersToSubjectsService, IFormsToTeacherSubjectsService formsToTeacherSubjectsService, IEmailsService emailsService, MarkToMarkDTO toDTO)
        {
            this.db = db;
            this.studentsService = studentsService;
            this.teachersToSubjectsService = teachersToSubjectsService;
            this.formsToTeacherSubjectsService = formsToTeacherSubjectsService;
            this.emailsService = emailsService;
            this.toDTO = toDTO;
        }

        public MarkDTO ConvertToMarkDTO(int id)
        {
            Mark x = GetByID(id);
            Student foundStudent = db.StudentsRepository.GetByID(x.Student.Id);
            Subject foundSubject = db.SubjectsRepository.GetByID(x.FormToTeacherSubject.TeacherToSubject.Subject.Id);
            Teacher foundTeacher = db.TeachersRepository.GetByID(x.FormToTeacherSubject.TeacherToSubject.Teacher.Id);
            Parent foundParent = db.ParentsRepository.GetByID(x.Student.Parent.Id);
            Form foundForm = db.FormsRepository.GetByID(x.Student.Form.Id);
            Teacher foundAttending = db.TeachersRepository.GetByID(x.Student.Form.AttendingTeacher.Id);
            TeacherToSubject foundTS = db.TeachersToSubjectsRepository.GetByID(x.FormToTeacherSubject.TeacherToSubject.Id);
            FormToTeacherSubject foundFTS = db.FormsToTeacherSubjectsRepository.GetByID(x.FormToTeacherSubject.Id);

            if (x != null)
            {
                MarkDTO dto = new MarkDTO
                {
                    Id = x.Id,
                    MarkValue = x.MarkValue,
                    Semester = x.Semester,
                    Created = x.Created,

                    StudentID = foundStudent.Id,
                    Student = foundStudent.FirstName + " " + foundStudent.LastName,

                    SubjectID = foundSubject.Id,
                    SubjectName = foundSubject.Name,

                    TeacherID = foundTeacher.Id,
                    Teacher = foundTeacher.FirstName + " " + foundTeacher.LastName,

                    ParentID = foundParent.Id,
                    FormID = foundForm.Id,
                    AttendingTeacherID = foundAttending.Id,

                    TeacherToSubjectID = foundTS.Id,
                    FormToTeacherToSubjectID = foundFTS.Id

                };
                return dto;
            }
            return null;
        }

        public MarkValuesListDTO ConvertToMarkValuesListDTO(FormToTeacherSubject fts, Student student, List<Mark> marks)
        {
            IList<int> marksFirstSemester = new List<int>();
            IList<int> marksSecondSemester = new List<int>();
            foreach (var mark in marks)
            {
                if (mark.Semester == 0)
                {
                    marksFirstSemester.Add(mark.MarkValue);

                }
                else
                {
                    marksSecondSemester.Add(mark.MarkValue);
                }
            }

            MarkValuesListDTO dto = new MarkValuesListDTO
            {
                SubjectID = fts.TeacherToSubject.Subject.Id,
                SubjectName = fts.TeacherToSubject.Subject.Name,
                TeacherID = fts.TeacherToSubject.Teacher.Id,
                Teacher = fts.TeacherToSubject.Teacher.FirstName + " " + fts.TeacherToSubject.Teacher.LastName,
                ParentID = student.Parent.Id,
                StudentID = student.Id,
                Student = student.FirstName + " " + student.LastName,
                FirstSemesterMarks = marksFirstSemester,
                SecondSemesterMarks = marksSecondSemester
            };
            return dto;
        }


        public IEnumerable<Mark> GetAll()
        {
            return db.MarksRepository.Get();
        }

        public Mark GetByID(int id)
        {
            return db.MarksRepository.GetByID(id);
        }

        public IEnumerable<MarkDTO> GetAllDTOs()
        {
            IEnumerable<Mark> marks = db.MarksRepository.Get();

            if (marks != null)
            {
                IList<MarkDTO> dtos = toDTO.ConvertToMarkDTOList((List<Mark>)marks);
                return dtos;
            }
            return null;
        }

        public IEnumerable<MarkDTO> GetAllDTOsFromService()
        {
            IEnumerable<Mark> marks = db.MarksRepository.Get();

            if (marks != null)
            {
                IList<MarkDTO> dtos = new List<MarkDTO>();
                foreach (var mark in marks)
                {
                    MarkDTO dto = ConvertToMarkDTO(mark.Id);
                    dtos.Add(dto);
                }
                return dtos;
            }
            return null;
        }

        public MarkDTO GetDTOByID(int id)
        {
            Mark mark = db.MarksRepository.GetByID(id);
            if (mark != null)
            {
                MarkDTO dto = toDTO.ConvertToMarkDTO(mark);
                return dto;
            }
            return null;
        }

        public MarkDTO GetByIDDTOFromService(int id)
        {
            Mark mark = db.MarksRepository.GetByID(id);
            if (mark != null)
            {
                MarkDTO dto = ConvertToMarkDTO(mark.Id);
                return dto;

            }
            return null;

        }

        public MarkDTO Create(PostMarkDTO postDTO, string teacherId, int subjectId, int formId, string studentId)
        {
            FormToTeacherSubject foundFTS = formsToTeacherSubjectsService.FindFTSForMark(formId, teacherId, subjectId);

            Student foundStudent = studentsService.GetByID(studentId);
            if (foundStudent == null)
            {
                throw new HttpException("The student with id: " + studentId + " was not found.");
            }

            if (foundStudent.IsActive == false)
            {
                throw new HttpException("The student with id: " + studentId + " is no longer actively enrolled in this school.");
            }

            if (foundFTS.Form.Id != foundStudent.Form.Id)
            {
                throw new HttpException("The teacher (id: " + teacherId + ") does not teach the subject (" + subjectId + ") " +
                    "in the student's (id: " + studentId + ") form.");
            }

            Mark mark = new Mark
            {
                MarkValue = postDTO.MarkValue,
                Created = DateTime.UtcNow,
                FormToTeacherSubject = foundFTS,
                Student = foundStudent
            };


            if (DateTime.Today.Month > DateTime.ParseExact("Avgust", "MMMM", CultureInfo.CurrentCulture).Month)
            {
                mark.Semester = Semesters.FIRST_SEMESTER;
            }
            else
            {
                mark.Semester = Semesters.SECOND_SEMESTER;
            }

            db.MarksRepository.Insert(mark);
            db.Save();

            emailsService.NewMarkMailForParent(mark.Id);
            emailsService.NewMarkMailForStudent(mark.Id);

            MarkDTO markDto = ConvertToMarkDTO(mark.Id);
            return markDto;
        }

        public MarkDTO Update(int id, string teacherId, int value)
        {
            Mark found = db.MarksRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The mark with id: " + id + " was not found.");
            }

            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("The teacher with id: " + teacherId + " was not found.");
            }

            if (found.FormToTeacherSubject.TeacherToSubject.Teacher.Id != foundTeacher.Id)
            {
                throw new HttpException("The mark with id: " + found.Id + " was not created by the teacher with id: " + foundTeacher.Id + ".");
            }

            int oldMarkValue = found.MarkValue;

            found.MarkValue = value;
            found.Created = DateTime.UtcNow;

            db.MarksRepository.Update(found);
            db.Save();

            emailsService.MarkUpdateMailForParent(found.Id, oldMarkValue);
            emailsService.MarkUpdateMailForStudent(found.Id, oldMarkValue);

            MarkDTO markDto = ConvertToMarkDTO(found.Id);
            return markDto;
        }

        public Mark Delete(int id, string teacherId)
        {
            Mark found = db.MarksRepository.GetByID(id);
            if (found == null)
            {
                throw new HttpException("The mark with id: " + id + " was not found.");
            }

            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("The teacher with id: " + teacherId + " was not found.");
            }

            if (found.FormToTeacherSubject.TeacherToSubject.Teacher.Id != foundTeacher.Id)
            {
                throw new HttpException("The mark with id: " + found.Id + " was not created by the teacher with id: " + foundTeacher.Id + ".");
            }

            db.MarksRepository.Delete(found);
            db.Save();
            return found;
        }

        public IEnumerable<Mark> GetMarksByStudentId(string studentId)
        {
            Student foundStudent = studentsService.GetByID(studentId);

            if (foundStudent == null)
            {
                throw new HttpException("Student with id: " + studentId + " was not found.");
            }

            IEnumerable<Mark> marksByfoundStudentId = db.MarksRepository.GetAllMarksByStudentId(foundStudent.Id);
            return marksByfoundStudentId;
        }

        public IEnumerable<MarkDTO> GetMarksDTOByStudentId(string studentId)
        {
            IList<Mark> marks = (List<Mark>)GetMarksByStudentId(studentId);
            if (marks.Count == 0)
            {
                throw new HttpException("Null");
            }

            IList<MarkDTO> dtos = new List<MarkDTO>();
            foreach (var mark in marks)
            {
                MarkDTO dto = ConvertToMarkDTO(mark.Id);
                dtos.Add(dto);
            }

            dtos = dtos.OrderBy(x => x.SubjectName).ThenBy(x => x.Created).ToList();
            return dtos;

        }

        public IEnumerable<MarkDTO> GetMarksDTOByStudentIdForTeacher(string studentId, string userId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(userId);

            IList<MarkDTO> dtos = (List<MarkDTO>)GetMarksDTOByStudentId(studentId);

            IList<MarkDTO> marksByFoundTeacher = new List<MarkDTO>();
            foreach (var mark in dtos)
            {
                if (mark.TeacherID == foundTeacher.Id)
                {
                    marksByFoundTeacher.Add(mark);
                }
            }
            marksByFoundTeacher = marksByFoundTeacher.OrderBy(x => x.SubjectName).ThenBy(x => x.Created).ToList();
            return marksByFoundTeacher;
        }

        public IEnumerable<MarkDTO> GetMarksByStudentIdForSubjectId(string studentId, int subjectId)
        {
            Subject foundSubject = db.SubjectsRepository.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("Subject with id: " + subjectId + " was not found.");
            }

            IEnumerable<Mark> marksByfoundStudentId = GetMarksByStudentId(studentId);
            IList<Mark> foundMarksForSubjectId = new List<Mark>();

            if (marksByfoundStudentId != null)
            {
                foreach (var mark in marksByfoundStudentId)
                {
                    if (mark.FormToTeacherSubject.TeacherToSubject.Subject.Id == foundSubject.Id)
                    {
                        foundMarksForSubjectId.Add(mark);
                    }
                }
            }
           
            if (foundMarksForSubjectId.Count == 0)
            {
                throw new HttpException("Null");
            }

            IList<MarkDTO> dtos = new List<MarkDTO>();

            foreach (var foundMark in foundMarksForSubjectId)
            {
                MarkDTO dto = ConvertToMarkDTO(foundMark.Id);
                dtos.Add(dto);
            }

            dtos = dtos.OrderBy(x => x.Created).ToList();
            return dtos;
        }

        public MarkValuesListDTO GetMarkValuesListByStudentIdForSubjectId(string studentId, int subjectId)
        {
            Subject foundSubject = db.SubjectsRepository.GetByID(subjectId);
            if (foundSubject == null)
            {
                throw new HttpException("Subject with id: " + subjectId + " was not found.");
            }

            Student foundStudent = studentsService.GetByID(studentId);

            if (foundStudent == null)
            {
                throw new HttpException("Student with id: " + studentId + " was not found.");
            }

            IEnumerable<Mark> marksByfoundStudentId = db.MarksRepository.GetAllMarksByStudentId(foundStudent.Id);
            IList<Mark> foundMarksForSubjectId = new List<Mark>();

            if (marksByfoundStudentId != null)
            {
                foreach (var mark in marksByfoundStudentId)
                {
                    if (mark.FormToTeacherSubject.TeacherToSubject.Subject.Id == foundSubject.Id)
                    {
                        foundMarksForSubjectId.Add(mark);
                    }
                }
            }

            if (foundMarksForSubjectId.Count == 0)
            {
                throw new HttpException("Null");
            }

            FormToTeacherSubject foundFTS = db.FormsToTeacherSubjectsRepository.GetByFormIdAndSubjectId(foundStudent.Form.Id, foundSubject.Id);
            if (foundFTS == null)
            {
                return null;
            }

            MarkValuesListDTO dto = ConvertToMarkValuesListDTO(foundFTS, foundStudent, (List<Mark>)foundMarksForSubjectId);
            return dto;
        }

        public MarkValuesListDTO GetMarkValuesListByStudentIdFromTeacherId(string studentId, string teacherId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("Teacher with id: " + teacherId + " was not found.");
            }

            Student foundStudent = db.StudentsRepository.GetByID(studentId);
            if (foundStudent == null)
            {
                throw new HttpException("Student with id: " + studentId + " was not found.");
            }

            IEnumerable<Mark> marksByfoundStudentId = db.MarksRepository.GetAllMarksByStudentId(foundStudent.Id);
            IList<Mark> foundMarksFromFoundTeacherId = new List<Mark>();

            if (marksByfoundStudentId != null)
            {
                foreach (var mark in marksByfoundStudentId)
                {
                    if (mark.FormToTeacherSubject.TeacherToSubject.Teacher.Id == foundTeacher.Id)
                    {
                        foundMarksFromFoundTeacherId.Add(mark);
                    }
                }
            }

            if (foundMarksFromFoundTeacherId.Count == 0)
            {
                throw new HttpException("Null");
            }
            FormToTeacherSubject foundFTS = db.FormsToTeacherSubjectsRepository.GetByFormIdAndTeacherId(foundStudent.Form.Id, foundTeacher.Id);

            if (foundFTS == null)
            {
                return null;
            }

            MarkValuesListDTO dto = ConvertToMarkValuesListDTO(foundFTS, foundStudent, (List<Mark>)foundMarksFromFoundTeacherId);
            return dto;
        }


        public IEnumerable<MarkDTO> GetMarksDTOByTeacherId(string teacherId)
        {
            IList<Mark> marks = (List<Mark>)GetMarksByTeacherId(teacherId);
            if (marks.Count == 0)
            {
                throw new HttpException("Null");
            }

            IList<MarkDTO> dtos = new List<MarkDTO>();
            foreach (var mark in marks)
            {
                MarkDTO dto = ConvertToMarkDTO(mark.Id);
                dtos.Add(dto);
            }
            dtos = dtos.OrderBy(x => x.Student).ThenBy(x => x.Created).ToList();
            return dtos;
        }

        public IEnumerable<Mark> GetMarksByTeacherId(string teacherId)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);

            if (foundTeacher == null)
            {
                throw new HttpException("Teacher with id: " + teacherId + " was not found.");
            }

            IEnumerable<Mark> marksByfoundTeacherId = db.MarksRepository.GetAllMarksByTeacherId(foundTeacher.Id);
            return marksByfoundTeacherId;
        }

        public IEnumerable<MarkValuesListDTO> GetMarksByFormIdFromTeacherIdForSubjectId(int formId, string teacherId, int subjectId)
        {
            Form foundForm = db.FormsRepository.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The form with id: " + formId + " was not found");
            }

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

            TeacherToSubject foundTS = db.TeachersToSubjectsRepository.GetByTeacherIdAndSubjectId(foundTeacher.Id, foundSubject.Id);

            if (foundTS == null)
            {
                throw new HttpException("The subject with id: " + subjectId + " has never been assigned to " +
                    "the teacher with id: " + teacherId + ".");
            }
            if (foundTS.StoppedTeaching != null)
            {
                throw new HttpException("The teacher id: " + foundTS.Teacher.Id + " is no longer teaching the subject id " + foundSubject.Id + ". " +
                    "The teaching engagement has stopped at: " + foundTS.StoppedTeaching);
            }

            FormToTeacherSubject foundActiveFTS = db.FormsToTeacherSubjectsRepository.GetByFormIdAndTeacherSubjectId(foundForm.Id, foundTS.Id);
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

            IEnumerable<Student> formStudents = foundForm.Students;
            IList<MarkValuesListDTO> dtos = new List<MarkValuesListDTO>();

            foreach (var student in formStudents)
            {
                IEnumerable<Mark> marksByfoundFTSByStudentId = db.MarksRepository.GetByFTSIdAndStudentId(foundActiveFTS.Id, student.Id);

                MarkValuesListDTO dto = ConvertToMarkValuesListDTO(foundActiveFTS, student, (List<Mark>)marksByfoundFTSByStudentId);
                dtos.Add(dto);
            }

            dtos = dtos.OrderBy(x => x.Student).ToList();

            return dtos;
        }

        public IEnumerable<Mark> GetMarksByFormIdForAttendingTeacher(int formId)
        {
            return db.MarksRepository.GetByFormId(formId);
        }


        public IEnumerable<MarkValuesListDTO> ConvertToMarkValuesListDTOList(int formId)
        {
            Form foundForm = db.FormsRepository.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The form with id: " + formId + " was not found");
            }

            IList<MarkValuesListDTO> dtos = new List<MarkValuesListDTO>();
            IEnumerable<Student> formStudents = foundForm.Students;

            IEnumerable<FormToTeacherSubject> formFTSs = db.FormsToTeacherSubjectsRepository.GetByFormIdOnlyActive(foundForm.Id);

            foreach (var fts in formFTSs)
            {
                foreach (var student in formStudents)
                {
                    IEnumerable<Mark> studentMarks = db.MarksRepository.GetByFTSIdAndStudentId(fts.Id, student.Id);

                    MarkValuesListDTO dto = ConvertToMarkValuesListDTO(fts, student, (List<Mark>)studentMarks);
                    dtos.Add(dto);
                }
            }

            dtos = dtos.OrderBy(x => x.Student).ThenBy(x => x.SubjectName).ToList();
            return dtos;
        }

        public Mark GetFirstMarkByFormIdForAttendingTeacherValidation(int formId)
        {
            Form foundForm = db.FormsRepository.GetByID(formId);
            if (foundForm == null)
            {
                throw new HttpException("The form with id: " + formId + " was not found");
            }

            return db.MarksRepository.GetByFormId(formId).FirstOrDefault();
        }

        //REPORT CARD
        public ReportCardDTO GetReportCardForStudentId(string studentId)
        {
            Student foundStudent = db.StudentsRepository.GetByID(studentId);
            if (foundStudent == null)
            {
                throw new HttpException("Student with id: " + studentId + " was not found.");
            }

            ReportCardDTO reportCard = new ReportCardDTO
            {
                SchoolYear = foundStudent.Form.Started.Year,
                StudentId = foundStudent.Id,
                Student = foundStudent.FirstName + " " + foundStudent.LastName,
                Form = foundStudent.Form.Grade + "-" + foundStudent.Form.Tag,
                AttendingTeacher = foundStudent.Form.AttendingTeacher.Id + ", " + foundStudent.Form.AttendingTeacher.FirstName + " " + foundStudent.Form.AttendingTeacher.LastName,
                Parent = foundStudent.Parent.Id + ", " + foundStudent.Parent.FirstName + " " + foundStudent.Parent.LastName
            };

            IList<ReportCardDTOItem> classes = new List<ReportCardDTOItem>();

            IEnumerable<FormToTeacherSubject> studentFormFTSs = db.FormsToTeacherSubjectsRepository.GetByFormIdOnlyActive(foundStudent.Form.Id);
            foreach (var fts in studentFormFTSs)
            {
                IEnumerable<Mark> studentMarks = db.MarksRepository.GetByFTSIdAndStudentId(fts.Id, foundStudent.Id);
                ReportCardDTOItem item = ConvertToReportCardDTOItem(fts, studentMarks);
                classes.Add(item);
            }

            classes = classes.OrderBy(x => x.Subject).ToList();
            reportCard.Classes = classes;

            emailsService.CreateReportCardMail(foundStudent.Id, reportCard);

            return reportCard;

        }

        private ReportCardDTOItem ConvertToReportCardDTOItem(FormToTeacherSubject fts, IEnumerable<Mark> studentMarks)
        {
            IList<int> firstSemesterMarks = new List<int>();
            IList<int> secondSemesterMarks = new List<int>();

            int firstSemesterSum = 0;
            int secondSemesterSum = 0;

            foreach (var mark in studentMarks)
            {
                if (mark.Semester == 0)
                {
                    firstSemesterMarks.Add(mark.MarkValue);
                    firstSemesterSum += mark.MarkValue;
                }
                else
                {
                    secondSemesterMarks.Add(mark.MarkValue);
                    secondSemesterSum += mark.MarkValue;
                }
            }

            ReportCardDTOItem item = new ReportCardDTOItem
            {
                Subject = fts.TeacherToSubject.Subject.Id + ", " + fts.TeacherToSubject.Subject.Name,
                Teacher = fts.TeacherToSubject.Teacher.Id + ", " + fts.TeacherToSubject.Teacher.LastName + " " + fts.TeacherToSubject.Teacher.FirstName,
                FirstSemesterMarks = firstSemesterMarks,
                SecondSemesterMarks = secondSemesterMarks

            };
            if (firstSemesterMarks.Count == 0)
            {
                item.FirstSemesterAverageMark = null;
            }
            else
            {
                item.FirstSemesterAverageMark = (double)firstSemesterSum / firstSemesterMarks.Count;
            }
            if (secondSemesterMarks.Count == 0)
            {
                item.SecondSemesterAverageMark = null;
            }
            else
            {
                item.SecondSemesterAverageMark = (double)secondSemesterSum / secondSemesterMarks.Count;
            }

            return item;
        }


    }
}

