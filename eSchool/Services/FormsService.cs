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
    public class FormsService : IFormsService 
    {
        private IUnitOfWork db;
        private FormToFormDTO toDTO;

        public FormsService(IUnitOfWork db, FormToFormDTO toDTO)
        {
            this.db = db;
            this.toDTO = toDTO;
        }

        public IEnumerable<Form> GetAll()
        {
            return db.FormsRepository.Get();
        }

        public Form GetByID(int id)
        {
            return db.FormsRepository.GetByID(id);
        }

        public IEnumerable<FormDTOForAdmin> GetAllForAdmin()
        {
            IEnumerable<Form> forms = GetAll();

            if (forms != null)
            {
                IList<FormDTOForAdmin> dtos = toDTO.ConvertToFormDTOListForAdmin((List<Form>)forms);
                return dtos;
            }
            return null;
        }

        public IEnumerable<FormDTOForTeacher> GetAllForTeacher()
        {
            IEnumerable<Form> forms = GetAll();

            if (forms != null)
            {
                IList<FormDTOForTeacher> dtos = toDTO.ConvertToFormDTOListForTeacher((List<Form>)forms);
                return dtos;
            }
            return null;
        }

        public IEnumerable<FormDTOForStudentAndParents> GetAllForStudentFromStudentForm(string userId)
        {
            Student foundUser = db.StudentsRepository.GetByID(userId);
            if (foundUser == null)
            {
                return null;
            }
            Form usersForm = GetByID(foundUser.Form.Id);

            if (usersForm == null)
            {
                return null;
            }

            FormDTOForStudentAndParents dto = toDTO.ConvertToFormDTOForStudentAndParent(usersForm);

            //mora se vratiti lista
            IList<FormDTOForStudentAndParents> dtos = new List<FormDTOForStudentAndParents>
            {
                dto
            };

            return dtos;
        }

        public IEnumerable<FormDTOForStudentAndParents> GetAllForParentFromStudentsForms(string userId)
        {
            Parent foundUser = db.ParentsRepository.GetByID(userId);
            if (foundUser == null)
            {
                return null;
            }

            IEnumerable<Student> children = foundUser.Students;
            if (children.Count() == 0) 
            {
                return null;
            }

            IList<Form> uniqueForms = new List<Form>();
            foreach (var child in children)
            {
                Form childsForm = db.FormsRepository.GetByID(child.Form.Id);
                if (!uniqueForms.Contains(childsForm))
                {
                    uniqueForms.Add(childsForm);
                }
            }

            IList<FormDTOForStudentAndParents> dtos = toDTO.ConvertToFormDTOListForStudentAndParent((List<Form>)uniqueForms);
            return dtos;
        }

        public FormDTOForAdmin Update(int id, PutFormDTO updated)
        {
            Form found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form with id: " + updated.Id + " was not found.");
            }

            if (updated.Grade != null)
                found.Grade = (int)updated.Grade;
            if (updated.Tag != null)
                found.Tag = updated.Tag;
            if (updated.Started != null)
                found.Started = (DateTime)updated.Started;
            if (updated.AttendingTeacherId != null)
            {
                Teacher foundTeacher = db.TeachersRepository.GetByID(updated.AttendingTeacherId);
                if (foundTeacher == null)
                {
                    throw new HttpException("Attending teacher with id: " + updated.AttendingTeacherId + " was not found.");
                }

                if (foundTeacher.FormAttending != null && foundTeacher.FormAttending.Id != found.Id)
                {
                    throw new HttpException("The teacher id " + updated.AttendingTeacherId + " is already assigned to the form " +
                        "with id: " + foundTeacher.FormAttending.Id + ". The teacher can only attend one form at a time.");
                }
                if (foundTeacher.IsStillWorking == false)
                {
                    throw new HttpException("The teacher id " + foundTeacher.Id + " is no longer working in this shool. " +
                        "You must assing someone who is still working.");
                }

                found.AttendingTeacher = foundTeacher;
            }

            db.FormsRepository.Update(found);

            Form duplicate = db.FormsRepository.GetDuplicate(found.Grade, found.Tag, found.Started.Year);

            if (duplicate != null && duplicate.Id != found.Id)
            {
                throw new HttpException("The form you are creating by this update is already in the system. " +
                    "The form id:" + duplicate.Id);
            }

            db.Save();

            FormDTOForAdmin updatedDTO = new FormDTOForAdmin();
            updatedDTO = toDTO.ConvertToFormDTOForAdmin(found);
            return updatedDTO;
        }

        public FormDTOForAdmin Create(PostFormDTO newForm)
        {
            Teacher attendingTeacher = db.TeachersRepository.GetByID(newForm.AttendingTeacherId);
            if (attendingTeacher == null)
            {
                throw new HttpException("Attending teacher with id: " + newForm.AttendingTeacherId + " was not found.");
            }

            if (attendingTeacher.FormAttending != null)
            {
                throw new HttpException("The teacher id " + newForm.AttendingTeacherId + " is already assigned to a form " +
                    "with id: " + attendingTeacher.FormAttending.Id + ". The teacher can only attend one form at a time.");
            }

            if (attendingTeacher.IsStillWorking == false)
            {
                throw new HttpException("The teacher id " + attendingTeacher.Id + " is no longer working in this shool. " +
                    "You must assing someone who is still working.");
            }

            Form form = ConvertFromDTO(newForm, attendingTeacher);

            db.FormsRepository.Insert(form);

            Form duplicate = db.FormsRepository.GetDuplicate(form.Grade, form.Tag, form.Started.Year);

            if (duplicate != null)
            {
                throw new HttpException("The form you are trying to create is already in the system. " +
                    "The form id:" + duplicate.Id);
            }

            db.Save();

            FormDTOForAdmin dto = toDTO.ConvertToFormDTOForAdmin(form);
            return dto;
        }

        private Form ConvertFromDTO(PostFormDTO newForm, Teacher attendingTeacher)
        {
            Form x = new Form
            {
                Grade = newForm.Grade,
                Tag = newForm.Tag,
                Started = newForm.Started,
                AttendingTeacher = attendingTeacher
            };

            return x;
        }

        public FormDTOForAdmin ChangeAttendingTeacher(int id, string teacherId)
        {
            Form found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form with id: " + id + " was not found.");
            }

            Teacher foundTeacher = db.TeachersRepository.GetByID(teacherId);
            if (foundTeacher == null)
            {
                throw new HttpException("Attending teacher with id: " + teacherId + " was not found.");
            }

            if (foundTeacher.FormAttending != null && foundTeacher.FormAttending.Id != found.Id)
            {
                throw new HttpException("The teacher id " + teacherId + " is already assigned to a form " +
                   "with id: " + foundTeacher.FormAttending.Id + ". The teacher can only attend one form at a time.");
            }

            if (foundTeacher.IsStillWorking == false)
            {
                throw new HttpException("The teacher id " + foundTeacher.Id + " is no longer working in this shool. " +
                    "You must assing someone who is still working.");
            }

            found.AttendingTeacher = foundTeacher;
            db.FormsRepository.Update(found);
            db.Save();

            FormDTOForAdmin dto = toDTO.ConvertToFormDTOForAdmin(found);
            return dto;
        }

        public Form AddStudent(int id, string studentId)
        {
            Form found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form with id: " + id + " was not found.");
            }
            
            if (found.Started.AddDays(360).CompareTo(DateTime.UtcNow) < 0 )
            {
                throw new HttpException("The Form with id: " + id + " was not created for this shool year. " +
                    "This form is from: " + found.Started.Year + ". Students must be assign to a form from this school year.");
            }

            Student foundStudent = db.StudentsRepository.GetByID(studentId);
            if (foundStudent == null)
            {
                throw new HttpException("Student with id: " + studentId + " was not found.");
            }

            if (foundStudent.IsActive == false)
            {
                throw new HttpException("The student with id: " + foundStudent.Id + " is no longer actively enrolled in this school.");
            }

            foundStudent.Form = found; 

            db.StudentsRepository.Update(foundStudent);
            db.Save();

            return found;
        }

        public Form Delete(int id)
        {
            Form form = db.FormsRepository.GetByID(id);
            if (form == null)
            {
                throw new HttpException("The form with id: " + id + " was not found.");
            }

            List<Student> formStudents = (List<Student>)form.Students;

            if (formStudents.Count != 0)
            {
                throw new HttpException("The form id: " + id + " has " + formStudents.Count + " students assigned " +
                    "to it. For more info go to HttpGet at route: " +
                    "http://localhost:54164/project/forms/" + form.Id + "/students . To delete this form, " +
                    "you need to assign students to a different form with HttpPut and route: " +
                    "http://localhost:54164/project/students/{id}/add-to-form/{formId:int} " +
                    " . Thank you for your cooperation.");
            }

            db.FormsRepository.Delete(form);
            db.Save();

            return form;
        }

        public FormIdStudentsDTO GetSortedStudentsNamesByFormId(int id)
        {
            Form found = GetByID(id);
            if (found == null)
            {
                throw new HttpException("The Form with id: " + id + " was not found.");
            }

            IEnumerable<Student> students = found.Students;
            if (students.Count() == 0)
            {
                throw new HttpException("Student list is empty.");
            }

            FormIdStudentsDTO dto = new FormIdStudentsDTO
            {
                Id = found.Id,
                Grade = found.Grade,
                Tag = found.Tag,
                Started = found.Started,
                AttendingTeacher = found.AttendingTeacher.FirstName + " " + found.AttendingTeacher.LastName,
                NumberOfStudents = 0,
                Students = new List<FormStudentDTO>()
            };

            foreach (var student in students)
            {
                FormStudentDTO studentDTO = ConvertToFormStudentDTO(student);
                dto.Students.Add(studentDTO);
                dto.NumberOfStudents++; 
            }

            dto.Students = dto.Students.OrderBy(x => x.Student).ThenBy(x => x.Id).ToList();
            return dto;
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

        public FormIdStudentsDTO GetSortedStudentsNamesByFormIdForParent(int formId, string parentId)
        {
            Parent foundParent = db.ParentsRepository.GetByID(parentId);
            if (foundParent == null)
            {
                //sanse da ulogovani korisnik ne postoji su nepostojece :)
                throw new HttpException("The User with id: " + parentId + " was not found.");
            }

            Form found = GetByID(formId);
            if (found == null)
            {
                throw new HttpException("The Form with id: " + formId + " was not found.");
            }

            IEnumerable<Student> students = found.Students;
            if (students.Count() == 0)
            {
                throw new HttpException("Student list is empty.");
            }
            if (students.Any(x => x.Parent.Id == parentId) == false)
            {
                throw new HttpException("Access Denied. We’re sorry, but you are not authorized to perform the requested operation.");
            }

            FormIdStudentsDTO dto = new FormIdStudentsDTO
            {
                Id = found.Id,
                Grade = found.Grade,
                Tag = found.Tag,
                Started = found.Started,
                AttendingTeacher = found.AttendingTeacher.FirstName + " " + found.AttendingTeacher.LastName,
                NumberOfStudents = 0,
                Students = new List<FormStudentDTO>()
            };

            foreach (var student in students)
            {
                FormStudentDTO studentDTO = ConvertToFormStudentDTO(student);
                dto.Students.Add(studentDTO);
                dto.NumberOfStudents++;
            }

            dto.Students = dto.Students.OrderBy(x => x.Student).ThenBy(x => x.Id).ToList();
            return dto;
        }

        public IEnumerable<FormDTOForAdmin> GetSortedFormsByGradeByYear(int grade, int year)
        {
            IEnumerable<Form> filtered = db.FormsRepository.GetAllByGradeByYear(grade, year);

            filtered = filtered.OrderBy(x => x.Tag).ThenBy(x => x.Started).ToList();

            if (filtered.Count() == 0)
            {
                throw new HttpException("Your search for grade: " + grade + " and year: " + year + " didn't match any forms.");
            }

            IList<FormDTOForAdmin> dtos = toDTO.ConvertToFormDTOListForAdmin((List<Form>)filtered); 
            return dtos;
        }

        public FormDTOForAdmin GetFormByAttendingTeacherLastName(string teacherUserName)
        {
            Teacher foundTeacher = db.TeachersRepository.GetByUserName(teacherUserName);  
            if (foundTeacher == null)
            {
                throw new HttpException("The Teacher with user name: " + teacherUserName + " was not found."); 
            }

            Form foundForm = db.FormsRepository.GetByAttendingTeacherId(foundTeacher.Id);
            if (foundForm == null)
            {
                throw new HttpException("Teacher " + foundTeacher.FirstName + " " + foundTeacher.LastName + " is currenty not assign to any form.");
            }

            FormDTOForAdmin dto = toDTO.ConvertToFormDTOForAdmin(foundForm);
            return dto; 
        }
    }
}

