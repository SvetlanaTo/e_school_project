using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace eSchool.Services
{
    public class EmailsService : IEmailsService
    {
        private IUnitOfWork db;

        public EmailsService(IUnitOfWork db)
        {
            this.db = db;
        }

        private void CreateMail(string newSubject, string newBody, string newEmailTo, bool isBodyHtml)
        {
            string subject = newSubject;
            string body = newBody;

            string FromMail = ConfigurationManager.AppSettings["from"];
            string emailTo = newEmailTo;

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["smtpServer"]);

            mail.From = new MailAddress(FromMail);
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;

            SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["from"], ConfigurationManager.AppSettings["password"]);
            SmtpServer.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]);

            SmtpServer.Send(mail);
        }


        public void CreateMailForUserUpdate(string id)
        {
            ApplicationUser found = db.UsersRepository.GetByID(id);

            if (found != null)
            {
                string subject = "Account Updated";
                string body = $"<p>Dear {found.FirstName} {found.LastName},<br/>" +
                    $"We are writing to let you know that your E-School account has been updated.<br/> " +
                    $"You can see your new account information by logging in at E-School." +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                   $"Svetlana Topalov, E-School";

                string emailTo = found.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);

            }
        }

        public void CreateMailForParentForStudentUpdate(string studentId)
        {
            Student student = db.StudentsRepository.GetByID(studentId);
            if (student != null)
            {
                Parent found = db.ParentsRepository.GetByID(student.Parent.Id);
                if (found != null)
                {
                    string subject = "Student Account Updated";
                    string body = $"<p>Dear {found.FirstName} {found.LastName},<br/>" +
                        $"We are writing to let you know that the student account that is assigned to your name at E-School" +
                        $" has been updated.<br/> " +
                        $"(Student: {student.FirstName} {student.LastName})<br/> " +
                        $"You can see new account information by logging in at E-School." +
                        $"<br/>" +
                        $"<br/>" +
                        $"Sincerely,<br/>" +
                        $"Svetlana Topalov, E-School";

                    string emailTo = found.Email;
                    bool isBodyHtml = true;

                    CreateMail(subject, body, emailTo, isBodyHtml);
                }
            }
        }

        public void CreateRegistrationMail(string id, string password)
        {
            ApplicationUser found = db.UsersRepository.GetByID(id);

            if (found != null)
            {
                string subject = "E-School Account Created";
                string body = $"<p>Dear {found.FirstName} {found.LastName},<br/>" +
                    $"We are writing to let you know that the account was made in your name at E-School.<br/> " +
                    $"<br/>" +
                    $"Your account details:<br/>" +
                    $"UserName: the email address to which this message was sent.</b><br/>" +
                    $"Password: {password}<br/></p>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = found.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);

            }
        }

        public void CreateRegistrationMailForAdminOrTeacher(string id)
        {
            ApplicationUser found = db.UsersRepository.GetByID(id);

            if (found != null)
            {
                string subject = "E-School Account Created";
                string body = $"<p>Dear {found.FirstName} {found.LastName},<br/>" +
                    $"We are writing to let you know that the account was made in your name at E-School.<br/> " +
                    $"<br/>" +
                    $"To receive your account information for logging into E-School, contact your Supervisor.<br/> " +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = found.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);

            }

        }

        public void CreateMailForParentNewStudentAssigned(string id)
        {
            Parent found = db.ParentsRepository.GetByID(id);

            if (found != null)
            {
                string subject = "E-School Account Created";
                string body = $"<p>Dear {found.FirstName} {found.LastName},<br/>" +
                    $"We are writing to let you know that the student account was assigned to your name at E-School.<br/> " +
                    $"<br/>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = found.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }


        public void NewMarkMailForParent(int markId)
        {
            Mark mark = db.MarksRepository.GetByID(markId);

            if (mark != null)
            {
                string subject = "E-School: The New Student Mark";
                string body = $"<p>Dear {mark.Student.Parent.FirstName} {mark.Student.Parent.LastName},<br/>" +
                    $"We are writing to inform you that student, {mark.Student.FirstName} {mark.Student.LastName}, " +
                    $"has received the following mark:<br/> " +
                    $"<br/>" +
                    $"Student: {mark.Student.FirstName} {mark.Student.LastName}<br/>" +
                    $"Form (grade-tag): {mark.Student.Form.Grade}-{mark.Student.Form.Tag}<br/>" +
                    $"Subject Name: {mark.FormToTeacherSubject.TeacherToSubject.Subject.Name}<br/>" +
                    $"Teacher: {mark.FormToTeacherSubject.TeacherToSubject.Teacher.FirstName} {mark.FormToTeacherSubject.TeacherToSubject.Teacher.LastName}<br/>" +
                    $"Mark Value: {mark.MarkValue}<br/>" +
                    $"Created: {mark.Created}<br/>" +
                    $"Semester: {mark.Semester}<br/>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"For any additional information, sign into your E-School account." +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = mark.Student.Parent.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }

        public void NewMarkMailForStudent(int markId)
        {
            Mark mark = db.MarksRepository.GetByID(markId);

            if (mark != null)
            {
                string subject = "E-School: The New Mark";
                string body = $"<p>Dear {mark.Student.FirstName} {mark.Student.LastName},<br/>" +
                    $"We are writing to inform you that you have received the following mark:<br/> " +
                    $"<br/>" +
                    $"Student: {mark.Student.FirstName} {mark.Student.LastName}<br/>" +
                    $"Form (grade-tag): {mark.Student.Form.Grade}-{mark.Student.Form.Tag}<br/>" +
                    $"Subject Name: {mark.FormToTeacherSubject.TeacherToSubject.Subject.Name}<br/>" +
                    $"Teacher: {mark.FormToTeacherSubject.TeacherToSubject.Teacher.FirstName} {mark.FormToTeacherSubject.TeacherToSubject.Teacher.LastName}<br/>" +
                    $"Mark Value: {mark.MarkValue}<br/>" +
                    $"Created: {mark.Created}<br/>" +
                    $"Semester: {mark.Semester}<br/>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"For any additional information, sign into your E-School account." +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = mark.Student.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }


        public void MarkUpdateMailForParent(int markId, int oldMarkValue)
        {
            Mark mark = db.MarksRepository.GetByID(markId);

            if (mark != null)
            {
                string subject = "E-School: The Student Mark Update";
                string body = $"<p>Dear {mark.Student.Parent.FirstName} {mark.Student.Parent.LastName},<br/>" +
                    $"We are writing to inform you that student's mark was updated.<br/>" +
                    $"The value was changed from {oldMarkValue} to {mark.MarkValue}.<br/> " +
                    $"<br/>" +
                    $"The updated mark:<br/> " +
                    $"<br/>" +
                    $"Student: {mark.Student.FirstName} {mark.Student.LastName}<br/>" +
                    $"Form (grade-tag): {mark.Student.Form.Grade}-{mark.Student.Form.Tag}<br/>" +
                    $"Subject Name: {mark.FormToTeacherSubject.TeacherToSubject.Subject.Name}<br/>" +
                    $"Teacher: {mark.FormToTeacherSubject.TeacherToSubject.Teacher.FirstName} {mark.FormToTeacherSubject.TeacherToSubject.Teacher.LastName}<br/>" +
                    $"Mark Value: {mark.MarkValue}<br/>" +
                    $"Created: {mark.Created}<br/>" +
                    $"Semester: {mark.Semester}<br/>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"For any additional information, sign into your E-School account." +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = mark.Student.Parent.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }

        public void MarkUpdateMailForStudent(int markId, int oldMarkValue)
        {
            Mark mark = db.MarksRepository.GetByID(markId);

            if (mark != null)
            {
                string subject = "E-School: The Mark Update";
                string body = $"<p>Dear {mark.Student.FirstName} {mark.Student.LastName},<br/>" +
                    $"We are writing to inform you that your mark was updated.<br/>" +
                    $"The value was changed from {oldMarkValue} to {mark.MarkValue}.<br/> " +
                    $"<br/>" +
                    $"The updated mark:<br/> " +
                    $"<br/>" +
                    $"Student: {mark.Student.FirstName} {mark.Student.LastName}<br/>" +
                    $"Form (grade-tag): {mark.Student.Form.Grade}-{mark.Student.Form.Tag}<br/>" +
                    $"Subject Name: {mark.FormToTeacherSubject.TeacherToSubject.Subject.Name}<br/>" +
                    $"Teacher: {mark.FormToTeacherSubject.TeacherToSubject.Teacher.FirstName} {mark.FormToTeacherSubject.TeacherToSubject.Teacher.LastName}<br/>" +
                    $"Mark Value: {mark.MarkValue}<br/>" +
                    $"Created: {mark.Created}<br/>" +
                    $"Semester: {mark.Semester}<br/>" +
                    $"<br/>" +
                    $"<br/>" +
                    $"For any additional information, sign into your E-School account." +
                    $"<br/>" +
                    $"<br/>" +
                    $"Sincerely,<br/>" +
                    $"Svetlana Topalov, E-School";

                string emailTo = mark.Student.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }

        public void CreateMailForAdminTeachersEngagementsReport(TeachersWeeklyEngagementsDTO report, string adminId)
        {
            Admin admin = db.AdminsRepository.GetByID(adminId);

            if (admin != null)
            {
                string subject = $"E-School: Teachers Weekly Engagements Report On The Date Of {report.OnDate}";

                string body = string.Empty;

                body += $"<p>Dear {admin.FirstName} {admin.LastName},<br/>" +
                       $"We are sending you Teachers Weekly Engagements Report on the date of {report.OnDate}.</p>" +
                       $"<br/>" +
                       $"<br/>";

                body += "<table border='1'><td><b><center> Teacher ID </center></b></td><td><b><center> Teacher </center></b></td><td><b><center> Number Of Classes Per Week </center></b></td>";

                foreach (var line in report.WeeklyEngagementsByTeachers)
                {
                    body += "<tr><td>" + line.TeacherID + "</td><td>" + line.Teacher + "</td><td><center>" + line.WeeklyEngagements + "</center></td></tr>";
                }

                body += "</table>";

                body += $"<br/>" +
                        $"<br/>" +
                        $"Sincerely,<br/>" +
                        $"Svetlana Topalov, E-School";

                string emailTo = admin.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }

        public void CreateReportCardMail(string studentId, ReportCardDTO reportCard)
        {
            Student student = db.StudentsRepository.GetByID(studentId);

            if (student != null)
            {
                string subject = $"E-School: Student Report Card";

                string body = string.Empty;

                body += $"<p>Dear {student.Parent.FirstName} {student.Parent.LastName},<br/>" +
                       $"We are sending you Report Card for Student {student.FirstName} {student.LastName} on the date of {DateTime.Today}.</p>" +
                       $"<br/>" +
                       $"<br/>" +

                $"<table border='1'>" +
                   $"<tr><td><b> School Year : </b></td><td colspan=3> {reportCard.SchoolYear} </td></tr>" +
                   $"<tr><td><b> Student : </b></td><td colspan=3> {reportCard.StudentId}, {reportCard.Student} </td></tr>" +
                   $"<tr><td><b> Form : </b></td><td colspan=3> {reportCard.Form} </td></tr>" +
                   $"<tr><td><b> Attending Teacher : </b></td><td colspan=3> {reportCard.AttendingTeacher} </td></tr>" +
                   $"<tr><td><b> Parent : </b></td><td colspan=3> {reportCard.Parent} </td></tr>" +
                   $"<tr><td colspan=4><b> Classes: </b></td></tr>" +
                   $"<tr><td><b><center> Subject </center></b></td><td><b><center> Teacher </center></b></td><td colspan=2><b><center> Mark </center></b></td></tr>";

                foreach (var line in reportCard.Classes)
                {
                    body += "<tr><td>" + line.Subject + "</td><td>" + line.Teacher + "</td><td colspan=2><center><b>" + line.FirstSemesterAverageMark + "</b></center></td></tr>";
                }

                body += "</table>";

                body += $"<br/>" +
                        $"<br/>" +
                        $"Sincerely,<br/>" +
                        $"Svetlana Topalov, E-School";

                string emailTo = student.Parent.Email;
                bool isBodyHtml = true;

                CreateMail(subject, body, emailTo, isBodyHtml);
            }
        }


    }
}


