using eSchool.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Infrastructure
{
    public class AuthContext : IdentityDbContext<ApplicationUser> 
    {
       
        public DbSet<Form> Forms { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<TeacherToSubject> TeachersToSubjects { get; set; }
        public DbSet<FormToTeacherSubject> FormsToTeacherSubjects { get; set; }

        public DbSet<Mark> Marks { get; set; }


        public AuthContext() : base("AuthContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AuthContext>());
            Database.SetInitializer(new Init());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Parent>().ToTable("parents");
            modelBuilder.Entity<Teacher>().ToTable("teachers");


            //1-N veza
            modelBuilder.Entity<Student>()
                    .HasRequired(student => student.Form)
                    .WithMany(form => form.Students);

            //1-N veza
            modelBuilder.Entity<Student>()
                    .HasRequired(student => student.Parent)
                    .WithMany(parent => parent.Students);

            //1-0..1 veza
            modelBuilder.Entity<Form>()
                    .HasRequired(form => form.AttendingTeacher)
                    .WithOptional(teacher => teacher.FormAttending);


            //1-N veza
            modelBuilder.Entity<TeacherToSubject>()
                .HasRequired(teacherToSubject => teacherToSubject.Teacher)
                .WithMany(teacher => teacher.TeachersSubjects);

            //1-N veza
            modelBuilder.Entity<TeacherToSubject>()
                .HasRequired(teacherToSubject => teacherToSubject.Subject)
                .WithMany(subject => subject.SubjectsTeachers);

            //1-N veza
            modelBuilder.Entity<FormToTeacherSubject>()
                .HasRequired(formToTeacherSubject => formToTeacherSubject.Form)
                .WithMany(form => form.FormsTeachersToSubjects)
                .WillCascadeOnDelete(false);

            //1-N veza
            modelBuilder.Entity<FormToTeacherSubject>()
                .HasRequired(formToTeacherSubject => formToTeacherSubject.TeacherToSubject)
                .WithMany(teacherToSubject => teacherToSubject.TeacherSubjectForms);

            //1-N veza
            modelBuilder.Entity<Mark>()
                .HasRequired(mark => mark.Student)
                .WithMany(student => student.Marks);

            //1-N veza
            modelBuilder.Entity<Mark>()
                .HasRequired(mark => mark.FormToTeacherSubject)
                .WithMany(formToTeacherSubject => formToTeacherSubject.Marks);


        }
    }
}