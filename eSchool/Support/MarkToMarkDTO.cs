using eSchool.Models;
using eSchool.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSchool.Support
{
    public class MarkToMarkDTO
    {

        public MarkDTO ConvertToMarkDTO(Mark x)
        {
            MarkDTO dto = new MarkDTO
            {
                Id = x.Id,
                MarkValue = x.MarkValue,
                Semester = x.Semester,
                Created = x.Created,

                StudentID = x.Student.Id,
                Student = x.Student.FirstName + " " + x.Student.LastName,

                SubjectID = x.FormToTeacherSubject.TeacherToSubject.Subject.Id,
                SubjectName = x.FormToTeacherSubject.TeacherToSubject.Subject.Name,

                TeacherID = x.FormToTeacherSubject.TeacherToSubject.Teacher.Id,
                Teacher = x.FormToTeacherSubject.TeacherToSubject.Teacher.FirstName + " " + x.FormToTeacherSubject.TeacherToSubject.Teacher.LastName,

                ParentID = x.Student.Parent.Id,
                FormID = x.Student.Form.Id,
                AttendingTeacherID = x.Student.Form.AttendingTeacher.Id,

                TeacherToSubjectID = x.FormToTeacherSubject.TeacherToSubject.Id,
                FormToTeacherToSubjectID = x.FormToTeacherSubject.Id

            };
            return dto;
        }

        public IList<MarkDTO> ConvertToMarkDTOList(List<Mark> marks)
        {
            IList<MarkDTO> dtos = new List<MarkDTO>();
            foreach (var mark in marks)
            {
                MarkDTO dto = ConvertToMarkDTO(mark);
                dtos.Add(dto);
            }
            return dtos;
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

        
    }
}