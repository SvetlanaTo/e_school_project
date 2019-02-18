using eSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSchool.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository AuthRepository { get; }
        IGenericRepository<ApplicationUser> UsersRepository { get; }
        IGenericRepository<Admin> AdminsRepository { get; }
        IStudentsRepository StudentsRepository { get; }
        IParentsRepository ParentsRepository { get; }
        ITeachersRepository TeachersRepository { get; }
        IFormsRepository FormsRepository { get; } 
        ISubjectsRepository SubjectsRepository { get; }
        ITeachersToSubjectsRepository TeachersToSubjectsRepository { get; } 
        IFormsToTeacherSubjectsRepository FormsToTeacherSubjectsRepository { get; }
        IMarksRepository MarksRepository { get; }

        void Save();
    }
}

