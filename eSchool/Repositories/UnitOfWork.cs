using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Unity.Attributes;

namespace eSchool.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private DbContext context;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        [Dependency]
        public IAuthRepository AuthRepository { get; set; }

        [Dependency]
        public IGenericRepository<ApplicationUser> UsersRepository { get; set; }

        [Dependency]
        public IGenericRepository<Admin> AdminsRepository { get; set; }

        [Dependency]
        public IStudentsRepository StudentsRepository { get; set; }

        [Dependency]
        public IParentsRepository ParentsRepository { get; set; }

        [Dependency]
        public ITeachersRepository TeachersRepository { get; set; }

        [Dependency]
        public IFormsRepository FormsRepository { get; set; } 

        [Dependency]
        public ISubjectsRepository SubjectsRepository { get; set; } 

        [Dependency]
        public ITeachersToSubjectsRepository TeachersToSubjectsRepository { get; set; } 

        [Dependency]
        public IFormsToTeacherSubjectsRepository FormsToTeacherSubjectsRepository { get; set; }

        [Dependency]
        public IMarksRepository MarksRepository { get; set; } 


        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}